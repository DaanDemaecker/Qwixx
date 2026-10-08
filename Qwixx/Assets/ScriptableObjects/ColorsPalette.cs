using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Scriptable Objects/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    [Serializable]
    public struct DicColorMaterialPair
    {
        public DiceColor Color;
        public Material Material;
    }

    public List<DicColorMaterialPair> _materialList = new();

    private Dictionary<DiceColor, Material> _materialDictionary = new();

    public Dictionary<DiceColor, Material> MaterialDictionary
    {
        get
        {
            if(_materialDictionary.Count <= 0)
            {
                RebuildDictionaries();
            }

            return _materialDictionary;
        }
    }

    public List<DicColorMaterialPair> _uiMaterialList = new();

    private Dictionary<DiceColor, Material> _uiMmaterialDictionary = new();

    public Dictionary<DiceColor, Material> UiMaterialDictionary
    {
        get
        {
            if (_uiMmaterialDictionary.Count <= 0)
            {
                RebuildDictionaries();
            }

            return _uiMmaterialDictionary;
        }
    }

    private void OnValidate()
    {
        RebuildDictionaries();
    }

    private void RebuildDictionaries()
    {
        _materialDictionary = _materialList.ToDictionary(x => x.Color, x => x.Material);


        _uiMmaterialDictionary = _uiMaterialList.ToDictionary(x => x.Color, x => x.Material);
    }
}
