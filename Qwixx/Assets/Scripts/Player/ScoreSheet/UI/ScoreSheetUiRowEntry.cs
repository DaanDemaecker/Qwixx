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
    private Image _numberBackgroundImage = null;

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
        Material uiMaterial = MaterialManager.Instance.GetUiMaterial(entry.IsLocked ? DiceColor.Default : entry.Color);
        Material uiMaterial2 = MaterialManager.Instance.GetUiMaterial(DiceColor.Color0);

        if (_text != null)
        {
            _text.text = entry.Value.ToString();
            _text.color = uiMaterial.color;
        }
        
        if(_numberBackgroundImage != null)
        {
            _numberBackgroundImage.material = uiMaterial2;
        }

        if (_background != null)
        {
            _background.material = uiMaterial;
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
