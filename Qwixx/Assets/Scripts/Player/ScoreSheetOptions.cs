using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class ScoreSheetOptions : MonoBehaviour
{
    private struct SelectedOption
    {
        public DiceColor Color;
        public int Value;
    }

    [Serializable]
    private struct ColorRowPair
    {
        public DiceColor Color;
        public ScoreSheetRow Row;
    }

    [SerializeField]
    private List<ColorRowPair> _rowPairs = new();

    private Dictionary<DiceColor, ScoreSheetRow> _rows = new();

    [SerializeField]
    private GameObject _nColoredOptionsObject = null;

    [SerializeField]
    private GameObject _coloredOptionsObject = null;

    [SerializeField]
    private List<ScoreSheetOption> _nColoredOptions = new();


    [Serializable]
    private struct ColorOptionsPair
    {
        public DiceColor Color;
        public ScoreSheetOptionPair Pair;
    }

    [SerializeField]
    private List<ColorOptionsPair> _pairPairs = new();

    [SerializeField]
    private Dictionary<DiceColor, ScoreSheetOptionPair> _pairs = null;


    private SelectedOption _nColorOptionSelected = new SelectedOption { Color = DiceColor.Color0, Value = -1 };
    private SelectedOption _colouredOptionSelected = new SelectedOption { Color = DiceColor.Color0, Value = -1 };

    private void Awake()
    {

        _rows = _rowPairs.ToDictionary(x => x.Color, x => x.Row);

        _pairs = _pairPairs.ToDictionary(x => x.Color, x => x.Pair);

        foreach(var pair in _pairs)
        {
            pair.Value.AddListener(OptionSelected);
        }

        foreach (var option in _nColoredOptions)
        {
            option.ButtonClickedEvent.AddListener(OptionSelected);
        }
    }

    private void OptionSelected(DiceColor color, int value, bool nColoredOption)
    {
        if(nColoredOption)
        {
            if(color == _colouredOptionSelected.Color && value == _colouredOptionSelected.Value)
            {
                return;
            }
        }
        else
        {
            if (color == _nColorOptionSelected.Color && value == _nColorOptionSelected.Value)
            {
                return;
            }
        }

        if (!_rows.ContainsKey(color))
        {
            return;
        }

        if (_rows[color] == null)
        {
            return;
        }

        if (_rows.ContainsKey(color) && _rows[color] != null)


            if (nColoredOption)
            {
                if (_nColorOptionSelected.Value >= 0)
                {
                    _rows[_nColorOptionSelected.Color].Select(_nColorOptionSelected.Value, false);
                }

                _nColorOptionSelected.Color = color;
                _nColorOptionSelected.Value = value;
                _rows[_nColorOptionSelected.Color].Select(_nColorOptionSelected.Value, true);
            }
            else
            {
                if (_colouredOptionSelected.Value >= 0)
                {
                    _rows[_colouredOptionSelected.Color].Select(_colouredOptionSelected.Value, false);
                }

                _colouredOptionSelected.Color = color;
                _colouredOptionSelected.Value = value;
                _rows[_colouredOptionSelected.Color].Select(_colouredOptionSelected.Value, true);
            }
    }

    public void SetWhite(int number)
    {
        foreach(var option in _nColoredOptions)
        {
            if(_rows.ContainsKey(option.Color) && _rows[option.Color] != null)
            {
                option.SetValue(number, _rows[option.Color].CanBeCrossed(number));
            }
            else
            {
                option.SetValue(number, true);
            }
        }
    }

    public void SetColor(DiceColor color, int number1, int number2, bool activePlayer)
    {
        if (_coloredOptionsObject != null)
        {
            _coloredOptionsObject.SetActive(activePlayer);
        }

        if (activePlayer)
        {
            if (!_rows.ContainsKey(color) || _rows[color] == null)
            {
                return;
            }

            if (!_pairs.ContainsKey(color) || _pairs[color] == null)
            {
                return;
            }

            bool canbeCrossed1 = _rows[color].CanBeCrossed(number1);

            bool canbeCrossed2 = _rows[color].CanBeCrossed(number2);

            _pairs[color].SetValues(number1, number2, canbeCrossed1, canbeCrossed2);
        }
    }

    public void OnConfirmChoicesClicked()
    {
        if (_rows.ContainsKey(_nColorOptionSelected.Color) && _rows[_nColorOptionSelected.Color] != null )
        {
            _rows[_nColorOptionSelected.Color].Cross(_nColorOptionSelected.Value);
        }

        if (_rows.ContainsKey(_colouredOptionSelected.Color) && _rows[_colouredOptionSelected.Color] != null)
        {
            _rows[_colouredOptionSelected.Color].Cross(_colouredOptionSelected.Value);
        }

        _nColorOptionSelected.Color = DiceColor.Color0;
        _nColorOptionSelected.Value = -1;

        _colouredOptionSelected.Color = DiceColor.Color0;
        _colouredOptionSelected.Value = -1;
    }
}
