using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [Serializable]
    public struct MaterialData
    {
        public DiceColor Color;
        public Material Material;
    }

    [SerializeField]
    private List<MaterialData> _materialList = new();

    private Dictionary<DiceColor, Material> _materialLookupTable = new();

    [SerializeField]
    private List<MaterialData> _uiMaterialList = new();

    private Dictionary<DiceColor, Material> _uiMaterialLookupTable = new();

    private void Awake()
    {
        _materialLookupTable = _materialList.ToDictionary(x => x.Color, x => x.Material);
        _uiMaterialLookupTable = _uiMaterialList.ToDictionary(x => x.Color, x => x.Material);
    }

    public Material GetMaterial(DiceColor color)
    {
        if(_materialLookupTable.ContainsKey(color))
        {
            return _materialLookupTable[color];
        }

        return default;
    }

    public Material GetUiMaterial(DiceColor color)
    {
        if (_uiMaterialLookupTable.ContainsKey(color))
        {
            return _uiMaterialLookupTable[color];
        }

        return default;
    }
}
