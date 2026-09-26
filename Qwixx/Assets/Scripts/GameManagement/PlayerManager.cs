using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject _hostPrefab = null;

    [SerializeField]
    private GameObject _playerPrefab = null;
    public void StartLoadingGameScene()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        SpawnPlayers();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
    }

    public void SpawnPlayers()
    {
        SpawnPlayerRpc();        
    }

    [Rpc(SendTo.Server)]
    private void SpawnPlayerRpc()
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (OwnerClientId == clientId)
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
                    NetworkObject networkObjectComponent = null;
                    if(player.TryGetComponent<NetworkObject>(out networkObjectComponent))
                    {
                        networkObjectComponent.Spawn(true);
                    }
                }
            }
        }
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
