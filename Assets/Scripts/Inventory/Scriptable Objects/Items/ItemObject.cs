using UnityEngine;

/// <summary>
/// Enumeration of all the itemType available in the game
/// </summary>
public enum ItemType
{
    Weapon,
    Armor,
    Potion
}
/// <summary>
/// Scriptable object that will be inherited by our derived class
/// </summary>
[System.Serializable]
public abstract class ItemObject : ScriptableObject
{
    public int _id;
    public Sprite _sprite;
    public ItemType _itemType;
    [TextArea(5, 10)]
    public string _description;

    public ItemType GetItemType { get { return _itemType; } }
}

[System.Serializable]
public class Item
{
    public int _id;
    public string _name;
    public Item(ItemObject item)
    {
        _id = item._id;
        _name = item._sprite.name;
    }

    public Item(int id, string name)
    {
        _id = id;
        _name = name;
    }
}