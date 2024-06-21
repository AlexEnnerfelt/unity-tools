using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagDefinedAttribute : PropertyAttribute {
    public string category;
    public TagDefinedAttribute(string category) {
        this.category = category;
    }
}
