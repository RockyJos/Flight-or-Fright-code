using UnityEngine;
using System.Collections;

public class SimpleObstacle : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float despawnX = -10f;

    [Header("Hitbox Settings")]
    public float obstacleHeight = 1.0f;
    public float obstacleWidth = 1.0f;
    public Vector3 hitboxOffset = Vector3.zero;
    public bool showHitbox = true;
    public Color hitboxColor = new Color(1, 0, 0, 0.3f);

    [Header("Hit Detection")]
    public float hitXPosition = 0f;
    public float hitTolerance = 0.5f;

    [Header("Visual Effects")]
    public bool flashOnHit = true;
    public Color hitFlashColor = Color.magenta;
    public bool destroyOnHit = true;
    public float destroyDelay = 0f;

    private bool hasHit = false;
    private bool hasPassedPlayer = false;
    private GameManager gameManager;
    private Renderer obstacleRenderer;
    private Color originalColor;
    private BoxCollider boxCollider;
    private bool isMoving = true;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        obstacleRenderer = GetComponent<Renderer>();
        boxCollider = GetComponent<BoxCollider>();

        if (obstacleRenderer != null)
            originalColor = obstacleRenderer.material.color;

        // Configure collider
        if (boxCollider != null)
        {
            boxCollider.size = new Vector3(obstacleWidth, obstacleHeight, 1f);
            boxCollider.center = hitboxOffset;
            boxCollider.isTrigger = false;
        }

        // Force Z to 0
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;

        isMoving = true;

        Debug.Log($"Obstacle spawned at {transform.position}");
    }

    public void SetGameManager(GameManager gm)
    {
        gameManager = gm;
    }

    void Update()
    {
        if (hasHit || !isMoving) return;

        // Move left
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Check if we're near the player's X position
        float distanceToPlayerX = Mathf.Abs(transform.position.x - hitXPosition);

        if (!hasPassedPlayer && distanceToPlayerX < hitTolerance)
        {
            hasPassedPlayer = true;

            SimplePlayer player = FindObjectOfType<SimplePlayer>();
            if (player != null)
            {
                bool hit = player.CheckObstacleHit(transform.position.y + hitboxOffset.y, obstacleHeight);

                if (hit)
                {
                    RegisterHit();
                }
            }
        }

        // Despawn if off screen
        if (!hasHit && transform.position.x < despawnX)
        {
            Destroy(gameObject);
        }
    }

    void RegisterHit()
    {
        hasHit = true;
        Debug.Log(" OBSTACLE HIT PLAYER!");

        if (gameManager != null)
            gameManager.PlayerHitObstacle();

        if (destroyOnHit)
        {
            if (destroyDelay > 0)
            {
                StartCoroutine(DestroyAfterDelay());
            }
            else
            {
                if (flashOnHit && obstacleRenderer != null)
                    obstacleRenderer.material.color = hitFlashColor;

                if (gameManager != null)
                    gameManager.UnregisterObstacle(gameObject);

                Destroy(gameObject);
            }
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        if (flashOnHit && obstacleRenderer != null)
            obstacleRenderer.material.color = hitFlashColor;

        yield return new WaitForSeconds(destroyDelay);

        if (gameManager != null)
            gameManager.UnregisterObstacle(gameObject);

        Destroy(gameObject);
    }

    public void StopMoving()
    {
        isMoving = false;
        Debug.Log($"Obstacle stopped at {transform.position}");
    }

    void OnDestroy()
    {
        if (gameManager != null && !hasHit)
        {
            gameManager.UnregisterObstacle(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if (!showHitbox) return;

        Vector3 worldCenter = transform.position + new Vector3(0, hitboxOffset.y, hitboxOffset.z);
        Vector3 hitboxSize = new Vector3(obstacleWidth, obstacleHeight, 1f);

        Gizmos.color = hitboxColor;
        Gizmos.DrawCube(worldCenter, hitboxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(worldCenter, hitboxSize);
    }
}