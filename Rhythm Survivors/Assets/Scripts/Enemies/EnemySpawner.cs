using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField]
    public float spawnInterval = 5f;

    [SerializeField]
    private List<GameObject> enemyPrefabs;
    public GameObject firstBoss;
    public GameObject secondBoss;
    public GameObject thirdBoss;

    [Header("Spawn Radius Settings")]
    [SerializeField]
    private float minSpawnRadius = 5f;
    [SerializeField]
    private float maxSpawnRadius = 10f;

    [Header("Boundary Colliders")]
    [SerializeField]
    private BoxCollider2D leftBoundary;
    [SerializeField]
    private BoxCollider2D rightBoundary;
    [SerializeField]
    private BoxCollider2D topBoundary;
    [SerializeField]
    private BoxCollider2D bottomBoundary;

    private float minX, maxX, minY, maxY;

    private int beatCount = 0;
    private BeatDetector beatDetector;
    private Transform playerTransform;
    private float difficultyScale = 2.5f;

    private int waveCount = 0;

    void Start()
    {
        beatDetector = FindObjectOfType<BeatDetector>();
        if (beatDetector != null)
        {
            beatDetector.OnBeatOccurred.AddListener(OnBeatOccurred);
            beatDetector.OnSongTransition.AddListener(OnSongTransition);
        }
        else
        {
            Debug.LogError("BeatDetector not found in the scene.");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }

        InitializeBoundaries();
        StartCoroutine(SpawnEnemiesRoutine());
    }

    void OnDestroy()
    {
        if (beatDetector != null)
        {
            beatDetector.OnBeatOccurred.RemoveListener(OnBeatOccurred);
        }
    }

    private void InitializeBoundaries()
    {
        if (leftBoundary != null)
            minX = leftBoundary.bounds.max.x;
        else
            minX = float.NegativeInfinity;

        if (rightBoundary != null)
            maxX = rightBoundary.bounds.min.x;
        else
            maxX = float.PositiveInfinity;

        if (bottomBoundary != null)
            minY = bottomBoundary.bounds.max.y;
        else
            minY = float.NegativeInfinity;

        if (topBoundary != null)
            maxY = topBoundary.bounds.min.y;
        else
            maxY = float.PositiveInfinity;
    }

    private void OnBeatOccurred()
    {
        beatCount++;
    }

    private void OnSongTransition()
    {
        difficultyScale -= 0.2f;
        if (difficultyScale <= 1.0f)
        {
            difficultyScale = 1.0f;
        }

        waveCount++;

        if (waveCount == 3)
        {
            enemyPrefabs.Add(firstBoss);
        }
        else if (waveCount == 5)
        {
            enemyPrefabs.Add(secondBoss);
        }
        else if (waveCount == 9)
        {
            enemyPrefabs.Add(thirdBoss);
        }
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int extra = 0;

            if (waveCount >= 3)
            {
                extra = 3;
            }
            else if (waveCount >= 5)
            {
                extra = 5;
            }
            else if (waveCount >= 6)
            {
                extra = 15;
            }

            int totalAvailableCost = (int)(beatCount / difficultyScale) + extra;

            beatCount = 0;

            List<GameObject> possibleEnemies = new List<GameObject>();

            // Determine which enemies to add based on the wave count
            int maxIndexToUse = Mathf.Min(waveCount / 2, enemyPrefabs.Count - 1);
            for (int i = 0; i <= maxIndexToUse; i++)
            {
                GameObject enemyPrefab = enemyPrefabs[i];
                Enemy enemyComponent = enemyPrefab.GetComponent<Enemy>();
                if (enemyComponent != null && enemyComponent.cost <= totalAvailableCost)
                {
                    possibleEnemies.Add(enemyPrefab);
                }
            }

            while (totalAvailableCost > 0 && possibleEnemies.Count > 0)
            {
                GameObject selectedEnemyPrefab = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
                Enemy enemyComponent = selectedEnemyPrefab.GetComponent<Enemy>();

                if (enemyComponent != null)
                {
                    int enemyCost = enemyComponent.cost;

                    if (enemyCost <= totalAvailableCost)
                    {
                        Vector2 spawnPosition = GenerateSpawnPosition();
                        Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);

                        totalAvailableCost -= enemyCost;
                    }
                    else
                    {
                        possibleEnemies.Remove(selectedEnemyPrefab);
                    }
                }
                else
                {
                    possibleEnemies.Remove(selectedEnemyPrefab);
                }
            }

            if (waveCount >= 10)
            {
                int numBosses = waveCount - 9;
                for (int i = 0; i < numBosses; i++)
                {
                    Vector2 spawnPosition = GenerateSpawnPosition();
                    Instantiate(firstBoss, spawnPosition, Quaternion.identity);
                }
            }
        }
    }

    private Vector2 GenerateSpawnPosition()
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector2 spawnPosition = (Vector2)playerTransform.position + offset;

            if (spawnPosition.x >= minX && spawnPosition.x <= maxX &&
                spawnPosition.y >= minY && spawnPosition.y <= maxY)
            {
                return spawnPosition;
            }
        }

        Debug.LogWarning("Could not find a valid spawn position within boundaries.");
        return playerTransform.position;
    }

    public void AddEnemyPrefab(GameObject enemyPrefab)
    {
        if (!enemyPrefabs.Contains(enemyPrefab))
        {
            enemyPrefabs.Add(enemyPrefab);
        }
    }
}
