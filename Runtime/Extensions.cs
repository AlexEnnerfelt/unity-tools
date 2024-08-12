using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = System.Random;

public static class Extensions {
    //Color Extensions
    public static string GetHexcode(this Color c) {
        var hex = "";
        byte[] bytes = {
            floatToByte(c.r),
            floatToByte(c.g),
            floatToByte(c.b),
            floatToByte(c.a),
        };
        hex = BitConverter.ToString(bytes);
        hex = Regex.Replace(hex, "-", "");
        return $"#{hex}";

        static byte floatToByte(float input) {
            var i = (byte)Mathf.RoundToInt(input * 255);
            return i;
        }
    }
    public static string GetHexcode(this Color32 c) {
        var hex = "";
        byte[] bytes = {
            c.r,
            c.g,
            c.b,
            c.a,
        };
        hex = BitConverter.ToString(bytes);
        hex = Regex.Replace(hex, "-", "");
        return $"#{hex}";
    }
    public static Color32 FadeAlpha(this Color32 color, float normalizedAlpha) {
        var newAlpha = (byte)(normalizedAlpha * 255);
        return new Color32(color.r, color.g, color.b, newAlpha);
    }
    public static Color FadeAlpha(this Color color, float normalizedAlpha) {
        return new Color(color.r, color.g, color.b, normalizedAlpha);
    }
    public static Transform[] GetChildren(this Transform transform) {
        var count = transform.childCount;
        var children = new Transform[count];
        for (var i = 0; i < count; i++) {
            children[i] = transform.GetChild(i);
        }
        return children;
    }
    public static Transform[] GetAllChildren(this Transform parent) {
        var allChildren = new List<Transform>();
        GetChildRecursive(parent, allChildren);
        return allChildren.ToArray();
    }

    private static void GetChildRecursive(Transform obj, List<Transform> childrenList) {
        if (obj == null)
            return;

        foreach (Transform child in obj) {
            if (child == null)
                continue;

            childrenList.Add(child);
            GetChildRecursive(child, childrenList);
        }
    }

    //Object model extensions
    public static void CopyPropertiesTo<T>(this T source, T dest) {
        var plist = from prop in typeof(T).GetProperties() where prop.CanRead && prop.CanWrite select prop;

        foreach (var prop in plist) {
            prop.SetValue(dest, prop.GetValue(source, null), null);
        }
    }
    public static void CopyPublicProperties<T>(this T source, T destination) {
        var propertyInfos = typeof(T).GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance);

