using System;
using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using UnityEngine;

namespace Characters.Weapons {
    public class BasicSword : MonoBehaviour, IWeapon {

        public Stats WeaponStats { get; set; }
        public float dmg = 5;
        public float attackDelay = 0.5f;
        private float timeBetweenAttacks = 0f;

        private BoxCollider _collider;
        private AudioSource _audioSource;
        
        void Start() {
            _collider = GetComponent<BoxCollider>();
            _audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            dmg = global::Player.Instance.playerStats.strength * 0.5f;
        }
        
        public void Attack() {
            if(Time.time >= timeBetweenAttacks)
            {
                //Debug.Log($"{GetType().Name} Attack");
                StartCoroutine(EnableCollider());
                _audioSource.PlayOneShot(SoundManager.Instance.ItemsAudioClips["LTTP_Sword1"]);
                global::Player.Instance.Animator.SetTrigger("isAttacking");

                timeBetweenAttacks = Time.time + attackDelay;
            }    
        }
        
        public void OnTriggerEnter(Collider collider) {
            if (collider.CompareTag("Enemy")) {
                collider.GetComponent<IEnemy>()?.TakeDamage(dmg);
            }
        }
        
        IEnumerator EnableCollider () {
            _collider.enabled = true;
            yield return new WaitForSeconds(0.5f); 
            _collider.enabled = false;
        }
    }
}
