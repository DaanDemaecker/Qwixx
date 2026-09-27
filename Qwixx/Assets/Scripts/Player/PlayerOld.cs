using Unity.Netcode;
using UnityEngine;

public class PlayerOld : NetworkBehaviour
{
    private HostOld _gameManager = null;

    [SerializeField]
    private ScoreSheetUi _scoreSheet = null;

    [SerializeField]
    private GameObject _playerUiPrefab = null;

    public override void OnNetworkSpawn()
    {
        _gameManager = FindAnyObjectByType<HostOld>();

        if (_gameManager != null && IsLocalPlayer)
        {
            _gameManager.RegisterPlayer(this);
        }

        if (_playerUiPrefab != null && IsLocalPlayer)
        {
            var playerUi = Instantiate(_playerUiPrefab);
            playerUi.transform.parent = Camera.main.transform;
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
            //_scoreSheet.SetRoll(roll, activePlayerId == OwnerClientId);
        }
    }
}
