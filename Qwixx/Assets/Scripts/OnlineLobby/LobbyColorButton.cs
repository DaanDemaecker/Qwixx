using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyColorButton : MonoBehaviour
{
    [SerializeField]
    private Image _colorImage = null;

    public UnityEvent<Color> onColorSelectedEvent;

    public void OnClick()
    {
        if (_colorImage != null)
        {
            onColorSelectedEvent.Invoke(_colorImage.color);
        }
    }

    public void SetColor(Color color)
    {
        if(_colorImage != null)
        {
            _colorImage.color = color;
        }
    }
}
