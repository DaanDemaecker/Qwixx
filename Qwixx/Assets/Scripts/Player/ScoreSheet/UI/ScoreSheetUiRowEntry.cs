using UnityEngine;
using UnityEngine.UI;

public class ScoreSheetUiRowEntry : MonoBehaviour
{
    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI _text = null;

    [SerializeField]
    private GameObject _crossContainer = null;

    [SerializeField]
    private PulsingComponent _pulsing = null;

    [SerializeField]
    private GameObject _textContainer = null;

    [SerializeField]
    private GameObject _lockContainer = null;

    public void SetPulsing(bool isPulsing)
    {
        if(_pulsing != null)
        {
            _pulsing.enabled = isPulsing;
        }
    }

    public void SetEntry(ScoreSheetRow.ScoreSheetRowEntry entry)
    {
        if (_text != null)
        {
            _text.text = entry.Value.ToString();
        }

        if (_background != null)
        {
            _background.material = MaterialManager.Instance.GetUiMaterial(entry.IsLocked ? DiceColor.Color0 : entry.Color);
        }

        if (_crossContainer != null)
        {
            _crossContainer.SetActive(entry.IsCrossed);
        }

        if(_textContainer != null)
        {
            _textContainer.SetActive(!entry.IsLock);
        }

        if (_lockContainer != null)
        {
            _lockContainer.SetActive(entry.IsLock);
        }
    }
}
