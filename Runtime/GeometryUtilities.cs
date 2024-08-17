using UnityEngine;

public static class GeometryUtilities {
    /// <summary>
    /// Get a random point on the edge of a rectangle defined by the center point and the size
    /// </summary>
    /// <param name="center">center point of the rectangle</param>
    /// <param name="size">Size of the rectangle</param>
    /// <returns>Random point on the rectangle defined</returns>
    public static Vector2 GetPointOnRectangle(Vector2 center, Vector2 size) {
        // Ensure that the size is not negative
        size.x = Mathf.Max(size.x, 0f);
        size.y = Mathf.Max(size.y, 0f);

        // Randomly choose one of the four edges
        int edge = RandomUtility.Range(0, 4);

        float x = 0f, y = 0f;

        switch (edge) {
            case 0: // Top edge
                x = RandomUtility.Range(-size.x / 2f, size.x / 2f);
                y = size.y / 2f;
                break;
            case 1: // Bottom edge
                x = RandomUtility.Range(-size.x / 2f, size.x / 2f);
                y = -size.y / 2f;
                break;
            case 2: // Left edge
                x = -size.x / 2f;
                y = RandomUtility.Range(-size.y / 2f, size.y / 2f);
                break;
            case 3: // Right edge
                x = size.x / 2f;
                y = RandomUtility.Range(-size.y / 2f, size.y / 2f);
                break;
        }

        return center + new Vector2(x, y);
    }

    /// <summary>
    /// Get a random point within a rectangle defined by the center point and the size
    /// </summary>
    /// <param name="center">center point of the rectangle</param>
    /// <param name="size">Size of the rectangle</param>
    /// <returns>Random point within the rectangle defined</returns>
    public static Vector2 GetPointInRectangle(Vector2 center, Vector2 size) {
        // Ensure that the size is not negative
        size.x = Mathf.Max(size.x, 0f);
        size.y = Mathf.Max(size.y, 0f);

        var x = RandomUtility.Range(-size.x / 2f, size.x / 2f);
        var y = RandomUtility.Range(-size.y / 2f, size.y / 2f);

        return center + new Vector2(x, y);
    }
    /// <summary>
    /// Extension for Vector3 will get a random point on the radius from the center of the vector 
    /// </summary>
    /// <param name="center">the input vector</param>
    /// <param name="radius">the radius to allow calculation within</param>
    /// <returns>Random point along the radius around the center point the radius from the center</returns>
    public static Vector2 GetPointOnRadius(Vector2 center, float radius) {
        // Generate a random angle between 0 and 2*PI
        var angle = RandomUtility.Range(0f, 2f * Mathf.PI);

        // Calculate the x and y position using the angle and radius
        var x = center.x + (radius * Mathf.Cos(angle));
        var y = center.y + (radius * Mathf.Sin(angle));

        return new Vector2(x, y);
    }

    /// <summary>
    /// Extension for Vector3 will get a random point within a radius from the center of the vector 
    /// </summary>
    /// <param name="center">the input vector</param>
    /// <param name="radius">the radius to allow calculation within</param>
    /// <returns>Random point within the radius from the center</returns>
    public static Vector2 GetPointInRadius(Vector3 center, float radius) {
        return GetPointInRadius(new Vector2(center.x, center.y), radius);
    }
    public static Vector2 GetPointInRadius(Vector2 center, float radius) {
        // Generate a random angle between 0 and 2*PI
        var angle = RandomUtility.Range(0f, 2f * Mathf.PI);
        var randomRadius = RandomUtility.Range(0f, radius);

        // Calculate the x and y position using the angle and radius
        var x = center.x + (randomRadius * Mathf.Cos(angle));
        var y = center.y + (randomRadius * Mathf.Sin(angle));

        return new Vector2(x, y);
    }
}