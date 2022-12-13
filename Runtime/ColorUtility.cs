using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public static class ColorUtility
{
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
}