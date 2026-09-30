using UnityEngine;
using UnityEngine.UI;

public class ScoreSheetUiOption : MonoBehaviour
{
    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI _buttonText = null;

    [SerializeField]
    private Button _button = null;

    private static MaterialManager _sMaterialManager = null;

    private void Awake()
    {
        if(_sMaterialManager == null)
        {
            _sMaterialManager = FindAnyObjectByType<MaterialManager>();
        }
    }

    public void SetEntry(ScoreSheetRow.ScoreSheetRowEntry entry)
    {
        if(_background != null && _sMaterialManager != null)
        {
            _background.material = _sMaterialManager.GetUiMaterial(entry.Color);
        }

        if(_buttonText != null)
        {
            _buttonText.text = entry.Value.ToString();
        }
    }
}