        foreach (var propertyInfo in propertyInfos) {
            if (propertyInfo.CanRead && propertyInfo.CanWrite) {
                var value = propertyInfo.GetValue(source, null);
                propertyInfo.SetValue(destination, value, null);
            }
        }
    }
    public static void CopyPublicFields<T>(this T source, T destination) {
        var fieldInfos = typeof(T).GetFields(
            BindingFlags.Public |
            BindingFlags.Instance);

        foreach (var fieldInfo in fieldInfos) {
            var value = fieldInfo.GetValue(source);
            fieldInfo.SetValue(destination, value);
        }
    }

    //Component assignment extension
    public static bool TryGetComponentInParent<T>(this GameObject obj, out T component) {
        return obj.transform.TryGetComponentInParent(out component);
    }
    public static bool TryGetComponentInParent<T>(this Component obj, out T component) {
        if (obj.TryGetComponent(out component)) {
            return true;
        }
        else {
            component = obj.GetComponentInParent<T>();
        }
        return component != null;
    }
    public static bool TryGetComponentsInParent<T>(this GameObject obj, out T component) {
        return obj.TryGetComponentsInParent(out component);
    }
    public static bool TryGetComponentsInParent<T>(this Component obj, out T component) {
        if (obj.TryGetComponent(out component)) {
            return true;
        }
        else {
            component = obj.GetComponentInParent<T>();
        }
        return component != null;
    }
    public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component) {
        return obj.transform.TryGetComponentInChildren(out component);
    }
    public static bool TryGetComponentInChildren<T>(this Component obj, out T component) {
        if (obj.TryGetComponent(out component)) {
            return true;
        }
        else {
            component = obj.GetComponentInChildren<T>();
        }
        return component != null;
    }
    public static bool TryGetComponentsInChildren<T>(this GameObject obj, out T[] components) {
        return obj.transform.TryGetComponentsInChildren(out components);
    }
    public static bool TryGetComponentsInChildren<T>(this Component obj, out T[] components) {
        components = obj.GetComponentsInChildren<T>();
        if (components == null || components.Length < 1) {
            return false;
        }
        else {
            return true;
        }
    }
    public static bool TryGetComponentAround<T>(this GameObject obj, out T components, bool parentFirst = true) {
        return obj.transform.TryGetComponentAround(out components, parentFirst);
    }
    public static bool TryGetComponentAround<T>(this Component obj, out T components, bool parentFirst = true) {
        components = parentFirst ? obj.GetComponentInParent<T>(true) : obj.GetComponentInChildren<T>(true);
        if (components == null) {
            components = parentFirst ? obj.GetComponentInChildren<T>() : obj.GetComponentInParent<T>();
        }
        return components != null;
    }

    //Layer extensions
    public static bool Contains(this LayerMask mask, int layer) {
        return (mask.value & (1 << layer)) != 0;
    }

    //Vector extensions
    public static Vector3 NormalizedToEuler(this Vector3 normalizedVector) {
        if (normalizedVector.x > 1 || normalizedVector.x < -1 || normalizedVector.y > 1 || normalizedVector.y < -1) {
            Debug.LogError("This vector is not normalized and will return zero");
            return Vector3.zero;
        }
        var angle = Mathf.Atan2(normalizedVector.y, normalizedVector.x) * Mathf.Rad2Deg;
        var eulerAngle = new Vector3(0, 0, angle);
        return eulerAngle;
    }
    public static Vector3 NormalizedToEuler(this Vector2 normalizedVector) {
        if (normalizedVector.x > 1 || normalizedVector.x < -1 || normalizedVector.y > 1 || normalizedVector.y < -1) {
            Debug.LogError("This vector is not normalized and will return zero");
            return Vector2.zero;
        }
        var angle = Mathf.Atan2(normalizedVector.y, normalizedVector.x) * Mathf.Rad2Deg;
        var eulerAngle = new Vector3(0, 0, angle);
        return eulerAngle;
    }

    //Audio extensions
    public static float DecibelToLinear(this float decibel) {
        var linear = Mathf.Pow(10f, decibel / 20f);
        return linear;
    }
}

public static class ListExtensions {

    public static T TakeRandom<T>(this IEnumerable<T> enumerable) {
        if (!enumerable.Any()) {
            throw new InvalidOperationException("Cannot select a random item from an empty set");
        }

        T item;
        do {
            item = enumerable.ElementAt(RandomUtility.globalRandomizer.Next(enumerable.Count()));
        }
        while (item == null);

        return item;
    }
    public static T TakeRandom<T>(this IEnumerable<T> enumerable, Random randomizer) {
        if (!enumerable.Any()) {
            throw new InvalidOperationException("Cannot select a random item from an empty set");
        }

        T item;
        do {
            item = enumerable.ElementAt(randomizer.Next(enumerable.Count()));
        }
        while (item == null);

        return item;
    }
}

public static class Vector2Extensions {
    public static Vector2 GetPointWithinRadius(this Vector2 center, float radius) {
        // Generate a random angle between 0 and 2*PI
        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);

        // Calculate the x and y position using the angle and radius
        float x = center.x + radius * Mathf.Cos(angle);
        float y = center.y + radius * Mathf.Sin(angle);

        return new Vector2(x, y);
    }
    public static Vector2 GetPointWithinRadius(this Vector3 center, float radius) {
        // Generate a random angle between 0 and 2*PI
        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float randomRadius = UnityEngine.Random.Range(0f, radius);

        // Calculate the x and y position using the angle and radius
        float x = center.x + randomRadius * Mathf.Cos(angle);
        float y = center.y + randomRadius * Mathf.Sin(angle);

        return new Vector2(x, y);
    }
}

public static class NavMeshExtensions {
    public static float navMeshArea;

    public static float CalculateNavmeshArea() {
        var navMesh = NavMesh.CalculateTriangulation();
        var mesh = new Mesh();
        mesh.vertices = navMesh.vertices;
        mesh.triangles = navMesh.indices;
        navMeshArea = mesh.CalculateSurfaceArea();
        return navMeshArea;
    }

