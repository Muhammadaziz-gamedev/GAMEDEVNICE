using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyContainer;
    [SerializeField]
    private int enemiesPerWave = 1;
    [SerializeField]
    private float timeBetweenEnemies = 3;
    [SerializeField]
    private float timeBetweenWaves = 6f;
    [SerializeField]
    private GameObject[] powerUps;
    [SerializeField]
    private float powerUpSpawnMin = 5f;
    [SerializeField]
    private float powerUpSpawnMax = 10f;
    [SerializeField]
    private GameObject bossPrefab;
    [SerializeField]
    private Transform bossSpawnPoint;
    [SerializeField] private bool stopSpawning = false;
    [SerializeField] private bool bossDestroyed;
    [SerializeField] private int waveCount = 0;
    private int activeEnemies = 0;
    [SerializeField]
    private int[] weights = { 0, 6 };
    private int enemyNumber = 0;
    [SerializeField]
    private GameObject alienBossPrefab;
    [SerializeField]
    private Transform alienBossPosition;

    public void StartSpawning()
    {
        StartCoroutine(EnemyWaveSequence());
        StartCoroutine(PowerUpSpawnRoutine());
    }

    private IEnumerator EnemyWaveSequence()
    {
        yield return new WaitForSeconds(3f);
        while (!stopSpawning)
        {
            if (waveCount <= 4)
            {
                waveCount++;
                for (int i = 0; i < enemiesPerWave; i++)
                {
                    Vector3 spawnPos = new Vector3(Random.Range(-9f, 9f), 7.70f + Random.Range(-1f, 1f), 0f);
                    GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, enemyContainer.transform);
                    activeEnemies++;
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
                yield return new WaitUntil(() => activeEnemies <= 0);
                enemiesPerWave++;
                timeBetweenEnemies = Mathf.Max(0.5f, timeBetweenEnemies - 0.2f);
                yield return new WaitForSeconds(timeBetweenWaves);
                if (waveCount % 2 == 0)
                {
                    SpawnBoss();
                }
                if (waveCount == 5)
                {
                    if (alienBossPrefab != null && bossSpawnPoint != null)
                    {
                        Instantiate(alienBossPrefab, alienBossPosition.position, alienBossPosition.rotation);
                    }
                    yield break;
                }
            }
        }
    }

    private IEnumerator PowerUpSpawnRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (!stopSpawning)
        {
            int index = GetWeightIndex();
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 7f, 0f);
            int randomIndex = Random.Range(0, powerUps.Length);
            Instantiate(powerUps[randomIndex], spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(powerUpSpawnMin, powerUpSpawnMax));
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("BossPrefab or BossSpawnPoint not assigned in SpawnManager!");
        }
    }

    public void OnPlayerDeath()
    {
        stopSpawning = true;
    }

    public void OnBossDestroyed()
    {
        stopSpawning = true;
        bossDestroyed = true;
    }

    public void OnEnemyDestroyed()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
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
