using UnityEngine;
using UnityEngine.UI;

public class ScoreSheetUi : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _nameText = null;

    [SerializeField]
    private Image _background = null;



    [SerializeField]
    private ScoreSheetOptions _options = null;

    public void SetRoll(DiceRoll.DiceRollData roll, bool activePlayer)
    {
        if (_options != null)
        {
            _options.SetWhite(roll.Color0_1 + roll.Color0_2);
            _options.SetColor(DiceColor.Color1, roll.Color0_1 + roll.Color1, roll.Color0_2 + roll.Color1, activePlayer);
            _options.SetColor(DiceColor.Color2, roll.Color0_1 + roll.Color2, roll.Color0_2 + roll.Color2, activePlayer);
            _options.SetColor(DiceColor.Color3, roll.Color0_1 + roll.Color3, roll.Color0_2 + roll.Color3, activePlayer);
            _options.SetColor(DiceColor.Color4, roll.Color0_1 + roll.Color4, roll.Color0_2 + roll.Color4, activePlayer);
        }
    }

    public void SetInfo(LobbyPlayerData playerData)
    {
        if (_nameText != null)
        {
            _nameText.text = playerData.Name.ToString();
            _nameText.color = playerData.Color;
        }

        if (_background != null)
        {
            _background.color = playerData.Color;
        }
    }
}