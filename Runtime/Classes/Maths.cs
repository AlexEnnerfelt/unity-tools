using UnityEngine;

public static class MathUtils
{
    public static Vector3 DirectionFromAngle2D(float angle, float additionalAngle)
    {
        angle += additionalAngle;

        Vector3 direction = Vector3.zero;
        direction.x = Mathf.Cos(angle * Mathf.Deg2Rad);
        direction.y = Mathf.Sin(angle * Mathf.Deg2Rad);
        direction.z = 0f;
        return direction;
    }
    public static float AngleFromDirection2D(Vector2 direction)
    {
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
}