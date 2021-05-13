using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    string savePath = "Assets/Scripts/Inventory/Database/storage.txt";
    public ItemDatabaseObject _databaseObject;
    public Inventory _container;

    public void AddItem(Item item, int amount)
    {
        //Debug.Log("New Item ID to compare : " + item._id + " ");
        bool hasItem = false;
        for (int i = 0; i < _container._itemsList.Length; i++)
        {
            // Quand le Item Minor Health Potion -> Item._id = 0 est ajouté, il n'initialise pas le ID du Inventory Slot 
            if (_container._itemsList[i]._item._id.Equals(item._id))
            {
                //Debug.Log("Already in Inventory Array, amount++ : " + _container._itemsList[i]._item._id + " ");
                _container._itemsList[i].AddAmount(amount);
                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            //Debug.Log("ToAdd : ");
            SetEmptySlot(item, amount);
        }
    }

    public InventorySlot SetEmptySlot(Item item, int amount)
    {
        for (int i = 0; i < _container._itemsList.Length; i++)
        {
            // when the id of the InventorySlot == -1, it means the inventory slot is empty, so we fill the inventory slot 
            // with the one from the object we collided with
            if (_container._itemsList[i]._id.Equals(-1))
            {
                //Debug.Log("Added : ");
                _container._itemsList[i].UpdateSlot(i, item, amount);
                return _container._itemsList[i];
            }
        }
        // setup for when inventory is full
        return null;
    }

    public void MoveItem(InventorySlot slot1, InventorySlot slot2)
    {
        InventorySlot temp = new InventorySlot(slot2._id, slot2._item, slot2._amount);
        slot2.UpdateSlot(slot1._id, slot1._item, slot1._amount);
        slot1.UpdateSlot(temp._id, temp._item, temp._amount);
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < _container._itemsList.Length; i++)
        {
            if (_container._itemsList[i]._item._id.Equals(item._id))
            {
                _container._itemsList[i].UpdateSlot(-1, new Item(-1, ""), 0);
                break;
            }
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        //IFormatter formatter = new BinaryFormatter();
        //Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        //formatter.Serialize(stream, _container);
        //stream.Close();

        string json = JsonUtility.ToJson(_container);
        File.WriteAllText(savePath, json);
    }
    [ContextMenu("Load")]
    public void Load()
    {
        //if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
        //    _container = (Inventory)formatter.Deserialize(stream);
        //    stream.Close();
        //}
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            Clear();
            _container = JsonUtility.FromJson<Inventory>(json);
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        _container = new Inventory();
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] _itemsList = new InventorySlot[24];
}

[System.Serializable]
public class InventorySlot
{
    public int _id;
    public Item _item;
    public int _amount;
    public InventoryManager _parent;
    /// <summary>
    /// Constructeur par defaut utilisé pour instancier le array qui compose l'inventaire -> _id = -1 pour faire un verification check lors du processus d'ajout d'item
    /// </summary>
    public InventorySlot()
    {
        _id = -1;
        _item = new Item(-1, "");
        _amount = 0;
    }
    public InventorySlot(int id, Item item, int amount)
    {
        _id = id;
        _item = item;
        _amount = amount;
    }
    /// <summary>
    /// Taking into account that we could create a banking system from this, we have to assume that the value of increment will not always be 1
    /// </summary>
    /// <param name="value"></param>
    public void AddAmount(int value)
    {
        _amount += value;
    }

    public void UpdateSlot(int id, Item item, int amount)
    {
        _id = id;
        _item = item;
        _amount = amount;
    }
}
