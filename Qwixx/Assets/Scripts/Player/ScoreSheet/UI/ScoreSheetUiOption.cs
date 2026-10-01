using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreSheetUiOption : MonoBehaviour
{
    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI _buttonText = null;

    [SerializeField]
    private Button _button = null;


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

    private static MaterialManager _sMaterialManager = null;

    private ScoreSheetRow.ScoreSheetRowEntry _currentEntry;

    // Events
    public UnityEvent<ScoreSheetRow.ScoreSheetRowEntry, bool> OnOptionClickedEvent;

    private void Awake()
    {
        if(_sMaterialManager == null)
        {
            _sMaterialManager = FindAnyObjectByType<MaterialManager>();
        }

        if(_button != null)
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
    }

    public void SetEntry(ScoreSheetRow.ScoreSheetRowEntry entry)
    {
        _currentEntry = entry;

        if(_background != null && _sMaterialManager != null)
        {
            _background.material = _sMaterialManager.GetUiMaterial(entry.IsLocked ? DiceColor.Color0 : entry.Color);
        }

        if(_buttonText != null)
        {
            _buttonText.text = entry.Value.ToString();
        }

        if (_crossContainer != null)
        {
            _crossContainer.SetActive(entry.IsLocked);
        }
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
