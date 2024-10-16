using UnityEngine;

public static class Utility {
    public static float AngleTowardsPlayer(Vector3 origin, Vector3 targetPosition) {
        Vector3 direction = targetPosition - origin;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
