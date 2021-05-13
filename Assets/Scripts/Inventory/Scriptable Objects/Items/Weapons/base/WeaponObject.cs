using Characters;
using System.Collections;
using System.Collections.Generic;
using Characters.Weapons;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Object", menuName = "Inventory System/Items/Weapon")]
public class WeaponObject : ItemObject, IWeapon
{
    public float _attackBonus;
    public float _defenseBonus;
    public bool _isRange;
    public float _maxDistance;
    public int _maxHp, _str, _agi, _def;

    public Stats WeaponStats { get; set; }

    public void Attack()
    {
        Debug.Log($"{GetType().Name} Attack");
    }

    public void OnTriggerEnter(Collider collider)
    {

    }

    public void Awake()
    {
        _itemType = ItemType.Weapon;
    }
    public void Start() {
        WeaponStats.maxHealth = _maxHp;
        WeaponStats.strength = _str;
        WeaponStats.defense = _def;
    }

}
