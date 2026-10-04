using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject _hostPrefab = null;

    [SerializeField]
    private GameObject _playerPrefab = null;

    [SerializeField]
    private TurnManager _turnManager = null;

    private NetworkList<LobbyPlayerData> _lobbyPlayerDatas;

    private Player _player = null;

    private Host _host = null;


    private void Awake()
    {
        _lobbyPlayerDatas = new();
    }
    public void StartLoadingGameScene(NetworkList<LobbyPlayerData> playerDatas)
    {
        foreach(LobbyPlayerData data in playerDatas)
        {
            _lobbyPlayerDatas.Add(data);
        }


        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        foreach (LobbyPlayerData data in _lobbyPlayerDatas)
        {
            if (_turnManager != null)
            {
                _turnManager.AddPlayerEntry(data.ClientId);
            }
        }

        SpawnPlayers();

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
    }

    public void SpawnPlayers()
    {
        if(IsHost)
        {
            SpawnPlayerRpc();        
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnPlayerRpc()
    {
        if (IsHost)
        {
            if (_hostPrefab != null)
            {
                GameObject host = Instantiate(_hostPrefab);
                Host hostComponent = null;

                if(host.TryGetComponent<Host>(out hostComponent))
                {
                    hostComponent.SpawnDice();
                    hostComponent.OnRollCompleteEvent.AddListener(PlayerManager_OnRollComplete);

                    _host = hostComponent;
                }
            }
        }
        else
        {
            if (_playerPrefab != null)
            {
                GameObject player = Instantiate(_playerPrefab);

                player.transform.SetParent(transform);

                if (player.TryGetComponent<Player>(out _player))
                {
                    _player.InitializeValues(GetPlayerData(NetworkManager.Singleton.LocalClientId));

                    ScoreSheet scoreSheet = _player.ScoreSheet;
                    if(scoreSheet != null)
                    {
                        SetupCallbacks(scoreSheet);
                    }
                }
            }
        }
    }

    private void SetupCallbacks(ScoreSheet scoreSheet)
    {
        scoreSheet.OnRollClickedEvent.AddListener(PlayerManager_OnRollClicked);
        scoreSheet.OnReadyClickedEvent.AddListener(PlayerManager_OnTurnConfirmed);
        scoreSheet.OnRowLockedEvent.AddListener(PlayerManager_OnRowLocked);
    }

    private void PlayerManager_OnRowLocked(int rowIndex)
    {
        LockRowServerRpc(rowIndex, new RpcParams());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void LockRowServerRpc(int rowIndex, RpcParams rpcParams)
    {
        if(_turnManager != null)
        {
            _turnManager.LockRow(rowIndex);
        }
    }


    private LobbyPlayerData GetPlayerData(ulong clientId)
    {
        foreach(LobbyPlayerData data in _lobbyPlayerDatas)
        {
            if(data.ClientId == clientId)
            {
                return data;
            }
        }

        return new LobbyPlayerData();
    }

    private void PlayerManager_OnRollClicked()
    {
        OnRollClickedServerRpc(new RpcParams());
    }

    private void PlayerManager_OnTurnConfirmed()
    {
        OnTurnConfirmedServerRpc(new RpcParams());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void OnRollClickedServerRpc(RpcParams rpcParams)
    {
        if(_host != null)
        {
            _host.RollDice(rpcParams.Receive.SenderClientId);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void OnTurnConfirmedServerRpc(RpcParams rpcParams)
    {
        if(_turnManager != null)
        {
            _turnManager.TurnConfirmed(rpcParams.Receive.SenderClientId);
        }
    }

    private void PlayerManager_OnRollComplete(DiceRoll.DiceRollData data, ulong activePlayerId)
    {
        OnRollCompleteClientRpc(data, activePlayerId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OnRollCompleteClientRpc(DiceRoll.DiceRollData data, ulong activePlayerId)
    {
        if(_player != null)
        {
            _player.SetRollData(data, activePlayerId == NetworkManager.Singleton.LocalClientId);
        }
    }

    public void StartTurn(ulong activePlayerId)
    {
        StartTurnClientRpc(activePlayerId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartTurnClientRpc( ulong activePlayerId)
    {
        if(_player != null)
        {
            _player.StartTurn(NetworkManager.Singleton.LocalClientId == activePlayerId);
        }
    }

    public void LockRow(int rowIndex)
    {
        LockRowClientRpc(rowIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void LockRowClientRpc(int rowIndex)
    {
        if(_player != null)
        {
            _player.LockRow(rowIndex);
        }
    }
}
