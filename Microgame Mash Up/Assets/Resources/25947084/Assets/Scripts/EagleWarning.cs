using UnityEngine;
using System.Collections;

public class EagleWarning : MonoBehaviour
{
    [Header("Warning Settings")]
    public float warningDuration = 1.5f;
    public float targetY;
    public float spawnX = 12f;

    [Header("Line Settings")]
    public float lineWidth = 0.15f;
    public Color warningColor = Color.yellow;

    private LineRenderer lineRenderer;
    private float timer;

    void Start()
    {
        CreateWarningLine();
        timer = warningDuration;

        // Auto-destroy after duration
        Destroy(gameObject, warningDuration);
    }

    void CreateWarningLine()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Bright yellow that pulses
        Color startColor = warningColor;
        startColor.a = 0.8f;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = startColor;

        // Horizontal line from left edge to spawn point
        float leftX = -10f;
        float rightX = spawnX;

        Vector3 startPos = new Vector3(leftX, targetY, 0);
        Vector3 endPos = new Vector3(rightX, targetY, 0);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Start pulsing
        StartCoroutine(PulseLine());
    }

    IEnumerator PulseLine()
    {
        float elapsed = 0f;

        while (lineRenderer != null)
        {
            elapsed += Time.deltaTime * 4f; // Faster pulse

            if (lineRenderer != null)
            {
                // Pulse width
                float width = lineWidth + Mathf.Sin(elapsed) * 0.08f;
                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;

                // Pulse alpha
                float alpha = 0.5f + Mathf.Sin(elapsed) * 0.5f;
                Color newColor = warningColor;
                newColor.a = alpha;
                lineRenderer.startColor = newColor;
                lineRenderer.endColor = newColor;
            }

            yield return null;
        }
    }
}