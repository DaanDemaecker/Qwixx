using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class OnlinePlayerManager : NetworkBehaviour
{
    private static OnlinePlayerManager _instance = null;

    public static OnlinePlayerManager Instance
    {
        get
        {
            return _instance;
        }
    }



    private Host _hostObject = null;

    private Player _playerObject = null;


    private void Start()
    {
        if (_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

    }


    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnPlayerRpc()
    {
        var players = OnlineLobbyManager.Instance.GetPlayerList();

        foreach (var player in players)
        {
            if (player.Id.Equals(UserManager.Instance.GetId()))
            {
                if (TryGetComponent<OnlinePlayerSpawner>(out OnlinePlayerSpawner playerSpawner))
                {

                    if (UserManager.Instance.IsHostDevice)
                    {
                        _hostObject = playerSpawner.SpawnHost(player);
                        _hostObject.OnRollCompleteEvent.AddListener(OnlinePlayerManager_OnRollComplete);
                    }
                    else
                    {
                        _playerObject = playerSpawner.SpawnPlayer(player);
                    }
                    break;
                }
            }
        }
    }

    private void OnlinePlayerManager_OnRollComplete(DiceRoll.DiceRollData data, FixedString32Bytes activePlayerId)
    {
        OnRollCompleteClientRpc(data, activePlayerId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OnRollCompleteClientRpc(DiceRoll.DiceRollData data, FixedString32Bytes activePlayerId)
    {
        if (_playerObject != null)
        {
            _playerObject.SetRollData(data, activePlayerId.Equals(UserManager.Instance.GetId()));
        }
    }

    public void OnRollClicked(FixedString32Bytes playerId)
    {
        OnRollCLickedRpc(playerId);
    }

    [Rpc(SendTo.Server)]
    private void OnRollCLickedRpc(FixedString32Bytes playerId)
    {
        if(_hostObject != null)
        {
            _hostObject.RollDice(playerId);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartTurnClientRpc(FixedString32Bytes activePlayerId)
    {
        if (_playerObject != null)
        {
            _playerObject.StartTurn(activePlayerId.Equals(UserManager.Instance.GetId()));
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void LockRowClientRpc(int rowIndex)
    {
        if (_playerObject != null)
        {
            _playerObject.LockRow(rowIndex);
        }
    }

    public void EndGame()
    {
        RegisterToScoreManagerClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RegisterToScoreManagerClientRpc()
    {
        if(_playerObject != null)
        {
            OnlineScoreManager.Instance.RegisterScore(UserManager.Instance.GetId(), _playerObject.GetScore());
        }
    }

    public void ResetGame()
    {
        _hostObject = null;
        _playerObject = null;

        if(TryGetComponent<OnlineTurnManager>(out OnlineTurnManager onlineTurnManager))
        {
            onlineTurnManager.ResetGame();
        }
    }
}