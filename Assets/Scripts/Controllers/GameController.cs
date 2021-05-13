using Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Singleton
    private static GameController instance;

    private GameController() { }

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameController();
            }
            return instance;
        }
    }
    #endregion

    private HudController hudController;

    private Transform enemiesParent;
    public GameObject[] enemiesPrefabs;
    public GameObject[] spawners;

    [HideInInspector]
    public int _currentWave = 1;
    public int _maxWaves = 5;
    public int _maxEnemiesPerWave = 10;
    [HideInInspector]
    public int _totalEnemiesSpawned = 0;
    [HideInInspector]
    public int _nbEnemies = 0, _nbMimic = 0, _nbBeholder = 0;

    public float _enemySpawnDelayMin = 3f;
    public float _enemySpawnDelayMax = 8f;
    private float _enemySpawnDelay;
    private float _timeUntilNextSpawn = 0f;
    private float _spawnRadiusMin = -8f;
    private float _spawnRadiusMax = 8f;

    public float _timeBetweenWave = 10;
    [HideInInspector]
    public float _waveTimeRemaining = 0;

    [HideInInspector]
    public bool _waveReady = false, _waveSpawned = false, _hasWon = false, _hasLost = false, _isPaused = false;

    [HideInInspector]
    public bool _canInteract = false, _canOpenInventory = false;

    private void Awake()
    {
        instance = this;

        _waveTimeRemaining = _timeBetweenWave;

        hudController = GameObject.FindObjectOfType<HudController>();

        enemiesParent = GameObject.Find("Enemies").transform;

        spawners = GameObject.FindGameObjectsWithTag("Spawner");

        Time.timeScale = 1;

        ResetTimers();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteractions();

        if (_waveTimeRemaining > 0 && !_waveReady)
        {
            _waveTimeRemaining -= Time.deltaTime;
        }
        else
        {
            _waveReady = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            hudController.ShowMenu();
        }

        if (!_isPaused && spawners.Length > 0 && _waveReady)
        {
            if (_totalEnemiesSpawned < _maxEnemiesPerWave)
            {
                if (Time.time > _timeUntilNextSpawn)
                {
                    SpawnEnemy();
                    ResetTimers();
                }
            }
            else
            {
                _waveSpawned = true;
                _waveTimeRemaining = _timeBetweenWave;
            }

            if (_waveSpawned && _nbEnemies == 0 && _currentWave != _maxWaves)
            {
                ResetTimers();
                _maxEnemiesPerWave *= 2;
                _totalEnemiesSpawned = 0;
                _currentWave++;
                _waveSpawned = false;
                _waveReady = false;
            }
        }
    }

    void ResetTimers()
    {
        _enemySpawnDelay = Random.Range(_enemySpawnDelayMin, _enemySpawnDelayMax);
        _timeUntilNextSpawn = Time.time + _enemySpawnDelay;
    }

    void SpawnEnemy()
    {
        int randomSpawnerIndex = Random.Range(0, spawners.Length);
        int randomEnemyIndex = Random.Range(0, enemiesPrefabs.Length);

        GameObject enemyClone = Instantiate(enemiesPrefabs[randomEnemyIndex], new Vector3(spawners[randomSpawnerIndex].transform.position.x + Random.Range(_spawnRadiusMin, _spawnRadiusMax), spawners[randomSpawnerIndex].transform.position.y, spawners[randomSpawnerIndex].transform.position.z + Random.Range(_spawnRadiusMin, _spawnRadiusMax)), Quaternion.identity);
        enemyClone.transform.SetParent(enemiesParent.transform);

        if (enemyClone.name.Contains("Mimic"))
        {
            _nbMimic++;
        }
        else
        {
            _nbBeholder++;
        }

        _totalEnemiesSpawned++;
        _nbEnemies++;
    }

    public bool CheckWinConditions()
    {
        if (_waveSpawned && _nbEnemies == 0 && _currentWave == _maxWaves)
        {
            _hasWon = true;
        }

        return _hasWon;
    }

    public bool CheckLoseConditions()
    {
        if (Player.Instance.playerStats.currentHealth <= 0)
        {
            _hasLost = true;
        }

        return _hasLost;
    }

    private void CheckInteractions()
    {
        if (!_hasWon && !_hasLost && !_isPaused)
        {
            _canOpenInventory = true;
        }
        else
        {
            _canOpenInventory = false;
        }

        if (_waveReady && !_hasWon && !_hasLost)
        {
            _canInteract = true;
        }
        else
        {
            _canInteract = false;
        }

    }
}
