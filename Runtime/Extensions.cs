using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public static class EnnerfeltExtensions
{
    //Color Extensions
    public static string GetHexcode(this Color c) {
        string hex = "";
        byte[] bytes = {
            floatToByte(c.r),
            floatToByte(c.g),
            floatToByte(c.b),
            floatToByte(c.a),
        };
        hex = BitConverter.ToString(bytes);
        hex = Regex.Replace(hex, "-", "");
        return $"#{hex}";

        byte floatToByte(float input) {
            byte i = (byte)Mathf.RoundToInt(input * 255);
            return i;
        }
    }
    public static string GetHexcode(this Color32 c) {
        string hex = "";
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
    public static Transform[] GetChildren(this Transform transform) {
        var count = transform.childCount;
        Transform[] children = new Transform[count];
        for (int i = 0; i < count; i++) {
            children[i] = transform.GetChild(i);
        }
        return children;
    }
}