using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : GAWGameManager
{
    [Header("Game Objects")]
    public GameObject playerPrefab;
    public TMP_Text instructionText;
    public TMP_Text hitsText;

    [Header("Difficulty Settings")]
    public List<DifficultySettings> difficultyPresets;
    private DifficultySettings currentDifficulty;

    [Header("Spawner Reference")]
    public SimpleSpawner spawnerPrefab;

    private GameObject player;
    private GameObject spawnerObject;
    private SimpleSpawner spawnerScript;
    private bool gameStarted = false;
    private bool gameEnded = false;
    private int currentHits = 0;
    private int maxHits = 1;

    private bool playerInvulnerable = false;
    public float invulnerabilityTime = 0.5f;

    private bool hasLoaded = false;

    public float instructionDisplayTime = 2.5f;

    private List<GameObject> activeObstacles = new List<GameObject>();

    public override void OnGameLoad()
    {
        Debug.Log("OnGameLoad called - Loading new level");

        // ALWAYS cleanup previous game first
        CleanupPreviousGame();

        LoadDifficultySettings();

        currentHits = 0;
        gameEnded = false;
        playerInvulnerable = false;
        gameStarted = false;
        hasLoaded = true;

        // Clear obstacle list
        activeObstacles.Clear();

        if (instructionText != null)
        {
            instructionText.text = $"Use Mouse Wheel to Move!\nMax Hits: {maxHits}\nAvoid The Birds!";
            instructionText.color = Color.white;
            instructionText.gameObject.SetActive(true);
        }

        if (hitsText != null)
        {
            hitsText.text = $"Hits: 0/{maxHits}";
            hitsText.color = Color.white;
        }

        // Spawn NEW player
        if (playerPrefab != null)
        {
            Vector3 startPos = new Vector3(0f, 0f, 0f);
            player = Instantiate(playerPrefab, startPos, Quaternion.identity);
            player.transform.localScale = Vector3.one;
            player.tag = "Player";

            Debug.Log("New player spawned for next level");
        }

        // Create NEW spawner
        if (spawnerPrefab != null)
        {
            spawnerObject = Instantiate(spawnerPrefab.gameObject);
            spawnerObject.name = "ObstacleSpawner";
            spawnerScript = spawnerObject.GetComponent<SimpleSpawner>();

            if (spawnerScript != null)
            {
                spawnerScript.SetGameManager(this);

                // Use the difficulty preset we already loaded
                if (currentDifficulty != null)
                {
                    spawnerScript.SetDifficultySettings(currentDifficulty);
                    Debug.Log($"Spawner configured with {currentDifficulty.difficultyName} preset");
                }
            }

            Debug.Log("New spawner created for next level");
        }
    }

    void ClearAllObstacles()
    {
        Debug.Log($"Clearing {activeObstacles.Count} active obstacles");

        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        int destroyedCount = 0;

        foreach (GameObject obstacle in allObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
                destroyedCount++;
            }
        }

        activeObstacles.Clear();
        Debug.Log($"Cleared {destroyedCount} obstacles");
    }

    public void RegisterObstacle(GameObject obstacle)
    {
        if (!activeObstacles.Contains(obstacle))
        {
            activeObstacles.Add(obstacle);
        }
    }

    public void UnregisterObstacle(GameObject obstacle)
    {
        if (activeObstacles.Contains(obstacle))
        {
            activeObstacles.Remove(obstacle);
        }
    }

    void StopAllObstacles()
    {
        foreach (GameObject obstacle in activeObstacles)
        {
            if (obstacle != null)
            {
                SimpleObstacle obsScript = obstacle.GetComponent<SimpleObstacle>();
                if (obsScript != null)
                {
                    obsScript.StopMoving();
                }
            }
        }
    }

    void CleanupPreviousGame()
    {
        Debug.Log("Cleaning up previous game...");

        // Stop spawning first
        if (spawnerScript != null)
        {
            spawnerScript.StopSpawning();
        }

        // Destroy spawner object
        if (spawnerObject != null)
        {
            Destroy(spawnerObject);
            spawnerObject = null;
            spawnerScript = null;
        }

        // Clear all obstacles
        ClearAllObstacles();

        // Destroy player
        if (player != null)
        {
            Destroy(player);
            player = null;
        }

        // Cancel any pending invokes
        CancelInvoke();

        Debug.Log("Cleanup complete");
    }

    void LoadDifficultySettings()
    {
        GameMaster.Difficulty currentGameDifficulty = GameMaster.GetDifficulty();

        foreach (var preset in difficultyPresets)
        {
            if (preset.difficulty == currentGameDifficulty)
            {
                currentDifficulty = preset;
                break;
            }
        }

        if (currentDifficulty == null)
        {
            Debug.LogWarning("No difficulty preset found! Using defaults.");
            currentDifficulty = ScriptableObject.CreateInstance<DifficultySettings>();
            currentDifficulty.maxHits = 1;
            currentDifficulty.minSpeed = 3f;
            currentDifficulty.maxSpeed = 5f;
            currentDifficulty.spawnInterval = 1.5f;
        }

        maxHits = currentDifficulty.maxHits;

        Debug.Log($"Loaded {currentDifficulty.difficultyName} difficulty");
    }

    void UpdateHitsDisplay()
    {
        if (hitsText != null)
        {
            int hitsRemaining = Mathf.Max(0, maxHits - currentHits);
            hitsText.text = $"Hits: {currentHits}/{maxHits}";

            if (hitsRemaining <= 1)
                hitsText.color = Color.red;
            else if (hitsRemaining <= 2)
                hitsText.color = Color.yellow;
            else
                hitsText.color = Color.green;
        }
    }

    public override void OnGameStart()
    {
        Debug.Log("Game Started!");
        gameStarted = true;
        gameEnded = false;
        currentHits = 0;
        playerInvulnerable = false;

        // Reset player
        if (player != null)
        {
            SimplePlayer playerScript = player.GetComponent<SimplePlayer>();
            if (playerScript != null)
            {
                playerScript.ResetPlayer();
            }
        }

        // START THE SPAWNER HERE - ONLY when game actually starts
        if (spawnerScript != null)
        {
            spawnerScript.StartSpawning();
        }

        if (instructionText != null)
        {
            instructionText.text = "";
        }

        UpdateHitsDisplay();
    }

    void Update()
    {
        if (gameStarted && !gameEnded)
        {
            float timeRemaining = GameMaster.GetTimeRemaining();

            if (timeRemaining <= 0f)
            {
                if (currentHits <= maxHits)
                {
                    Debug.Log($"Time reached 0 with {currentHits}/{maxHits} hits - VICTORY!");
                    StopAllObstacles();

                    GameMaster.GameSucceeded();
                    OnGameSucceeded();
                }
                else
                {
                    Debug.Log($"Time reached 0 but player exceeded hits - FAILURE!");
                    StopAllObstacles();

                    OnGameFailed();
                }
            }
        }
    }

    public void PlayerHitObstacle()
    {
        if (!gameStarted || gameEnded || playerInvulnerable)
            return;

        currentHits++;
        Debug.Log($"Player hit! Hits: {currentHits}/{maxHits}");

        playerInvulnerable = true;
        Invoke("ResetInvulnerability", invulnerabilityTime);

        UpdateHitsDisplay();

        if (currentHits > maxHits)
        {
            Debug.Log($"Player exceeded max hits ({currentHits}/{maxHits}) - GAME OVER");
            StopAllObstacles();

            DestroyPlayer();

            OnGameFailed();
        }
    }

    void DestroyPlayer()
    {
        if (player != null)
        {
            StartCoroutine(DestroyPlayerEffect());
        }
    }

    System.Collections.IEnumerator DestroyPlayerEffect()
    {
        if (player == null) yield break;

        Renderer playerRenderer = player.GetComponent<Renderer>();
        Color originalColor = playerRenderer != null ? playerRenderer.material.color : Color.white;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            player.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            if (playerRenderer != null)
            {
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(1f, 0f, t);
                playerRenderer.material.color = newColor;
            }

            yield return null;
        }

        Debug.Log("Player destroyed!");
        Destroy(player);
        player = null;
    }

    void ResetInvulnerability()
    {
        playerInvulnerable = false;
    }

    public override void OnGameSucceeded()
    {
        if (gameEnded) return;

        Debug.Log($"VICTORY! Hits taken: {currentHits}/{maxHits}");
        gameStarted = false;
        gameEnded = true;

        if (instructionText != null)
        {
            instructionText.text = $"Victory!";
            instructionText.color = Color.green;
            instructionText.gameObject.SetActive(true);
        }

        UpdateHitsDisplay();

        if (spawnerScript != null)
            spawnerScript.StopSpawning();

        if (player != null)
        {
            player.GetComponent<SimplePlayer>()?.StopMoving();
        }
    }

    public override void OnGameFailed()
    {
        if (gameEnded) return;

        Debug.Log($"FAILURE! Hits taken: {currentHits}/{maxHits}");
        gameStarted = false;
        gameEnded = true;

        if (instructionText != null)
        {
            instructionText.text = $"Failed!";
            instructionText.color = Color.red;
            instructionText.gameObject.SetActive(true);
        }

        UpdateHitsDisplay();

        if (spawnerScript != null)
            spawnerScript.StopSpawning();
    }

    void OnDestroy()
    {
        CleanupPreviousGame();
    }
}