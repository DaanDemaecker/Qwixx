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
            Color0_1 = Random.Range(1, 7),
            Color0_2 = Random.Range(1, 7),
            Color1 = Random.Range(1, 7),
            Color2 = Random.Range(1, 7),
            Color3 = Random.Range(1, 7),
            Color4 = Random.Range(1, 7)
        };

        Debug.Log($"White1, {currentRoll.Color0_1}");
        Debug.Log($"White2, {currentRoll.Color0_2}");
        Debug.Log($"Red, {currentRoll.Color1}");
        Debug.Log($"Yellow, {currentRoll.Color2}");
        Debug.Log($"Green, {currentRoll.Color3}");
        Debug.Log($"Blue, {currentRoll.Color4}");


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
