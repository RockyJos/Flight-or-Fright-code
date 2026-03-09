using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    void FixedUpdate()
    {
        // Check for nearby obstacles every physics frame
        Collider[] nearby = Physics.OverlapSphere(transform.position, 2f);

        foreach (Collider col in nearby)
        {
            if (col.gameObject != gameObject && col.CompareTag("Obstacle"))
            {
                Debug.Log($"Obstacle nearby: {col.gameObject.name} at distance {Vector3.Distance(transform.position, col.transform.position)}");

                // Draw line to show it
                Debug.DrawLine(transform.position, col.transform.position, Color.yellow, 0.5f);
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw detection sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}