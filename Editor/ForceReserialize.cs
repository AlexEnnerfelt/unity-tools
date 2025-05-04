using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public static class ForceReSerialization
{


    [MenuItem("Assets/Force Reserialize")]
    private static void DoSomethingWithVariable()
    {
        if (NeedsReserialization(Selection.activeObject))
        {
            // Do something with you variable
            AssetDatabase.ForceReserializeAssets(new string[] { AssetDatabase.GetAssetPath(Selection.activeObject) }, ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
            EditorUtility.SetDirty(Selection.activeObject);
            AssetDatabase.SaveAssets();
        }
    }
    // Note that we pass the same path, and also pass "true" to the second argument.
    [MenuItem("Assets/Force Reserialize", true)]
    private static bool NewMenuOptionValidation()
    {
        // This returns true when the selected object is a Variable (the menu item will be disabled otherwise).
        return Selection.activeObject is Object;
    }

    [MenuItem("Tools/Force Reserialize")]
    private static void ReserializeWhereNeeded()
    {
        // Reserialize assets
        ReserializeAssets();

        // Reserialize scenes
        //ReserializeScenes();
    }

    private static void ReserializeAssets()
    {
        // Find all assets of type Object
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();


        foreach (string assetPath in allAssetPaths)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset == null)
            {
                continue;
            }

            if (NeedsReserialization(asset))
            {
                // Reserialize the object and save it
                AssetDatabase.ForceReserializeAssets(new string[] { assetPath }, ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }
        }
    }
    [MenuItem("Tools/Force Reserialize Scenes")]
    private static void ReserializeScenes()
    {
        // Iterate through all scenes
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

            // Find all root game objects in the scene
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject rootObject in rootObjects)
            {
                Component[] components = rootObject.GetComponentsInChildren<Component>(true);
                foreach (Component component in components)
                {
                    if (NeedsReserialization(component))
                    {
                        // Mark the scene as dirty and save it
                        EditorSceneManager.MarkSceneDirty(scene);
                        EditorSceneManager.SaveScene(scene);
                        break;
                    }
                }
            }

            // Unload the scene after processing
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static bool NeedsReserialization(Object obj)
    {
        // Get all components of the object
        var assemblies = new List<string>() {
            "Carnage",
            "Carnage.UI",
            "Carnage.GameOptions",
            "UnpopularOpinion.TopDown",
            "UnpopularOpinion.Tools",
        };
        var fields = new List<FieldInfo>();
        var components = new List<Component>();

        if (obj is GameObject gameObject)
        {
            foreach (var component in gameObject.GetComponents<Component>())
            {
                try
                {
                    if (assemblies.Contains(component.GetType().Assembly.GetName().Name))
                    {
                        fields.AddRange(component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                    }
                }
                catch (System.Exception)
                {
                    continue;
                }
            }
            foreach (var property in fields)
            {
                var attribute = property.GetCustomAttribute<FormerlySerializedAsAttribute>();
                if (attribute != null)
                {
                    Debug.Log($"<color=orange>Reserialized:</color> {AssetDatabase.GetAssetPath(obj)}", obj);
                    return true;
                }
            }
        }
        else if (obj is ScriptableObject scriptableObject)
        {

            if (!assemblies.Contains(scriptableObject.GetType().Assembly.GetName().Name))
            {
                return false;
            }

            fields.AddRange(scriptableObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
            SerializedObject serializedObject = new(scriptableObject);
            foreach (var property in fields)
            {
                var attribute = property.GetCustomAttribute<FormerlySerializedAsAttribute>();

                if (attribute != null)
                {
                    if (true)
                    {
                        Debug.Log($"<color=orange>Reserialized:</color> {AssetDatabase.GetAssetPath(obj)}", obj);
                        return true;
                    }
                }
            }
        }

        return false;
    }

}