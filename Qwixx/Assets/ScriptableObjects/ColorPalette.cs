using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Scriptable Objects/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    public Color Color0 = Color.white;
    public Color Color1 = Color.red;
    public Color Color2 = Color.yellow;
    public Color Color3 = Color.green;
    public Color Color4 = Color.blue;
    public Color ColorDefault = Color.gray;
}
