using System.Collections.Generic;
using UnityEngine;

public class ColorSettings : MonoBehaviour
{
    [SerializeField]
    private List<ColorPalette> _colorPalettes = new();

    [SerializeField]
    private GameObject _colorPresetPrefab = null;

    [SerializeField]
    private GameObject _colorPresetsParent = null;

    [SerializeField]
    private UiGridScaler _gridScaler = null;

    private void Start()
    {
        if (_colorPresetPrefab == null)
        {
            Debug.LogError("No color preset prefab was asigned");
        }
        if (_colorPalettes.Count == 0)
        {
            Debug.LogError("No color palettes were asigned");
        }

        if(_colorPresetsParent == null)
        {
            Debug.LogError("No color presets parent was asigned");
        }

        if(_gridScaler == null)
        {
            Debug.LogError("No grid scaler was asigned");
        }


        SetupColorPresets();
    }


    private void SetupColorPresets()
    {
        foreach (var palette in _colorPalettes)
        {
            var preset = Instantiate(_colorPresetPrefab, _colorPresetsParent.transform);
            var presetScript = preset.GetComponent<ColorPalettePreset>();
            if (presetScript == null)
            {
                Debug.LogError("Color preset prefab has no ColorPalettePreset script attached");
                return;
            }
            presetScript.SetColorPalette(palette);

            _gridScaler.RowAmount = _colorPresetsParent.transform.childCount;
        }
    }
}
