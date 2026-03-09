using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;
    public GameObject eaglePrefab; // Just the eagle, no warning prefab needed

    [Header("Spawn Settings")]
    public float spawnX = 8f;
    public float minY = -3.5f;
    public float maxY = 3.5f;
    public float despawnX = -12f;

    [Header("Current Settings - Set by Difficulty Preset")]
    public float spawnInterval = 1.5f;
    public float minSpeed = 3f;
    public float maxSpeed = 5f;
    public float minHeight = 0.8f;
    public float maxHeight = 1.2f;
    public float minWidth = 0.8f;
    public float maxWidth = 1.2f;
    public bool randomizeSpeed = true;
    public bool randomizeSize = true;

    [Header("Eagle Settings - Set by Difficulty Preset")]
    public bool enableEagle = false;
    [Range(0f, 1f)]
    public float eagleChance = 0.2f;
    public float eagleSpeed = 15f;

    private float timer;
    private bool isSpawning = false;
    private GameManager gameManager;
    private DifficultySettings currentDifficultySettings;
    private float currentEagleChance;
    private float currentEagleSpeed;

    void Start()
    {
        timer = spawnInterval;
    }

    public void SetGameManager(GameManager gm)
    {
        gameManager = gm;
    }

    public void SetDifficultySettings(DifficultySettings settings)
    {
        if (settings == null) return;

        currentDifficultySettings = settings;

        spawnInterval = settings.spawnInterval;
        minSpeed = settings.minSpeed;
        maxSpeed = settings.maxSpeed;
        minHeight = settings.minHeight;
        maxHeight = settings.maxHeight;
        minWidth = settings.minWidth;
        maxWidth = settings.maxWidth;
        randomizeSpeed = settings.randomizeSpeed;
        randomizeSize = settings.randomizeSize;

        eagleChance = settings.eagleChance;

        enableEagle = (settings.difficulty == GameMaster.Difficulty.NORMAL ||
                      settings.difficulty == GameMaster.Difficulty.HARD ||
                      settings.difficulty == GameMaster.Difficulty.VERY_HARD);

        CalculateEagleSettings();

        Debug.Log($"Spawner configured with {settings.difficultyName} preset");
        Debug.Log($"- Eagles: {(enableEagle ? $"ENABLED ({currentEagleChance * 100:F1}% chance)" : "DISABLED")}");
    }

    void CalculateEagleSettings()
    {
        if (!enableEagle)
        {
            currentEagleChance = 0f;
            currentEagleSpeed = eagleSpeed;
            return;
        }

        currentEagleChance = eagleChance;
        currentEagleSpeed = eagleSpeed;

        if (currentDifficultySettings != null)
        {
            switch (currentDifficultySettings.difficulty)
            {
                case GameMaster.Difficulty.HARD:
                    currentEagleSpeed = eagleSpeed * 1.2f;
                    break;
                case GameMaster.Difficulty.VERY_HARD:
                    currentEagleChance = eagleChance * 1.5f;
                    currentEagleSpeed = eagleSpeed * 1.4f;
                    break;
            }
        }

        currentEagleChance = Mathf.Clamp(currentEagleChance, 0.05f, 0.8f);
    }

    public void StartSpawning()
    {
        isSpawning = true;
        timer = spawnInterval;
        Debug.Log("Spawner started");
    }

    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("Spawner stopped");
    }

    void Update()
    {
        if (!isSpawning) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnObstacle();
            timer = spawnInterval;
        }
    }

    void SpawnObstacle()
    {
        float yPosition = Random.Range(minY, maxY);

        // Check if we should spawn an eagle
        if (enableEagle && eaglePrefab != null && Random.value < currentEagleChance)
        {
            SpawnEagle(yPosition);
        }
        else
        {
            SpawnRegularObstacle(yPosition);
        }
    }

    void SpawnEagle(float yPosition)
    {
        Vector3 spawnPos = new Vector3(spawnX, yPosition, 0);
        GameObject eagle = Instantiate(eaglePrefab, spawnPos, Quaternion.identity);

        if (gameManager != null)
            gameManager.RegisterObstacle(eagle);

        EagleObstacle eagleScript = eagle.GetComponent<EagleObstacle>();
        if (eagleScript != null)
        {
            eagleScript.SetGameManager(gameManager);
            eagleScript.speed = currentEagleSpeed;
            eagleScript.despawnX = despawnX;
            eagleScript.obstacleHeight = Random.Range(minHeight, maxHeight) * 1.2f; // Slightly larger
            eagleScript.obstacleWidth = Random.Range(minWidth, maxWidth) * 1.5f;
        }

        Debug.Log($"EAGLE SPAWNED at Y: {yPosition:F1} with speed {currentEagleSpeed}!");
    }

    void SpawnRegularObstacle(float yPosition)
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogError("No obstacle prefabs assigned!");
            return;
        }

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefabToSpawn = obstaclePrefabs[randomIndex];
        Vector3 spawnPos = new Vector3(spawnX, yPosition, 0);

        GameObject obstacle = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        if (gameManager != null)
            gameManager.RegisterObstacle(obstacle);

        SimpleObstacle obsScript = obstacle.GetComponent<SimpleObstacle>();

        if (obsScript != null)
        {
            obsScript.SetGameManager(gameManager);
            obsScript.speed = randomizeSpeed ?
                Random.Range(minSpeed, maxSpeed) : (minSpeed + maxSpeed) / 2f;
            obsScript.despawnX = despawnX;

            if (randomizeSize)
            {
                obsScript.obstacleHeight = Random.Range(minHeight, maxHeight);
                obsScript.obstacleWidth = Random.Range(minWidth, maxWidth);
            }
        }
    }

    public void ForceSpawnEagle()
    {
        if (!enableEagle || eaglePrefab == null) return;

        float yPosition = Random.Range(minY, maxY);
        SpawnEagle(yPosition);
        Debug.Log("FORCE SPAWNED EAGLE");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(spawnX, 0, 0), 0.5f);
        Gizmos.DrawLine(new Vector3(spawnX, minY, 0), new Vector3(spawnX, maxY, 0));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(despawnX, 0, 0), 0.5f);
        Gizmos.DrawLine(new Vector3(despawnX, minY, 0), new Vector3(despawnX, maxY, 0));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(0, minY, 0), new Vector3(0, maxY, 0));

        // Show eagle chance in editor
        if (enableEagle)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(spawnX, 0, 0), 1f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(new Vector3(spawnX + 1, 2, 0), $"Eagle: {currentEagleChance * 100:F1}%");
#endif
        }
    }
}