using UnityEngine;
using UnityEngine.UI;

public class ScoreSheetEntry : MonoBehaviour
{
    [SerializeField]
    private int _value = 0;

    public int Value
    {
        get
        {
            return _value;
        }
    }

    [SerializeField]
    private PulsingComponent _pulsing = null;

    [SerializeField]
    private GameObject _crossout = null;

    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI _text = null;

    private bool _crossedOff = false;

    private const float DISABLED_ALPHA = 1.0f;
    private const float ENABLED_ALPHA = 0.5f;

    public bool CrossedOff
    {
        get
        {
            return _crossedOff;
        }
    }

    public void Select(bool selected)
    {
        if (_pulsing != null)
        {
            _pulsing.enabled = selected;
        }
    }

    public void SetEnabled(bool value)
    {
        if (_background != null)
        {
            Color color = _background.color;
            if (value)
            {
                color.a = ENABLED_ALPHA;
            }
            else
            {
                color.a = DISABLED_ALPHA;
            }
            _background.color = color;
        }
    }

    public void CrossOff(bool value)
    {
        _crossedOff = value;


        if (_crossout != null)
        {
            _crossout.SetActive(_crossedOff);
        }
    }

    public void OnValidate()
    {
        if(_text != null)
        {
            _text.text = _value.ToString();
        }
    }
}
