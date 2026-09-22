using System.Collections.Generic;
using UnityEngine;

public class ScoreSheet : MonoBehaviour
{
    [SerializeField]
    private Dictionary<DiceColor, ScoreSheetRow> _rows = new();

    [SerializeField]
    private ScoreSheetOptions _options = null;

    public void SetRoll(DiceRoll roll, bool activePlayer)
    {
        if (_options != null)
        {
            _options.SetWhite(roll.Color0_1 + roll.Color0_2);
            _options.SetColor1(roll.Color0_1 + roll.Color1, roll.Color0_2 + roll.Color1, activePlayer);
        }
    }
}