    public static float CalculateSurfaceArea(this Mesh mesh) {
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;

        double sum = 0.0;

        for (int i = 0; i < triangles.Length; i += 3) {
            Vector3 corner = vertices[triangles[i]];
            Vector3 a = vertices[triangles[i + 1]] - corner;
            Vector3 b = vertices[triangles[i + 2]] - corner;

            sum += Vector3.Cross(a, b).magnitude;
        }

        return (float)(sum / 2.0);
    }
}

public static class LayerMaskExtensions {
    public static LayerMask Inverse(this LayerMask mask) {
        return new LayerMask() {
            value = ~mask.value
        };
    }
}

public static class RandomUtility {
    public static string globalSeed = "borkingBork";
    public static Random globalRandomizer { get; private set; } = new();

    /// <summary>
    /// Initilizes the randomizer with the global seed and resets the number generator
    /// </summary>
    public static void Initialize() {
        globalRandomizer = new(globalSeed.GetHashCode());
    }

    public static float Range(this Random rand, float min, float max) {
        double val = (rand.NextDouble() * (max - min)) + min;
        return (float)val;
    }

    public static float Variance(this Random rand, float baseValue, float variance) {
        var val1 = baseValue - variance;
        var val2 = baseValue + variance;

        return rand.Range(Mathf.Min(val1, val2), Mathf.Max(val1, val2));
    }
    public static float Variance(float baseValue, float variance) {
        return globalRandomizer.Variance(baseValue, variance);
    }

    public static float Range(float min, float max) {
        return globalRandomizer.Range(min, max);
    }
    public static bool RollPercentage(float percentage) {
        var roll = globalRandomizer.Range(0f, 1f);
        return roll <= percentage;
    }

}

public static class ShapeUtilities {
    public static Vector2 RandomPointInEllipse(float width, float height, Vector2 center) {
        var angle = RandomUtility.Range(0f, 2f * Mathf.PI);
        var radius = Mathf.Sqrt(RandomUtility.Range(0f, 1f)) * Mathf.Min(width, height) / 2f;
        var x = radius * Mathf.Cos(angle);
        var y = radius * Mathf.Sin(angle);
        return new Vector2(x, y) + center;
    }
    public static Vector2 RandomPointOnEllipseEdge(float radius, Vector2 center) {
        var angle = RandomUtility.Range(0f, 2f * Mathf.PI);
        //var radius = Mathf.Sqrt(width * width + height * height) / 2f;
        var x = radius * Mathf.Cos(angle);
        var y = radius * Mathf.Sin(angle);
        return new Vector2(x, y) + center;
    }
    public static Vector2 RandomPointInOval(float width, float height, Vector2 center) {
        var angle = RandomUtility.Range(0f, 2f * Mathf.PI);
        var radius = Mathf.Sqrt(RandomUtility.Range(0f, 1f)) * Mathf.Min(width, height) / 2f;
        var x = radius * Mathf.Cos(angle);
        var y = radius * Mathf.Sin(angle);
        x *= width / height;
        return new Vector2(x, y) + center;
    }
    public static Vector2 RandomPointInRectangle(float width, float height, Vector2 center) {
        var angle = RandomUtility.Range(0f, 2f * Mathf.PI);
        var radius = Mathf.Sqrt(RandomUtility.Range(0f, 1f)) * Mathf.Min(width, height) / 2f;
        var x = radius * Mathf.Cos(angle);
        var y = radius * Mathf.Sin(angle);
        x *= width / height;
        return new Vector2(x, y) + center;
    }

    public static Vector2 PositionWithinRectangle(float xExtent, float yExtent) {
        float x = RandomUtility.Range(-xExtent / 2, xExtent / 2); // Adjust the range as needed
        float y = RandomUtility.Range(-yExtent / 2, yExtent / 2); // Adjust the range as needed
        return new Vector2(x, y);
    }
}

public static class SceneUtilities {
    /// <summary>
    /// Returns true if the scene 'name' exists and is in your Build settings, false otherwise
    /// </summary>
    public static bool DoesSceneExist(string name) {
        if (string.IsNullOrEmpty(name))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (string.Compare(name, sceneName, true) == 0)
                return true;
        }

        return false;
    }
}


public interface IRandomized {
    public void SetSeed(int seed);
}