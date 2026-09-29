using UnityEngine;
using UnityEngine.Events;

public class ScoreSheetOptionPairOld : MonoBehaviour
{
    [SerializeField]
    private DiceColor _color = DiceColor.Color0;

    [SerializeField]
    private ScoreSheetOptionOld _option1 = null;

    [SerializeField]
    private ScoreSheetOptionOld _option2 = null;

    private void Awake()
    {
        if(_option1 != null)
        {
            _option1.Color = _color;
        }
        if(_option2 != null)
        {
            _option2.Color = _color;
        }

    }

    public void SetValues(int value1, int value2, bool canBeClicked1, bool canBeClicked2)
    {
        if(_option1 != null)
        {
            _option1.SetValue(value1, canBeClicked1);
        }
        if(_option2 != null)
        {
            _option2.SetValue(value2, canBeClicked2);
        }
    }

    public void AddListener(UnityAction<DiceColor, int, bool> listener)
    {
        if(_option1 != null)
        {
            _option1.ButtonClickedEvent.AddListener(listener);
        }
        if(_option2 != null)
        {
            _option2.ButtonClickedEvent.AddListener(listener);
        }
    }
}
