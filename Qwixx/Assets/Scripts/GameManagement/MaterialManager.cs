using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [SerializeField]
    private ColorPalette _colorPalette = null;

    private static MaterialManager _sInstance = null;

    public static MaterialManager Instance
    {
        get
        {
            return _sInstance;
        }
    }

    private void Awake()
    {
        if(_sInstance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
        }

        _sInstance = this;

        if(_colorPalette == null)
        {
            Debug.LogError("No color palette was given");
        }
    }

    public Material GetMaterial(DiceColor color)
    {
        if(_colorPalette.MaterialDictionary.ContainsKey(color))
        {
            return _colorPalette.MaterialDictionary[color];
        }

        return default;
    }

    public Material GetUiMaterial(DiceColor color)
    {
        if (_colorPalette.UiMaterialDictionary.ContainsKey(color))
        {
            return _colorPalette.UiMaterialDictionary[color];
        }

        return default;
    }
}
