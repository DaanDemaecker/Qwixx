using UnityEngine;

public class ColorPalettePreset : MonoBehaviour
{
    [SerializeField]
    private GameObject _colorImagePrefab = null;



    private void Start()
    {
        if (_colorImagePrefab == null)
        {
            Debug.LogError("No color image prefab was asigned");
        }
    }

    public void SetColorPalette(ColorPalette palette)
    {
        CreateColor(palette.ColorDefault);
        CreateColor(palette.Color4);
        CreateColor(palette.Color3);
        CreateColor(palette.Color2);
        CreateColor(palette.Color1);
    }

    private void CreateColor(Color color)
    {
        GameObject colorImage = Instantiate(_colorImagePrefab, transform);
        ColorPalettePresetImage colorImageScript = colorImage.GetComponent<ColorPalettePresetImage>();
        if (colorImageScript == null)
        {
            Debug.LogError("No ColorPalettePresetImage script was found on the prefab");
            return;
        }
        colorImageScript.SetColor(color);
    }
}
