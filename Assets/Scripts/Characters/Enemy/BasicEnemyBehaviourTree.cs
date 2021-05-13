
using System;
using System.Collections.Generic;
using Panda;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Characters.Enemy
{

    [System.Serializable]
    public class BasicEnemyBehaviourTree : MonoBehaviour, IEnemy
    {
        public float startingAngle = -60;
        public float stepAngle = 5;
        public float sightDistance = 15f;
        public float attackDistance = 2f;
        public float wanderRadius = 20f;
        public int minItemDrops = 0;
        public int maxItemDrops = 3;
        public List<GameObject> itemDrops;

        private Transform itemDropsParent;
        [field: SerializeField]
        public Stats EnemyStats { get; set; }
        public float minDamage = 1f;
        public float maxDamage = 5f;
        
        private Animator _animator;
        private Slider _healthSlider;
        private Transform _playerTransform;

        private IAstarAI _astar;
        private AIDestinationSetter _destinationSetter;

        private int _layerMask;
        private Quaternion _startingAngle;
        private Quaternion _stepAngle;

        private bool _isDead;
        private bool _hasAggro;

        private void Start()
        {
            EnemyStats.maxHealth *= (GameController.Instance._currentWave * EnemyStats.healthMultiplier);
            minDamage *= (GameController.Instance._currentWave * EnemyStats.damageMultiplier);
            maxDamage *= (GameController.Instance._currentWave * EnemyStats.damageMultiplier);

            EnemyStats.currentHealth = EnemyStats.maxHealth;

            _animator = GetComponent<Animator>();
            _healthSlider = GetComponentInChildren<Slider>();

            _playerTransform = global::Player.Instance.transform;

            _astar = GetComponent<IAstarAI>();
            _destinationSetter = GetComponent<AIDestinationSetter>();
            _destinationSetter.target = null;

            _layerMask = ~(1 << gameObject.layer);
            _startingAngle = Quaternion.AngleAxis(startingAngle, Vector3.up);
            _stepAngle = Quaternion.AngleAxis(stepAngle, Vector3.up);
        }

        [Task]
        private void IsAttackDistance()
        {
            float distance = 0;
            if (_destinationSetter.target != null)
            {
                distance = Vector3.Distance(_destinationSetter.target.position, transform.position);
                if (distance < attackDistance)
                {
                    // attack
                    Task.current.Succeed();
                    return;
                }
            }
        }

        [Task]
        private void IsChasingPlayer()
        {
            float distance = 0;
            if (_destinationSetter.target != null)
            {
                distance = Vector3.Distance(_destinationSetter.target.position, transform.position);
                if (distance > sightDistance)
                {
                    // stop aggroing
                    _hasAggro = false;
                    _destinationSetter.target = null;
                    return;
                }
            }
            if (_hasAggro)
            {
                Task.current.Succeed();
            }
        }

        [Task]
        private void CheckForAggro()
        {
            RaycastHit hit;
            var angle = transform.rotation * _startingAngle;
            var direction = angle * Vector3.forward;
            var pos = transform.position + Vector3.up;

            for (var i = 0; i < 24; i++)
            {
                if (Physics.Raycast(pos, direction, out hit, sightDistance, _layerMask))
                {
                    if (hit.collider.tag.Equals("Player"))
                    {
                        Debug.DrawRay(pos, direction * hit.distance, Color.red);
                        _hasAggro = true;
                        break;
                    }
                    else
                    {
                        Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                    }
                }
                else
                {
                    Debug.DrawRay(pos, direction * sightDistance, Color.white);
                }
                direction = _stepAngle * direction;
            }
            Task.current.Succeed();
        }

        [Task]
        private void HasAggro()
        {
            if (_hasAggro)
            {
                return;
            }
            Task.current.Succeed();
        }

        [Task]
        private void Wander()
        {
            _animator.SetBool("isWalking", true);

            if (!_astar.pathPending && (_astar.reachedEndOfPath || !_astar.hasPath))
            {
                _astar.destination = PickRandomWanderPoint();
                _astar.SearchPath();
            }
            Task.current.Succeed();
        }

        private Vector3 PickRandomWanderPoint()
        {
            var point = UnityEngine.Random.insideUnitSphere * wanderRadius;

            point.y = 0;
            point += _astar.position;

            return point;
        }


        /// <summary>
        /// Use to set destination of the instance to a specific point on the map or to follow a target
        /// </summary>
        [Task]
        public void EnablePathfinding()
        {
            // need to set destination to target here
            if (_hasAggro)
            {
                _animator.SetBool("isWalking", true);

                _astar.canMove = true;
                _destinationSetter.enabled = true;
                _destinationSetter.target = global::Player.Instance.transform;
                return;
            }
            else
            {
                _destinationSetter.target = null;
            }
        }

        [Task]
        public void Attack()
        {
            _animator.SetTrigger("isAttk");

            transform.LookAt(_destinationSetter.target);
            if (!global::Player.Instance._isDead)
            {
                float damage = Random.Range(minDamage, maxDamage);
                global::Player.Instance.TakeDamage(damage);
                Task.current.Succeed();
                return;
            }
        }

        public void TakeDamage(float amount)
        {
            //Debug.Log($"{gameObject.name} : Hit for {amount}");
            EnemyStats.currentHealth -= amount;

            _healthSlider.value = EnemyStats.currentHealth / EnemyStats.maxHealth;
            // when this instance is attacked or receive damage it set a destination to the player's transform
            _hasAggro = true;

            if (EnemyStats.currentHealth <= 0 && !_isDead)
            {
                //Debug.Log($"{gameObject.name} : Died");
                Die();
            }
        }

        public void Die()
        {
            _animator.SetTrigger("isDead");

            if (!_isDead)
            {
                GameController.Instance._nbEnemies--;

                if (gameObject.name.Contains("Mimic"))
                {
                    GameController.Instance._nbMimic--;
                }
                else
                {
                    GameController.Instance._nbBeholder--;
                }
                DisablePathFinding();
                _isDead = true;
            }
            DropItems();
            Destroy(gameObject, 5f);
        }

        public void DisablePathFinding()
        {
            _animator.SetBool("isWalking", false);

            _astar.canMove = false;
            _destinationSetter.target = null;
            _destinationSetter.enabled = false;
        }

        [Task]
        public void IsPlayerDead()
        {
            if (global::Player.Instance._isDead)
            {
                Task.current.Succeed();
                return;
            }
        }

        [Task]
        public void IsAlive()
        {
            if (!_isDead)
            {
                Task.current.Succeed();
                return;
            }
            Task.current.Fail();
        }

        private void DropItems()
        {
            int randomNbItemDrop = Random.Range(minItemDrops, maxItemDrops + 1);

            //Debug.Log("Number of items dropped: " + randomNbItemDrop);

            for (int i = 0; i < randomNbItemDrop; i++)
            {
                int randomItemIndex = Random.Range(0, itemDrops.Count);

                //Debug.Log("Item index: " + randomItemIndex);

                GameObject itemClone = Instantiate(itemDrops[randomItemIndex], new Vector3(transform.position.x + Random.Range(-4f, 4f), transform.position.y + 1.5f, transform.position.z + Random.Range(-4f, 4f)), Quaternion.identity);
                itemClone.transform.SetParent(itemDropsParent);
            }
        }
    }
}

