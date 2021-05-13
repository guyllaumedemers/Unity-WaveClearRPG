using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] items;
    public Dictionary<int, ItemObject> getItem = new Dictionary<int, ItemObject>();

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i]._id = i;
            getItem.Add(i, items[i]);
        }
        //foreach (KeyValuePair<int, ItemObject> kvp in getItem)
        //{
        //    Debug.Log("Key : " + kvp.Key + " ItemObject ID : " + kvp.Value._id);
        //}
    }

    public void OnBeforeSerialize()
    {
        getItem = new Dictionary<int, ItemObject>();
    }
}
