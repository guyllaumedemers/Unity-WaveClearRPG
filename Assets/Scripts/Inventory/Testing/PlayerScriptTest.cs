using UnityEngine;

public class PlayerScriptTest : MonoBehaviour
{
    public MouseItem _mouseItem = new MouseItem();
    public InventoryObject _inventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            _inventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _inventory.Load();
        }
    }

    /// <summary>
    /// When trigerred, we look for the Item Class attached to the gameobject, we than retrieve the scriptable object attached to it,
    /// add it to our inventory list and destroy the object we just collided with
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        GroundItem item = other.GetComponent<GroundItem>();
        if (item != null)
        {
            _inventory.AddItem(new Item(item._item), 1);
            Destroy(other.gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        _inventory._container._itemsList = new InventorySlot[24];
    }
}
