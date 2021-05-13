using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Characters.Player;
/// <summary>
/// InventoryManager class is there to handle all the events and behaviour regarding the display of the inventory
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public InventoryObject _inventory;
    public Player player;
    public GameObject _emptyInventoryStorageDisplay;
    private GridLayoutGroup layoutGroup;

    public Dictionary<GameObject, InventorySlot> _itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

    public void Start()
    {
        layoutGroup = GetComponent<GridLayoutGroup>();
        layoutGroup.cellSize = new Vector2(Screen.width / 20.0f, Screen.height / 11.25f);
        CreateSlot();
    }

    public void Update()
    {
        UpdateDisplay();
    }

    public void CreateSlot()
    {
        _itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < _inventory._container._itemsList.Length; i++)
        {
            GameObject go = Instantiate(_emptyInventoryStorageDisplay, new Vector3(0, 0, 0), Quaternion.identity, transform);
            AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnter(go); });
            AddEvent(go, EventTriggerType.PointerExit, delegate { OnExit(go); });
            AddEvent(go, EventTriggerType.BeginDrag, delegate { OnDragEnter(go); });
            AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
            AddEvent(go, EventTriggerType.EndDrag, delegate { OnDragExit(go); });
            AddEvent(go, EventTriggerType.PointerClick, delegate { OnClick(go); });

            _itemsDisplayed.Add(go, _inventory._container._itemsList[i]);
        }
    }

    protected void UpdateDisplay()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> kvp in _itemsDisplayed)
        {
            if (kvp.Value._id >= 0)
            {
                kvp.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _inventory._databaseObject.getItem[kvp.Value._item._id]._sprite;
                kvp.Key.GetComponentInChildren<TextMeshProUGUI>().text = kvp.Value._amount.ToString();
            }
            else
            {
                kvp.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _emptyInventoryStorageDisplay.GetComponentInChildren<Image>().sprite;
                kvp.Key.GetComponentInChildren<TextMeshProUGUI>().text = "0";
            }
        }
    }

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnClick(GameObject obj)
    {
        if (_itemsDisplayed.ContainsKey(obj) && !_itemsDisplayed[obj]._item._id.Equals(-1))
        {
            ItemObject item = _inventory._databaseObject.getItem[_itemsDisplayed[obj]._item._id];
            if (item._itemType.Equals(ItemType.Potion))
            {
                PotionObject potionObject = (PotionObject)item;
                switch (item._id)
                {
                    case 0:
                        global::Player.Instance.HealDamage(potionObject._healthRestore);
                        break;
                    case 1:
                        global::Player.Instance.IncreaseDefense(potionObject._defenseBonus);
                        break;
                    case 2:
                        global::Player.Instance.IncreaseStrength(potionObject._strengthBonnus);
                        break;
                }
                if (_itemsDisplayed[obj]._amount > 0)
                {
                    _itemsDisplayed[obj]._amount--;
                    if (_itemsDisplayed[obj]._amount == 0)
                    {
                        _inventory.RemoveItem(_itemsDisplayed[obj]._item);
                    }
                }
            }
        }
    }


    /// <summary>
    /// We listen to the event onEnter -> the trigerred object is the one we hover on, if its part of our Dictionnary<> for displaying items, we add the InventorySlot
    /// attached to it to the mouseItem.hoverItem (So we are in possession of the Inventory Slot and can drag and rop it where we want)
    /// </summary>
    /// <param name="obj"></param>
    public void OnEnter(GameObject obj)
    {
        player._mouseItem._hoverGameObject = obj;
        if (_itemsDisplayed.ContainsKey(obj))
        {
            player._mouseItem._hoverItemSlot = _itemsDisplayed[obj];
        }
    }
    /// <summary>
    /// OnExit, we reset our mouseItem
    /// </summary>
    /// <param name="obj"></param>
    public void OnExit(GameObject obj)
    {
        player._mouseItem._hoverItemSlot = null;
        player._mouseItem._hoverGameObject = null;
    }

    public void OnDragEnter(GameObject obj)
    {
        GameObject mouseObject = new GameObject();
        RectTransform rt = mouseObject.AddComponent<RectTransform>();
        // we size the grab object to the size of our inventory slot
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);
        // if the inventorySlot selected is bigger than -1, meaning it is initialize with a instance that contain an item
        if (_itemsDisplayed[obj]._id >= 0)
        {
            Image img = mouseObject.AddComponent<Image>();
            img.sprite = _inventory._databaseObject.getItem[_itemsDisplayed[obj]._item._id]._sprite;
            img.raycastTarget = false;
        }
        // we than set the values retrieve from the dictionnary to the mouseItem so we can swap later
        // our mouse Item now contains a gameobject and an inventorySlot
        player._mouseItem._gameObject = mouseObject;
        player._mouseItem._itemSlot = _itemsDisplayed[obj];
    }
    /// <summary>
    /// We verify that we indeed have an instance of a gameobject in hand that we can move around
    /// </summary>
    /// <param name="obj"></param>
    public void OnDrag(GameObject obj)
    {
        if (player._mouseItem._gameObject != null)
        {
            player._mouseItem._gameObject.transform.position = Input.mousePosition; // a test
        }
    }
    /// <summary>
    /// We than move the items in Inventory by swaping the values retrieve from the Dictionnary<> use for displaying Inventory Slot
    /// </summary>
    /// <param name="obj"></param>
    public void OnDragExit(GameObject obj)
    {
        if (player._mouseItem._hoverGameObject)
        {
            _inventory.MoveItem(_itemsDisplayed[player._mouseItem._hoverGameObject], _itemsDisplayed[obj]);
        }
        else
        {
            _inventory.RemoveItem(_itemsDisplayed[obj]._item);
        }
        Destroy(player._mouseItem._gameObject);
        player._mouseItem._itemSlot = null;
    }
}

public class MouseItem
{
    public GameObject _gameObject;
    public InventorySlot _itemSlot;
    public InventorySlot _hoverItemSlot;
    public GameObject _hoverGameObject;
}