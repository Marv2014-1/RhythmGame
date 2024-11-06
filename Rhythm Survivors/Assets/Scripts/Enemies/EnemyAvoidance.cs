using UnityEngine;

public class EnemyAvoidance : MonoBehaviour
{
    public float avoidanceRadius = 1.0f;
    public float avoidanceForce = 2.0f;

    private void FixedUpdate()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, avoidanceRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider enemy in nearbyEnemies)
        {
            if (enemy.gameObject != this.gameObject)
            {
                Vector3 directionAway = transform.position - enemy.transform.position;
                transform.position += directionAway.normalized * avoidanceForce * Time.deltaTime;
            }
        }
    }
}
