using UnityEngine;

public class ScoreSheet : MonoBehaviour
{
    [SerializeField]
    private ScoreSheetOptions _options = null;

    public void SetRoll(DiceRoll roll, bool activePlayer)
    {
        if (_options != null)
        {
            _options.SetWhite(roll.White1 + roll.White2);
            _options.SetRed(roll.White1 + roll.Red, roll.White2 + roll.Red, activePlayer);
        }
    }
}
