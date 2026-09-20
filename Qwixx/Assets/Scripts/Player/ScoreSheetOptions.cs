using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ScoreSheetOptions : MonoBehaviour
{
    private struct SelectedOption
    {
        public DiceColor Color;
        public int Value;
    }

    [SerializeField]
    private ScoreSheetRow _redRow = null;

    [SerializeField]
    private ScoreSheetRow _yellowRow = null;

    [SerializeField]
    private ScoreSheetRow _greenRow = null;

    [SerializeField]
    private ScoreSheetRow _blueRow = null;

    private Dictionary<DiceColor, ScoreSheetRow> _rows = new();


    [SerializeField]
    private List<ScoreSheetOption> _whiteOptions = new();


    [SerializeField]
    private GameObject _coloredOptions = null;


    [SerializeField]
    private ScoreSheetOption _redOption1 = null;

    [SerializeField]
    private ScoreSheetOption _redOption2 = null;


    [SerializeField]
    private ScoreSheetOption _yellowOption1 = null;

    [SerializeField]
    private ScoreSheetOption _yellowOption2 = null;


    [SerializeField]
    private ScoreSheetOption _greenOption1 = null;

    [SerializeField]
    private ScoreSheetOption _greenOption2 = null;


    [SerializeField]
    private ScoreSheetOption _blueOption1 = null;

    [SerializeField]
    private ScoreSheetOption _blueOption2 = null;


    private SelectedOption _whiteOptionSelected = new SelectedOption { Color = DiceColor.White, Value = -1 };
    private SelectedOption _colouredOptionSelected = new SelectedOption { Color = DiceColor.White, Value = -1 };

    private void Awake()
    {
        _rows[DiceColor.Red] = _redRow;
        _rows[DiceColor.Yellow] = _yellowRow;
        _rows[DiceColor.Green] = _greenRow;
        _rows[DiceColor.Blue] = _blueRow;


        foreach (var option in _whiteOptions)
        {
            option.ButtonClickedEvent.AddListener(OptionSelected);
        }

        _redOption1.ButtonClickedEvent.AddListener(OptionSelected);
        _redOption2.ButtonClickedEvent.AddListener(OptionSelected);
    }

    private void OptionSelected(DiceColor color, int value, bool whiteOption)
    {
        if(whiteOption)
        {
            if(color == _colouredOptionSelected.Color && value == _colouredOptionSelected.Value)
            {
                return;
            }
        }
        else
        {
            if (color == _whiteOptionSelected.Color && value == _whiteOptionSelected.Value)
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


            if (whiteOption)
            {
                if (_whiteOptionSelected.Value >= 0)
                {
                    _rows[_whiteOptionSelected.Color].Select(_whiteOptionSelected.Value, false);
                }

                _whiteOptionSelected.Color = color;
                _whiteOptionSelected.Value = value;
                _rows[_whiteOptionSelected.Color].Select(_whiteOptionSelected.Value, true);
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
        foreach(var option in _whiteOptions)
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

    public void SetRed(int number1, int number2, bool activePlayer)
    {
        if(_coloredOptions != null)
        {
            _coloredOptions.SetActive(activePlayer);
        }

        if (activePlayer)
        {
            if(_redRow == null)
            {
                return;
            }

            if (_redOption1 != null)
            {
                _redOption1.SetValue(number1, _redRow.CanBeCrossed(number1));
            }

            if (_redOption2 != null)
            {
                _redOption2.SetValue(number2, _redRow.CanBeCrossed(number2));
            }
        }
    }

    public void OnConfirmChoicesClicked()
    {
        if (_rows.ContainsKey(_whiteOptionSelected.Color) && _rows[_whiteOptionSelected.Color] != null )
        {
            _rows[_whiteOptionSelected.Color].Cross(_whiteOptionSelected.Value);
        }

        if (_rows.ContainsKey(_colouredOptionSelected.Color) && _rows[_colouredOptionSelected.Color] != null)
        {
            _rows[_colouredOptionSelected.Color].Cross(_colouredOptionSelected.Value);
        }

        _whiteOptionSelected.Color = DiceColor.White;
        _whiteOptionSelected.Value = -1;

        _colouredOptionSelected.Color = DiceColor.White;
        _colouredOptionSelected.Value = -1;
    }
}
