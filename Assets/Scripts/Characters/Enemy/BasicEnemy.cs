using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Characters.Enemy
{
    public enum State
    {
        Wander,
        Chase,
        Attack,
        Dead
    }
    
    [System.Serializable]
    public class BasicEnemy : MonoBehaviour, IEnemy
    {
        [Header("AI Settings")]
        public float startingAngle = -60;
        public float stepAngle = 5;
        public float sightDistance = 15f;
        public float attackDistance = 2f;
        public float attackSpeed = 2f;
        public float wanderRadius = 20f;
        public State currentState;
        [Header("Items")]
        public int minItemDrops = 0;
        public int maxItemDrops = 3;
        public List<GameObject> itemDrops;
        [field: SerializeField]
        public Stats EnemyStats { get; set; }
        public float minDamage = 1f;
        public float maxDamage = 5f;
        
        private Animator _animator;
        private Slider _healthSlider;
        private Transform _playerTransform;
        
        private IAstarAI _astar;
        private AIDestinationSetter _destinationSetter;

        private Transform itemDropsParent;

        private int _layerMask;
        private Quaternion _startingAngle;
        private Quaternion _stepAngle;

        private bool _isDead;
        private float _atkTimer;
        
        private void Start() {
            EnemyStats.maxHealth *= (GameController.Instance._currentWave * EnemyStats.healthMultiplier);
            minDamage *= (GameController.Instance._currentWave * EnemyStats.damageMultiplier);
            maxDamage *= (GameController.Instance._currentWave * EnemyStats.damageMultiplier);

            EnemyStats.currentHealth = EnemyStats.maxHealth;
            
            _animator = GetComponent<Animator>();
            _healthSlider = GetComponentInChildren<Slider>();
            currentState = State.Wander;
            
            _playerTransform = global::Player.Instance.transform;
            
            _astar = GetComponent<IAstarAI>();
            _destinationSetter = GetComponent<AIDestinationSetter>();
            _destinationSetter.target = null;
            
            _layerMask = ~(1 << gameObject.layer);
            _startingAngle = Quaternion.AngleAxis(startingAngle, Vector3.up);
            _stepAngle = Quaternion.AngleAxis(stepAngle, Vector3.up);

            _atkTimer = attackSpeed;

            itemDropsParent = GameObject.Find("ItemDrops").transform;
        }
        
        void Update() {

            _atkTimer -= Time.deltaTime;
            
            switch(currentState) {
                
                case State.Wander:
                    if (CheckForAggro() && !global::Player.Instance._isDead) {
                        EnablePathfinding(_playerTransform);
                        currentState = State.Chase;
                    } else
                    {
                        currentState = State.Wander;
                        Wander();
                    }
                    break;
                
                case State.Chase:
                    if (IsChasingPlayer()) {
                        if (IsAttackDistance()) {
                            currentState = State.Attack;
                        }
                    }
                    else{
                        currentState = State.Wander;
                        Wander();
                    }
                    break;
                
                case State.Attack:
                    if (!global::Player.Instance._isDead) {
                        if (_atkTimer <= 0) {
                            Attack();
                            _atkTimer = attackSpeed;
                        }
                    }
                    else {
                        currentState = State.Wander;
                    }

                    break;
                
                case State.Dead:
                    if (!_isDead)
                        Die();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsAttackDistance() {
            if (Vector3.Distance(_destinationSetter.target.position, transform.position) < attackDistance)
                return true;
            return false;
        }

        private bool IsChasingPlayer() {
            if (Vector3.Distance(_destinationSetter.target.position, transform.position) > sightDistance)
            {
                return false;
            }
                
            return true;
        }

        private bool Wander()
        {
            _animator.SetBool("isWalking", true);
            _destinationSetter.target = null;

            if (!_astar.pathPending && (_astar.reachedEndOfPath || !_astar.hasPath))
            {
                _astar.destination = PickRandomWanderPoint();
                _astar.SearchPath();
            }
            return true;
        }

        public void EnablePathfinding(Transform target) {
            _animator.SetBool("isWalking", true);
            
            _astar.canMove = true;
            _destinationSetter.enabled = true;
            _destinationSetter.target = target;
        }

        public void DisablePathFinding() {
            _animator.SetBool("isWalking", false);
            
            _astar.canMove = false;
            _destinationSetter.target = null;
            _destinationSetter.enabled = false;
        }
        
        public void Attack() {
            if (IsAttackDistance()) {
                _animator.SetTrigger("isAttk");
                transform.LookAt(_destinationSetter.target);
                float damage = Random.Range(minDamage, maxDamage);
                global::Player.Instance.TakeDamage(damage);
            }
            else {
                currentState = State.Chase;
            }
        }

        public void TakeDamage(float amount) {
            if (currentState != State.Dead) {
                //Debug.Log($"{gameObject.name} : Hit for {amount}");
                EnemyStats.currentHealth -= amount;

                _healthSlider.value = EnemyStats.currentHealth / EnemyStats.maxHealth;
                
                currentState = State.Chase;
                EnablePathfinding(_playerTransform);
            
                if (EnemyStats.currentHealth <= 0 && !_isDead) { 
                    //Debug.Log($"{gameObject.name} : Died");
                    currentState = State.Dead;
                }
            }
        }
        
        public void Die() {

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

        private bool CheckForAggro() {
            RaycastHit hit;
            var angle = transform.rotation * _startingAngle;
            var direction = angle * Vector3.forward;
            var pos = transform.position + Vector3.up;

            for (var i = 0; i < 24; i++) {
                if (Physics.Raycast(pos, direction, out hit, sightDistance, _layerMask)) {
                    if (hit.collider.tag.Equals("Player")) {
                         Debug.DrawRay(pos, direction * hit.distance, Color.red);
                         return true;
                    }
                    else {
                        Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                    }
                }
                else {
                    Debug.DrawRay(pos, direction * sightDistance, Color.white);
                }

                direction = _stepAngle * direction;
            }
            
            return false;
        }

        private Vector3 PickRandomWanderPoint()
        {
            var point = UnityEngine.Random.insideUnitSphere * wanderRadius;

            point.y = 0;
            point += _astar.position;

            return point;
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

