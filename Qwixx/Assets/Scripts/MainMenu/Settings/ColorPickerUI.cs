using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorPickerUI : MonoBehaviour
{
    [SerializeField]
    private ColorSliderUI _hueSlider = null;

    [SerializeField]
    private ColorSquareUi _colorSquare = null;

    [SerializeField]
    private Image _finalColorImage = null;

    private float _hue = 0;
    private float _value = 1;
    private float _saturation = 1;

    private Color _currentColor = Color.red;

    public Color CurrentColor
    {
        get
        {
            return _currentColor;
        }
    }

    public UnityEvent<Color> ColorChangedEvent = new UnityEvent<Color>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_hueSlider == null)
        {
            Debug.LogError("No hueslider was set");
        }

        if(_finalColorImage == null)
        {
            Debug.LogError("No final color image was set");
        }

        if (_colorSquare == null)
        {
            Debug.LogError("No color square was set");
        }
    }

    private void OnEnable()
    {
        _hueSlider.OnHueChangedEvent.AddListener(OnHueChanged);

        _colorSquare.OnValueChangedEvent.AddListener(OnValueChanged);
        _colorSquare.OnSaturationChangedEvent.AddListener(OnSaturationChanged);
    }

    private void OnDisable()
    {
        _hueSlider.OnHueChangedEvent.RemoveListener(OnHueChanged);

        _colorSquare.OnValueChangedEvent.RemoveListener(OnValueChanged);
        _colorSquare.OnSaturationChangedEvent.RemoveListener(OnSaturationChanged);
    }

    private void OnHueChanged(float newHue)
    {
        _hue = newHue;
        _colorSquare.SetHue(newHue);
        SetColor();
    }

    private void OnValueChanged(float newValue)
    {
        _value = newValue;
        SetColor();
    }

    private void OnSaturationChanged(float newSaturation)
    {
        _saturation = newSaturation;
        SetColor();
    }

    private void SetColor()
    {
        _currentColor = Color.HSVToRGB(_hue, _saturation, _value);

        _finalColorImage.color = _currentColor;

        ColorChangedEvent.Invoke(_currentColor);
    }
}
