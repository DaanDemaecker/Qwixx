using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorSliderUI : MonoBehaviour
{
    [SerializeField]
    private Image _sliderImage = null;

    [SerializeField]
    private Image _sliderHandleImage = null;

    [SerializeField]
    private Slider _slider = null;

    private Texture2D _gradientTexture = null;

    public float Hue
    {
        get
        {
            return _slider.value;
        }
    }

    public UnityEvent<float> OnHueChangedEvent = new UnityEvent<float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateTexture();

        if (_slider == null)
        {
            Debug.LogError("No slider assigned to ColorSliderUI.");
        }
        _slider.onValueChanged.AddListener(OnUpdateValue);


        if (_sliderHandleImage == null)
        {
            Debug.LogError("Slider Handle Image is not assigned.");
        }

        OnUpdateValue(_slider.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnUpdateValue(float value)
    {
        Color color = Color.HSVToRGB(value, 1, 1);
        _sliderHandleImage.color = color;

        OnHueChangedEvent.Invoke(value);
    }

    private void CreateTexture()
    {
        if(_sliderImage == null)
        {
            Debug.LogError("Slider Image is not assigned.");
            return;
        }

        const float textureHeight = 1024f;
        const float textureWidth = 1f;

        _gradientTexture = new Texture2D((int)textureWidth, (int)textureHeight);
        _gradientTexture.wrapMode = TextureWrapMode.Clamp;
       

        for(int i = 0; i < textureHeight; i++)
        {
            float hue = 1 - (float)i / textureHeight;
            Color color = Color.HSVToRGB(hue, 1, 1);
            

            for(int j = 0; j < textureWidth; j++)
            {
                _gradientTexture.SetPixel(j, i, color);
            }
        }

        _gradientTexture.Apply();

        _sliderImage.sprite = Sprite.Create(_gradientTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
    }
}
