using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private int _enemiesPerWave = 1;
    [SerializeField]
    private float _timeBetweenEnemies = 3;
    [SerializeField]
    private float _timeBetweenWaves = 6f;
    [SerializeField]
    private GameObject[] _powerUps;
    [SerializeField]
    private float _powerUpSpawnMin = 5f;
    [SerializeField]
    private float _powerUpSpawnMax = 10f;
    [SerializeField]
    private GameObject _bossPrefab;
    [SerializeField]
    private Transform _bossSpawnPoint;
    [SerializeField] public bool _stopSpawning = false;
    [SerializeField] public bool _bossDestroyed;
    [SerializeField] public int _waveCount = 0;
    private int _activeEnemies = 0;
    [SerializeField]
    private int[] weights = { 0, 6 };
    private int enemyNumber = 0;
    [SerializeField]
    private GameObject _alienBossPrefab;
    [SerializeField]
    private Transform _alienBossPosition;

    public void StartSpawning()
    {
        StartCoroutine(EnemyWaveSequence());
        StartCoroutine(PowerUpSpawnRoutine());
    }

    private IEnumerator EnemyWaveSequence()
    {
        yield return new WaitForSeconds(3f);
        while (!_stopSpawning)
        {
            if (_waveCount <= 4)
            {
                _waveCount++;
                for (int i = 0; i < _enemiesPerWave; i++)
                {
                    Vector3 spawnPos = new Vector3(Random.Range(-9f, 9f), 7.70f + Random.Range(-1f, 1f), 0f);
                    GameObject newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity, _enemyContainer.transform);
                    _activeEnemies++;
                    enemyNumber++;
                    Enemy enemyScript = newEnemy.GetComponent<Enemy>();
                    if (enemyNumber % 3 == 0)
                    {
                        if (enemyScript != null)
                            enemyScript.SetShieldtrue();
                    }
                    if (enemyNumber % 3 == 0)
                    {
                        enemyScript.IsRunningActive();
                    }
                    if (enemyNumber % 4 == 0)
                    {
                        enemyScript.IsRammingActive();
                    }
                }
                yield return new WaitUntil(() => _activeEnemies <= 0);
                _enemiesPerWave++;
                _timeBetweenEnemies = Mathf.Max(0.5f, _timeBetweenEnemies - 0.2f);
                yield return new WaitForSeconds(_timeBetweenWaves);
                if (_waveCount % 2 == 0)
                {
                    SpawnBoss();
                }
                if (_waveCount == 5)
                {
                    if (_alienBossPrefab != null && _bossSpawnPoint != null)
                    {
                        Instantiate(_alienBossPrefab, _alienBossPosition.position, _alienBossPosition.rotation);
                    }
                    yield break;
                }
            }
        }
    }

    private IEnumerator PowerUpSpawnRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (!_stopSpawning)
        {
            int index = GetWeightIndex();
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 7f, 0f);
            int randomIndex = Random.Range(0, _powerUps.Length);
            Instantiate(_powerUps[randomIndex], spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_powerUpSpawnMin, _powerUpSpawnMax));
        }
    }

    private void SpawnBoss()
    {
        if (_bossPrefab != null && _bossSpawnPoint != null)
        {
            Instantiate(_bossPrefab, _bossSpawnPoint.position, _bossSpawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("BossPrefab or BossSpawnPoint not assigned in SpawnManager!");
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void OnBossDestroyed()
    {
        _stopSpawning = true;
        _bossDestroyed = true;
    }

    public void OnEnemyDestroyed()
    {
        _activeEnemies = Mathf.Max(0, _activeEnemies - 1);
    }

    private int GetWeightIndex()
    {
        int total = 0;
        foreach (int w in weights)
            total += w;
        int rand = Random.Range(0, total);
        for (int i = 0; i < weights.Length; i++)
        {
            if (rand < weights[i])
                return i;
            rand -= weights[i];
        }
        return 0;
    }
}
