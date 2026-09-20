using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private Player _player = null;

    public void RegisterPlayer(Player player)
    {
        _player = player;
    }

    public void RollDice(ulong clientId)
    {
        DiceRoll currentRoll = new DiceRoll
        {
            White1 = Random.Range(1, 7),
            White2 = Random.Range(1, 7),
            Red = Random.Range(1, 7),
            Yellow = Random.Range(1, 7),
            Green = Random.Range(1, 7),
            Blue = Random.Range(1, 7)
        };

        Debug.Log($"White1, {currentRoll.White1}");
        Debug.Log($"White2, {currentRoll.White2}");
        Debug.Log($"Red, {currentRoll.Red}");
        Debug.Log($"Yellow, {currentRoll.Yellow}");
        Debug.Log($"Green, {currentRoll.Green}");
        Debug.Log($"Blue, {currentRoll.Blue}");


        RollDiceResultClientRpc(currentRoll, clientId);
    }

    [ClientRpc]
    private void RollDiceResultClientRpc(DiceRoll diceRoll, ulong clientId)
    {
        if(_player != null)
        {
            _player.ReceiveRoll(diceRoll, clientId);
        }
    }
}
