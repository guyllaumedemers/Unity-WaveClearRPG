using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion Object", menuName = "Inventory System/Items/Potion")]
public class PotionObject : ItemObject
{
    public int _healthRestore;
    public int _defenseBonus;
    public int _strengthBonnus;
    public void Awake()
    {
        _itemType = ItemType.Potion;
    }
}
