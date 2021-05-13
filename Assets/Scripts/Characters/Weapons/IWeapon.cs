using UnityEngine;

namespace Characters.Weapons
{
    public interface IWeapon
    {

        Stats WeaponStats { get; set; }

        void Attack();
        void OnTriggerEnter(Collider collider);
    }
}
