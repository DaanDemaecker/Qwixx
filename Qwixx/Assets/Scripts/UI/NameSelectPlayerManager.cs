using System.Collections.Generic;
using UnityEngine;

public class NameSelectPlayerManager : MonoBehaviour
{
    [SerializeField]
    private List<NameSelectPlayer> _nameSelectPlayers = new();

    private void PlayerJoined(int playerNumber)
    {
        SetPlayerActive(playerNumber, true);
    }

    private void PlayerLeft(int playerNumber)
    {
        SetPlayerActive(playerNumber, false);
    }

    private void SetPlayerActive(int playerNumber, bool active)
    {
        if (playerNumber >= 0 && playerNumber < _nameSelectPlayers.Count)
        {
            foreach (var player in _nameSelectPlayers)
            {
                if (player.PlayerNumber == playerNumber)
                {
                    player.gameObject.SetActive(active);
                }
            }
        }
    }    
}
