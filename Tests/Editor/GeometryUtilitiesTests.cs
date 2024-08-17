
using NUnit.Framework;
using UnityEngine;

public class GeometryUtilitiesTests {
    [Test]
    public void GetPointWithinRadius_Vector3_ReturnsPointWithinRadius() {
        // Arrange
        var center = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        var radius = Random.Range(0f, 10f);
        var testIterations = 10000; // Number of times to run the test

        for (var i = 0; i < testIterations; i++) {
            // Act
            var result = GeometryUtilities.GetPointInRadius(center, radius);

            // Assert
            var distance = Vector2.Distance(new Vector2(center.x, center.y), result);
            Assert.LessOrEqual(distance, radius, $"The point is outside the specified radius on iteration {i}.");
        }
    }

    [Test]
    public void GetPointOnRadius_Vector2_ReturnsPointOnRadius() {
        // Arrange
        var testIterations = 10000; // Number of times to run the test

        for (int i = 0; i < testIterations; i++) {
            var center = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            var radius = Random.Range(0f, 10f);

            // Act
            var result = GeometryUtilities.GetPointOnRadius(center, radius);

            // Assert
            var distance = Vector2.Distance(center, result);
            Assert.AreEqual(radius, distance, 0.0001f, $"The point is not exactly on the specified radius on iteration {i}.");
        }
    }

    [Test]
    public void PositionWithinRectangle_ReturnsPointWithinRectangle() {
        // Arrange
        var testIterations = 10000; // Number of times to run the test

        for (var i = 0; i < testIterations; i++) {
            var center = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            var size = new Vector2(Random.Range(-10, 10f), Random.Range(0.1f, 10f));

            // Act
            var result = GeometryUtilities.GetPointInRectangle(center, size);

            // Ensure x and y are at least 0
            size.x = Mathf.Max(size.x, 0f);
            size.y = Mathf.Max(size.y, 0f);

            // Assert
            Assert.GreaterOrEqual(result.x, center.x - size.x / 2, $"The point is outside the rectangle on the left side on iteration {i}.");
            Assert.LessOrEqual(result.x, center.x + size.x / 2, $"The point is outside the rectangle on the right side on iteration {i}.");
            Assert.GreaterOrEqual(result.y, center.y - size.y / 2, $"The point is outside the rectangle on the bottom side on iteration {i}.");
            Assert.LessOrEqual(result.y, center.y + size.y / 2, $"The point is outside the rectangle on the top side on iteration {i}.");
        }
    }

    [Test]
    public void GetPointOnRectangle_ReturnsPointOnEdge() {
        // Arrange
        var center = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        var size = new Vector2(Random.Range(-10, 100f), Random.Range(-10, 100f));
        int testIterations = 1000; // Number of times to run the test

        for (int i = 0; i < testIterations; i++) {
            // Act
            Vector2 result = GeometryUtilities.GetPointOnRectangle(center, size);

            // Assert
            bool isOnEdge =
                     (Mathf.Approximately(result.x, center.x - size.x / 2f) || Mathf.Approximately(result.x, center.x + size.x / 2f)) &&
                     (result.y >= center.y - size.y / 2f && result.y <= center.y + size.y / 2f) ||
                     (Mathf.Approximately(result.y, center.y - size.y / 2f) || Mathf.Approximately(result.y, center.y + size.y / 2f)) &&
                     (result.x >= center.x - size.x / 2f && result.x <= center.x + size.x / 2f);


            Assert.IsTrue(isOnEdge, $"The point is not on the edge of the rectangle on iteration {i}. Result: {result}");
        }
    }
}