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

    public void Disable()
    {
        if (_background != null)
        {
            Color color = _background.color;
            color.a = 1;
            _background.color = color;
        }
    }

    public void CrossOff()
    {
        if (_crossout != null)
        {
            _crossout.SetActive(true);
        }
        _crossedOff = true;
    }

    public void OnValidate()
    {
        if(_text != null)
        {
            _text.text = _value.ToString();
        }
    }
}
