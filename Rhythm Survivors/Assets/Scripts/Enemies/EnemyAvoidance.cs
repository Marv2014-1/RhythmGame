using UnityEngine;

public class EnemyAvoidance : MonoBehaviour
{
    public float avoidanceRadius = 1.0f;
    public float avoidanceForce = 2.0f;
    public float minimumDistance = 0.5f; // Define minimumDistance

    private void FixedUpdate()
    {
        // Define a LayerMask that includes both "Enemy" and "Default" layers by name
        LayerMask avoidanceLayers = LayerMask.GetMask("Enemy", "Default");

        // Check for colliders in the avoidance radius on specified layers
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, avoidanceRadius, avoidanceLayers);

        foreach (Collider obj in nearbyObjects)
        {
            // Avoid self-collision and check distance
            if (obj.gameObject != this.gameObject)
            {
                Vector3 directionAway = transform.position - obj.transform.position;
                float distance = directionAway.magnitude;

                // Only apply avoidance force if within the minimum distance
                if (distance < minimumDistance)
                {
                    Vector3 avoidanceDirection = directionAway.normalized * (avoidanceForce * Time.deltaTime);
                    transform.position += avoidanceDirection;
                }
            }
        }
    }
}