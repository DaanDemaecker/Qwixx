using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyColorButton : MonoBehaviour
{
    [SerializeField]
    private Image _colorImage = null;

    public UnityEvent<Color> onColorSelected;

    public void OnClick()
    {
        if(_colorImage != null)
        {
            onColorSelected.Invoke(_colorImage.color);
        }
    }
}
