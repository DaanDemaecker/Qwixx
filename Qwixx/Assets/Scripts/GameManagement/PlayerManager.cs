using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject _hostPrefab = null;

    [SerializeField]
    private GameObject _playerPrefab = null;

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

                    ScoreSheet scoreSheet = null;
                    if(player.TryGetComponent<ScoreSheet>(out scoreSheet))
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
        OnRollClickedServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission =RpcInvokePermission.Everyone)]
    private void OnRollClickedServerRpc()
    {
        if(_host != null)
        {
            _host.RollDice();
        }
    }
}
