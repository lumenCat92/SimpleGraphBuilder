using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LumenCat92.SimpleGraphBuilder
{
    public static class ColorConveter
    {
        public static Color HexToColor(string hexCode)
        {
            // Remove the "#" symbol (if present)
            if (hexCode.StartsWith("#"))
            {
                hexCode = hexCode.Substring(1);
            }
            // Separate the color components (assuming RGBA format)
            string redString = hexCode.Substring(0, 2);
            string greenString = hexCode.Substring(2, 2);
            string blueString = hexCode.Substring(4, 2);
            string alphaString = (hexCode.Length == 8) ? hexCode.Substring(6, 2) : "FF"; // Handle alpha if present

            // Convert hex strings to floating-point values
            float red = Convert.ToInt32(redString, 16) / 255.0f;
            float green = Convert.ToInt32(greenString, 16) / 255.0f;
            float blue = Convert.ToInt32(blueString, 16) / 255.0f;
            float alpha = (hexCode.Length == 8) ? Convert.ToInt32(alphaString, 16) / 255.0f : 1.0f;

            // Create the Unity.Color object
            return new Color(red, green, blue, alpha);
        }
    }
}
