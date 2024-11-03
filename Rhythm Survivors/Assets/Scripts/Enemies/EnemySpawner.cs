using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField]
    public float spawnInterval = 5f; // Duration to wait before spawning enemies

    [SerializeField]
    private List<GameObject> enemyPrefabs; // List of enemy prefabs to spawn
    public GameObject firstBoss;
    public GameObject secondBoss;
    public GameObject thirdBoss;

    [Header("Spawn Radius Settings")]
    [SerializeField]
    private float minSpawnRadius = 5f; // Minimum distance from the player to spawn
    [SerializeField]
    private float maxSpawnRadius = 10f; // Maximum distance from the player to spawn

    [Header("Boundary Colliders")]
    [SerializeField]
    private BoxCollider2D leftBoundary;
    [SerializeField]
    private BoxCollider2D rightBoundary;
    [SerializeField]
    private BoxCollider2D topBoundary;
    [SerializeField]
    private BoxCollider2D bottomBoundary;

    private float minX, maxX, minY, maxY; // Boundary limits

    private int beatCount = 0; // Counts the number of beats occurred during the interval
    private BeatDetector beatDetector;
    private Transform playerTransform;
    private float difficultyScale = 2.5f;

    void Start()
    {
        // Find the BeatDetector in the scene
        beatDetector = FindObjectOfType<BeatDetector>();
        if (beatDetector != null)
        {
            // Subscribe to the OnBeatOccurred event
            beatDetector.OnBeatOccurred.AddListener(OnBeatOccurred);
            beatDetector.OnSongTransition.AddListener(OnSongTransition);
        }
        else
        {
            Debug.LogError("BeatDetector not found in the scene.");
        }

        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }

        // Initialize boundary limits
        InitializeBoundaries();

        // Start the spawning coroutine
        StartCoroutine(SpawnEnemiesRoutine());
    }

    void OnDestroy()
    {
        // Unsubscribe from the event
        if (beatDetector != null)
        {
            beatDetector.OnBeatOccurred.RemoveListener(OnBeatOccurred);
        }
    }

    /// Initializes the boundary limits based on the BoxCollider2D components.
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

    /// Called whenever a beat occurs.
    private void OnBeatOccurred()
    {
        beatCount++;
    }

    private void OnSongTransition()
    {
        difficultyScale -= 0.2f;
        if (difficultyScale <= 1.0f)
        {
            difficultyScale = 0.5f;
        }
    }

    /// Coroutine that waits for the spawn interval, then spawns enemies based on beat count.
    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            // Wait for the spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Calculate the total available cost based on beat count

            int totalAvailableCost = (int)(beatCount / difficultyScale) + 1; // Divide by dificulty scale to reduce the difficulty

            // Reset beat count
            beatCount = 0;

            // Create a list to hold possible enemies
            List<GameObject> possibleEnemies = new List<GameObject>();

            // Populate possible enemies based on totalAvailableCost
            foreach (GameObject enemyPrefab in enemyPrefabs)
            {
                Enemy enemyComponent = enemyPrefab.GetComponent<Enemy>();
                if (enemyComponent != null && enemyComponent.cost <= totalAvailableCost)
                {
                    possibleEnemies.Add(enemyPrefab);
                }
            }

            // Spawn enemies until we exhaust the totalAvailableCost
            while (totalAvailableCost > 0 && possibleEnemies.Count > 0)
            {
                // Select a random enemy from possible enemies
                GameObject selectedEnemyPrefab = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
                Enemy enemyComponent = selectedEnemyPrefab.GetComponent<Enemy>();

                if (enemyComponent != null)
                {
                    int enemyCost = enemyComponent.cost;

                    if (enemyCost <= totalAvailableCost)
                    {
                        // Generate a spawn position around the player within boundaries
                        Vector2 spawnPosition = GenerateSpawnPosition();

                        // Spawn the enemy at the calculated position
                        Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);

                        // Deduct the enemy cost from totalAvailableCost
                        totalAvailableCost -= enemyCost;
                    }
                    else
                    {
                        // Remove this enemy from possible enemies since it exceeds the remaining cost
                        possibleEnemies.Remove(selectedEnemyPrefab);
                    }
                }
                else
                {
                    // If the enemy prefab doesn't have an Enemy component, remove it
                    possibleEnemies.Remove(selectedEnemyPrefab);
                }
            }
        }
    }

    /// Generates a random spawn position around the player within the boundaries.
    private Vector2 GenerateSpawnPosition()
    {
        int maxAttempts = 10; // To prevent infinite loops
        for (int i = 0; i < maxAttempts; i++)
        {
            // Random angle and radius
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

            // Calculate offset from player position
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector2 spawnPosition = (Vector2)playerTransform.position + offset;

            // Check if the spawnPosition is within boundaries
            if (spawnPosition.x >= minX && spawnPosition.x <= maxX &&
                spawnPosition.y >= minY && spawnPosition.y <= maxY)
            {
                return spawnPosition;
            }
        }

        // If a valid position isn't found, default to player's position (you can handle this differently)
        Debug.LogWarning("Could not find a valid spawn position within boundaries.");
        return playerTransform.position;
    }

    /// Allows you to drag and drop enemy prefabs in the Inspector.
    public void AddEnemyPrefab(GameObject enemyPrefab)
    {
        if (!enemyPrefabs.Contains(enemyPrefab))
        {
            enemyPrefabs.Add(enemyPrefab);
        }
    }
}
