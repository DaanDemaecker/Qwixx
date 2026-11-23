using UnityEngine;
using UnityEngine.UI;

public class ColorPalettePresetImage : MonoBehaviour
{
    [SerializeField]
    private Image _image = null;

    private void Start()
    {
        if (_image == null)
        {
            Debug.LogError("No image was asigned");
        }
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }
}
