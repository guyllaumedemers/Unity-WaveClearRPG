using Characters.Shields;
using Characters.Weapons;
using UnityEngine;

namespace Characters
{
    public class PlayerWeaponController : MonoBehaviour
    {

        public GameObject rightHand;
        public GameObject leftHand;

        public GameObject EquipedWeapon { get; set; }
        public GameObject EquipedShield { get; set; }

        private Stats _playerStats;
        private Animator _animator;

        void Start()
        {
            _playerStats = GetComponent<global::Player>().playerStats;
            EquipedWeapon = rightHand.GetComponentInChildren<BasicSword>().gameObject;
        }

        private void Update()
        {
            if (GameController.Instance._waveReady && !global::Player.Instance._isDisplay && !GameController.Instance._hasWon && !GameController.Instance._hasLost)
                if (Input.GetKeyDown(KeyCode.Space))
                    WeaponAttack();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                ShieldBlock();
        }

        public void EquipWeapon(GameObject weapon)
        {
            if (EquipedWeapon != null)
            {
                _playerStats.RemoveBonus(EquipedWeapon.GetComponent<IWeapon>().WeaponStats);
                Destroy(rightHand.transform.GetChild(0).gameObject);
            }

            EquipedWeapon = Instantiate(weapon, rightHand.transform.position, rightHand.transform.rotation * weapon.transform.rotation);
            _playerStats.AddStatBonus(EquipedWeapon.GetComponent<IWeapon>().WeaponStats);
            EquipedWeapon.transform.SetParent(rightHand.transform);

            Debug.Log($"Equipped Weapon {weapon}");
        }

        public void EquipShield(GameObject shield)
        {

            if (EquipedShield != null)
            {
                _playerStats.RemoveBonus(EquipedShield.GetComponent<IShield>().ShieldStats);
                Destroy(leftHand.transform.GetChild(0).gameObject);
            }

            EquipedShield = Instantiate(shield, leftHand.transform.position, leftHand.transform.rotation);
            _playerStats.AddStatBonus(EquipedShield.GetComponent<IShield>().ShieldStats);
            EquipedShield.transform.SetParent(leftHand.transform);

            Debug.Log($"Equipped Shield {shield}");
        }

        private void WeaponAttack()
        {
            EquipedWeapon?.GetComponent<IWeapon>().Attack();
        }

        private void ShieldBlock()
        {
            EquipedShield?.GetComponent<IShield>().Block();
            if (!global::Player.Instance.Animator.GetBool("isBlocking"))
            {
                Debug.Log("Blocking");
                global::Player.Instance.Animator.SetBool("isBlocking", true);
            }
            else
            {
                Debug.Log("Not Blocking");
                global::Player.Instance.Animator.SetBool("isBlocking", false);
            }

        }
    }
}
