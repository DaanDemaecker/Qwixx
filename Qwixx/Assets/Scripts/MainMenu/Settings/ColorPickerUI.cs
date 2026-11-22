using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerUI : MonoBehaviour
{
    [SerializeField]
    private ColorSliderUI _hueSlider = null;

    [SerializeField]
    private Image _finalColorImage = null;

    private float _hue = 0;
    private float _value = 1;
    private float _saturation = 1;

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
    }

    private void OnEnable()
    {
        _hueSlider.OnHueChangedEvent.AddListener(OnHueChanged);
    }

    private void OnDisable()
    {
        _hueSlider.OnHueChangedEvent.RemoveListener(OnHueChanged);
    }

    private void OnHueChanged(float newHue)
    {
        _hue = newHue;
        SetColor();
    }

    private void SetColor()
    {
        _finalColorImage.color = Color.HSVToRGB(_hue, _saturation, _value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
