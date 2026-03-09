using UnityEngine;
using System.Collections;

public class EagleObstacle : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    public float despawnX = -12f;

    [Header("Hitbox Settings")]
    public float obstacleHeight = 1.2f;
    public float obstacleWidth = 1.5f;
    public Vector3 hitboxOffset = Vector3.zero;
    public bool showHitbox = true;
    public Color hitboxColor = new Color(1, 0.5f, 0, 0.3f);

    [Header("Visual Effects")]
    public bool flashOnHit = true;
    public Color hitFlashColor = Color.red;
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

        if (boxCollider != null)
        {
            boxCollider.size = new Vector3(obstacleWidth, obstacleHeight, 1f);
            boxCollider.center = hitboxOffset;
            boxCollider.isTrigger = false;
        }

        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;

        isMoving = true;

        Debug.Log($"Eagle spawned at Y: {transform.position.y} with speed: {speed}");
    }

    public void SetGameManager(GameManager gm)
    {
        gameManager = gm;
    }

    void Update()
    {
        if (hasHit || !isMoving) return;

        transform.position += Vector3.left * speed * Time.deltaTime;

        // DESPAWN CHECK - same as regular obstacles
        if (transform.position.x < despawnX)
        {
            if (gameManager != null)
                gameManager.UnregisterObstacle(gameObject);
            Destroy(gameObject);
            return;
        }

        float distanceToPlayerX = Mathf.Abs(transform.position.x - 0f);

        if (!hasPassedPlayer && distanceToPlayerX < 0.5f)
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
    }

    void RegisterHit()
    {
        hasHit = true;
        Debug.Log("EAGLE HIT PLAYER!");

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
        Debug.Log($"Eagle stopped at {transform.position}");
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(worldCenter, hitboxSize);
    }
}