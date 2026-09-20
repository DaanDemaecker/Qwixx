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
            _background.color = Color.gray;
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
}
