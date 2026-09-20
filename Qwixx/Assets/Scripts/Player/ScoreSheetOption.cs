using UnityEngine;
using UnityEngine.Events;

public class ScoreSheetOption : MonoBehaviour
{
    public UnityEvent<DiceColor, int, bool> ButtonClickedEvent = new UnityEvent<DiceColor, int, bool>();

    [SerializeField]
    private TMPro.TextMeshProUGUI _text = null;

    [SerializeField]
    private DiceColor _color = DiceColor.White;

    [SerializeField]
    private GameObject _crossedOffImage = null;

    public DiceColor Color
    {
        get
        {
            return _color;
        }
    }

    [SerializeField]
    private bool _whiteOption = true;

    private int _value = -1;

    private bool _canBeClicked = true;

    public void SetValue(int value, bool canBeClicked)
    {
        _value = value;

        _canBeClicked = canBeClicked;

        if(_crossedOffImage != null)
        {
            _crossedOffImage.SetActive(!canBeClicked);
        }

        if(_text != null)
        {
            _text.text = value.ToString();
        }
    }

    public void ClickButton()
    {
        if(!_canBeClicked)
        {
            return;
        }

        ButtonClickedEvent.Invoke(_color, _value, _whiteOption);
    }
}
