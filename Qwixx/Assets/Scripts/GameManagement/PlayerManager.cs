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

    private List<Player> _players = new();

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
                Instantiate(_hostPrefab);
            }
        }
        else
        {
            if (_playerPrefab != null)
            {
                GameObject player = Instantiate(_playerPrefab);

                player.transform.SetParent(transform);

                Player playerComponent = null;
                if (player.TryGetComponent<Player>(out playerComponent))
                {
                    playerComponent.InitializeValues(GetPlayerData(NetworkManager.Singleton.LocalClientId));
                }
            }
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

    public void DestroyPlayers()
    {
        DestroyPlayersRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyPlayersRpc()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
