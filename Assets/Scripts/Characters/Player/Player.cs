using System.Collections.Generic;
using Characters;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour
{
    public static Player Instance { get; protected set; }
    public Stats playerStats;

    private Animator _animator;
    private AudioSource _audioSource;
    private HudController _hudController;
    [HideInInspector] public bool _isDead;

    public Animator Animator
    {
        get => _animator;
        set => _animator = value;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _displayInventory = GameObject.FindGameObjectWithTag("Inventory");
        _isDisplay = false;
        _hudController = GameObject.FindObjectOfType<HudController>();
        _isDead = false;
    }

    void Start()
    {
        playerStats.currentHealth = playerStats.maxHealth;
    }

    void Update()
    {
        _hudController?.UpdatePlayerHud(playerStats);

        if (playerStats.currentHealth <= 0 && !_isDead)
        {
            Die();
            return;
        }

        ToggleInventory();
        SaveAndLoad();
    }

    public void TakeDamage(float amount)
    {
        _audioSource.PlayOneShot(SoundManager.Instance.PlayerAudioClips["LTTP_Link_Hurt"]);
        _animator.SetTrigger("isHit");

        float damageReceived = amount / (playerStats.defense / 5);

        Debug.Log("Damage received: " + damageReceived);

        playerStats.ChangeHealth(-damageReceived);
        if (playerStats.currentHealth <= 0 && !_isDead) {
            Die();
        }
    }

    public void HealDamage(float amount)
    {
        playerStats.ChangeHealth(amount);
    }

    public void IncreaseDefense(float amount)
    {
        playerStats.IncreaseDefense(amount);
    }

    public void IncreaseStrength(float amount)
    {
        playerStats.IncreaseStrength(amount);
    }

    private void Die()
    {
        _isDead = true;
        _audioSource.PlayOneShot(SoundManager.Instance.PlayerAudioClips["LTTP_Link_Dying"]);
        GameObject.Find("MainScripts").GetComponent<AudioSource>().Stop();
        _animator.SetTrigger("isDead");
        Debug.Log("Player died");
    }

    #region INVENTORY SYSTEM

    public MouseItem _mouseItem = new MouseItem();
    public InventoryObject _inventory;
    private GameObject _displayInventory;
    public bool _isDisplay;

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

    public void SaveAndLoad()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _inventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _inventory.Load();
        }
    }
    public void ToggleInventory()
    {
        if (GameController.Instance._canOpenInventory)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && GameController.Instance._waveReady)
            {
                _isDisplay = !_isDisplay;
            }
            _displayInventory.SetActive(_isDisplay);
        }
    }
    #endregion
}