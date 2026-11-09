using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ScoreSheetUiOption : MonoBehaviour
{
    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI _buttonText = null;

    [SerializeField]
    private Button _button = null;

    [SerializeField]
    private Image _buttonImage = null;


    [SerializeField]
    private GameObject _crossContainer = null;

    private bool _isNColoredOption = false;

    public bool IsNColoredOption
    {
        set
        {
            _isNColoredOption = value;
        }
    }

    private ScoreSheetRow.ScoreSheetRowEntry _currentEntry;

    // Events
    public UnityEvent<ScoreSheetRow.ScoreSheetRowEntry, bool> OnOptionClickedEvent;

    private void Awake()
    {
        if(_button != null)
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("No button given");
        }

        if(_buttonText == null)
        {
            Debug.LogError("No button text given");
        }

        if (_crossContainer == null)
        {
            Debug.LogError("No cross cointainer given");
        }

        if(_background == null)
        {
            Debug.LogError("No background image given");
        }

        if(_buttonImage == null)
        {
            Debug.LogError("No button image given");
        }
    }

    public void SetEntry(ScoreSheetRow.ScoreSheetRowEntry entry)
    {
        _currentEntry = entry;

        if (_isNColoredOption)
        {
            SetColorsNColored(entry.Color, entry.IsLocked);
        }
        else
        {
            SetColorsColored(entry.Color, entry.IsLocked);
        }

        _buttonText.text = entry.Value.ToString();
        _crossContainer.SetActive(entry.IsLocked);
    }

    private void SetColorsColored(DiceColor color, bool isLocked)
    {
        Material uiMaterialColor = MaterialManager.Instance.GetUiMaterial(isLocked ? DiceColor.Default : color);
        Material uiMaterial0 = MaterialManager.Instance.GetUiMaterial(DiceColor.Color0);

        _background.material = uiMaterialColor;
        _buttonImage.material = uiMaterial0;
        _buttonText.color = uiMaterialColor.color;
    }

    private void SetColorsNColored(DiceColor color, bool isLocked)
    {
        Material uiMaterialColor = MaterialManager.Instance.GetUiMaterial(isLocked ? DiceColor.Default : color);
        Material uiMaterial0 = MaterialManager.Instance.GetUiMaterial(DiceColor.Color0);

        _background.material = uiMaterial0;
        _buttonImage.material = uiMaterialColor;
        _buttonText.color = uiMaterial0.color;
    }

    public void OnButtonClicked()
    {
        if(_currentEntry.IsLocked)
        {
            return;
        }

        OnOptionClickedEvent.Invoke(_currentEntry, _isNColoredOption);
    }
}
