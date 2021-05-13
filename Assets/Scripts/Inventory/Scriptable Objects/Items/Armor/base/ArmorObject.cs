using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor Object", menuName = "Inventory System/Items/Armor")]
public class ArmorObject : ItemObject
{
    public int _defenseBonus;
    public void Awake()
    {
        _itemType = ItemType.Armor;
    }
}
