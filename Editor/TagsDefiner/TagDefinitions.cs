using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TagDefinitions : ScriptableObject {
    public const string managerPath = "TagDefinitions";
    public static TagDefinitions _instance = null;
    public static TagDefinitions Instance {
        get {
            if (_instance is null) {
                _instance = Resources.Load<TagDefinitions>(managerPath);
            }
            return _instance;
        }
    }

    public TagDefinition[] definitions;

    [Serializable]
    public class TagDefinition {
        public string category;
        public string[] tags;
    }
}
