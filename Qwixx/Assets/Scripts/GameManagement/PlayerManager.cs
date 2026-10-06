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

    private NetworkList<PlayerData> _playerDatas;

    public NetworkList<PlayerData> PlayerDatas
    {
        get
        {
            return _playerDatas;
        }
    }
    

    private Player _player = null;

    private Host _host = null;

    private const string GAME_OVER_SCENE = "EndGameScene";


    private void Awake()
    {
        _playerDatas = new();
    }
    public void StartLoadingGameScene(NetworkList<PlayerData> playerDatas)
    {
        foreach(PlayerData data in playerDatas)
        {
            _playerDatas.Add(data);
        }


        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnGameSceneLoadEventCompleted;
    }

    private void SceneManager_OnGameSceneLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        foreach (PlayerData data in _playerDatas)
        {
            if (_turnManager != null)
            {
                _turnManager.AddPlayerEntry(data.ClientId);
            }
        }

        SpawnPlayers();

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnGameSceneLoadEventCompleted;
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
                    int playerDataIndex = GetPlayerDataIndex(NetworkManager.Singleton.LocalClientId);

                    if(playerDataIndex < 0 || playerDataIndex >= _playerDatas.Count)
                    {
                        return;
                    }

                    _player.InitializeValues(_playerDatas[playerDataIndex]);

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
        scoreSheet.OnPlayerDiedEvent.AddListener(PlayerManager_OnPlayerDied);
    }

    private void PlayerManager_OnPlayerDied()
    {
        PlayerDiedServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void PlayerDiedServerRpc()
    {
        if(_turnManager != null)
        {
            _turnManager.PlayerDied();
        }
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


    private int GetPlayerDataIndex(ulong clientId)
    {
        for(int i = 0; i < _playerDatas.Count; ++i)
        {
            if (_playerDatas[i].ClientId == clientId)
            {
                return i;
            }
        }

        return -1;
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

    [Rpc(SendTo.ClientsAndHost)]
    public void StartTurnClientRpc( ulong activePlayerId)
    {
        if(_player != null)
        {
            _player.StartTurn(NetworkManager.Singleton.LocalClientId == activePlayerId);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void LockRowClientRpc(int rowIndex)
    {
        if(_player != null)
        {
            _player.LockRow(rowIndex);
        }
    }

    public void EndGame()
    {
        for(int i = 0; i < _playerDatas.Count; i++)
        {
            PlayerData data = _playerDatas[i];

            data.IsReady = false;

            _playerDatas.Set(i, data, true);
        }

        SetPlayerScoreClientRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetPlayerScoreServerRpc(PlayerData data, int index)
    {
        data.IsReady = true;

        _playerDatas.Set(index, data, true);

        if (AllPlayersReady())
        {
            SceneManager sceneManager = FindAnyObjectByType<SceneManager>();

            if (sceneManager != null)
            {
                sceneManager.LoadSceneNetwork(GAME_OVER_SCENE, false);
            }
        }
    }

    private bool AllPlayersReady()
    {
        foreach(PlayerData data in _playerDatas)
        {
            if(!data.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerScoreClientRpc()
    {
        if(_player != null)
        {
            int playerDataIndex = GetPlayerDataIndex(NetworkManager.Singleton.LocalClientId);

            if (playerDataIndex >= 0 || playerDataIndex < _playerDatas.Count)
            {
                PlayerData data = _playerDatas[playerDataIndex];
                data.Score = _player.GetScore();

                SetPlayerScoreServerRpc(data, playerDataIndex);
            }

            Destroy(_player.gameObject);
            _player = null;
        }

        if(_host != null)
        {
            Destroy(_host.gameObject);
            _host = null;
        }
    }
}
