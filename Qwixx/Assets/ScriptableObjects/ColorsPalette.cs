using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Scriptable Objects/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    public Color Color0;
    public Color Color1;
    public Color Color2;
    public Color Color3;
    public Color Color4;
    public Color DisabledColor;
}
