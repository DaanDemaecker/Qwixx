using UnityEngine;
using UnityEngine.UI;

public class ScoreSheetUiRowEntry : MonoBehaviour
{
    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI _text = null;
    
    
    private static MaterialManager _sMaterialManager = null;

    public void SetEntry(ScoreSheetRow.ScoreSheetRowEntry entry)
    {
        if(_text != null)
        {
            _text.text = entry.Value.ToString();
        }

        if (_background != null)
        {
            MaterialManager materialManager = GetMaterialManager();

            if(materialManager == null)
            {
                return;
            }

            _background.material = materialManager.GetUiMaterial(entry.Color);
        }
    }

    private MaterialManager GetMaterialManager()
    {
        if (_sMaterialManager == null)
        {
            _sMaterialManager = FindAnyObjectByType<MaterialManager>();
        }

        return _sMaterialManager;
    }
}
