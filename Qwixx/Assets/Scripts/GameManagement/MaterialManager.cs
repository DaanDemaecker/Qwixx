using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [SerializeField]
    private MaterialCollection _materialCollection = null;



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

        if(_materialCollection == null)
        {
            Debug.LogError("No color palette was given");
        }
    }

    public Material GetMaterial(DiceColor color)
    {
        if(_materialCollection.MaterialDictionary.ContainsKey(color))
        {
            return _materialCollection.MaterialDictionary[color];
        }

        return default;
    }

    public Material GetUiMaterial(DiceColor color)
    {
        if (_materialCollection.UiMaterialDictionary.ContainsKey(color))
        {
            return _materialCollection.UiMaterialDictionary[color];
        }

        return default;
    }
}
