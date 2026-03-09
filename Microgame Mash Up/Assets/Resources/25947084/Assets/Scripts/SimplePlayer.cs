using UnityEngine;
using System.Collections;

public class SimplePlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float smoothTime = 0.1f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Hitbox Settings")]
    public float playerHeight = 1.0f;
    public float playerWidth = 1.0f;
    public Vector3 hitboxOffset = Vector3.zero;
    public bool showHitbox = true;
    public Color hitboxColor = new Color(0, 0, 1, 0.3f);

    [Header("Sprite Settings - Assign These!")]
    public GameObject playerSpriteObject; // The main player sprite with Animator
    public GameObject flashSpriteObject;  // The red flash sprite
    public float flashDuration = 0.1f;

    private float targetY;
    private float currentVelocity = 0f;
    private float currentY;

    private GameManager gameManager;
    private Rigidbody rb; // Using 3D Rigidbody (not 2D)
    private bool canMove = true;
    private float lastHitTime = 0f;
    public float hitCooldown = 0.2f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Get the 3D Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Configure Rigidbody for manual movement
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; // We'll control movement manually
            Debug.Log("Rigidbody configured for player movement");
        }

        // Auto-find sprites if not assigned
        if (playerSpriteObject == null)
            playerSpriteObject = transform.Find("PlayerSprite")?.gameObject;

        if (flashSpriteObject == null)
            flashSpriteObject = transform.Find("FlashSprite")?.gameObject;

        // Make sure player sprite is visible, flash is hidden
        if (playerSpriteObject != null)
            playerSpriteObject.SetActive(true);

        if (flashSpriteObject != null)
            flashSpriteObject.SetActive(false);

        // Log what we found
        Debug.Log($"Player Sprite: {(playerSpriteObject != null ? "Found" : "MISSING")}");
        Debug.Log($"Flash Sprite: {(flashSpriteObject != null ? "Found" : "MISSING")}");

        ResetPlayer();
    }

    public void ResetPlayer()
    {
        // Use Rigidbody for positioning if available
        if (rb != null)
        {
            rb.MovePosition(new Vector3(0f, 0f, 0f));
        }
        else
        {
            transform.position = new Vector3(0f, 0f, 0f);
        }

        targetY = 0f;
        currentY = 0f;
        canMove = true;

        // Show player sprite, hide flash
        if (playerSpriteObject != null)
            playerSpriteObject.SetActive(true);

        if (flashSpriteObject != null)
            flashSpriteObject.SetActive(false);

        transform.localScale = Vector3.one;
    }

    void Update()
    {
        if (!canMove) return;

        // Get mouse scroll input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            targetY += scroll * moveSpeed * 2f;
            targetY = Mathf.Clamp(targetY, minY, maxY);
        }

        // Smooth movement
        currentY = Mathf.SmoothDamp(currentY, targetY, ref currentVelocity, smoothTime);

        // Apply movement using 3D physics
        if (rb != null)
        {
            // Use Rigidbody for movement (better with physics)
            rb.MovePosition(new Vector3(0f, currentY, 0f));
        }
        else
        {
            transform.position = new Vector3(0f, currentY, 0f);
        }
    }

    // Call this when player gets hit
    public void FlashDamage()
    {
        if (playerSpriteObject != null && flashSpriteObject != null)
        {
            StopCoroutine("FlashCoroutine");
            StartCoroutine("FlashCoroutine");
        }
        else
        {
            Debug.LogWarning("Cannot flash - sprite objects not assigned!");
        }
    }

    IEnumerator FlashCoroutine()
    {
        // Switch to flash sprite
        if (playerSpriteObject != null) playerSpriteObject.SetActive(false);
        if (flashSpriteObject != null) flashSpriteObject.SetActive(true);

        // Wait
        yield return new WaitForSeconds(flashDuration);

        // Switch back to player sprite
        if (playerSpriteObject != null) playerSpriteObject.SetActive(true);
        if (flashSpriteObject != null) flashSpriteObject.SetActive(false);
    }

    public bool CheckObstacleHit(float obstacleY, float obstacleHeight)
    {
        if (Time.time - lastHitTime < hitCooldown)
            return false;

        float playerTop = transform.position.y + hitboxOffset.y + (playerHeight / 2f);
        float playerBottom = transform.position.y + hitboxOffset.y - (playerHeight / 2f);

        float obstacleTop = obstacleY + (obstacleHeight / 2f);
        float obstacleBottom = obstacleY - (obstacleHeight / 2f);

        bool yOverlap = playerBottom < obstacleTop && playerTop > obstacleBottom;

        if (yOverlap)
        {
            lastHitTime = Time.time;
            return true;
        }

        return false;
    }

    public void StopMoving()
    {
        canMove = false;
    }

    // 3D Collision detection - matches your Box Collider
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision with: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            FlashDamage();

            if (gameManager != null)
                gameManager.PlayerHitObstacle();
        }
    }

    // Also check for triggers
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger with: {other.gameObject.tag}");

        if (other.CompareTag("Obstacle"))
        {
            FlashDamage();

            if (gameManager != null)
                gameManager.PlayerHitObstacle();
        }
    }

    void OnDrawGizmos()
    {
        if (!showHitbox) return;

        Vector3 worldCenter = transform.position + hitboxOffset;
        Vector3 hitboxSize = new Vector3(playerWidth, playerHeight, 1f);

        Gizmos.color = hitboxColor;
        Gizmos.DrawCube(worldCenter, hitboxSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(worldCenter, hitboxSize);
    }
}