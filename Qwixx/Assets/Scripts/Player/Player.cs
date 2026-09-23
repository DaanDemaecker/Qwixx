using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private GameManager _gameManager = null;

    [SerializeField]
    private ScoreSheet _scoreSheet = null;

    public override void OnNetworkSpawn()
    {
        _gameManager = FindAnyObjectByType<GameManager>();

        if (_gameManager != null && IsOwner)
        {
            _gameManager.RegisterPlayer(this);
        }
    }

    public void Roll()
    {
        if(IsOwner)
        {
            RollServerRpc();
        }
    }

    [ServerRpc]
    public void RollServerRpc()
    {
        if (_gameManager != null)
        {
            _gameManager.RollDice(OwnerClientId);
        }
    }

    public void ReceiveRoll(DiceRoll.DiceRollData roll, ulong activePlayerId)
    {
        if(IsOwner && _scoreSheet != null)
        {
            _scoreSheet.SetRoll(roll, activePlayerId == OwnerClientId);
        }
    }
}
