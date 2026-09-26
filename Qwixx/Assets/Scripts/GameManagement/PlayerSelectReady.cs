using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSelectReady : NetworkBehaviour
{
    [SerializeField]
    private int _minPlayers = 1;

    private Dictionary<ulong, bool> _playerReadyStatus = new Dictionary<ulong, bool>();
    public UnityEvent OnPlayerReadyChanged;

    public const string GAME_SCENE_NAME = "GameScene";

    private NetworkList<PlayerInitData> _playerInitDataList;

    public NetworkList<PlayerInitData> PlayerInitDataList
    {
        get
        {
            return _playerInitDataList;
        }
    }

    public UnityEvent OnPlayerDataNetworkListChanged;

    private void Awake()
    {
        _playerInitDataList = new();

        if(IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += PlayerSelectReady_OnClientConnectedCallback;
            _playerInitDataList.OnListChanged += PlayerSelectReady_OnListChanged;
        }
    }

    public override void OnDestroy()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= PlayerSelectReady_OnClientConnectedCallback;
        }
    }

    private void PlayerSelectReady_OnListChanged(NetworkListEvent<PlayerInitData> changeEvent)
    {
        OnPlayerDataNetworkListChanged.Invoke();
    }

    private void PlayerSelectReady_OnClientConnectedCallback(ulong clientId)
    {
        int playerNumber = GetNextPlayerNumber();

        PlayerInitData playerData = new PlayerInitData
        {
            ClientId = clientId,
            PlayerNumber = playerNumber,
            Name = $"Player{playerNumber}",
            Color = Color.white
        };

        _playerInitDataList.Add(playerData);
    }

    private int GetNextPlayerNumber()
    {
        int nextPlayerNumber = 1;
        foreach (var playerData in _playerInitDataList)
        {
            if (playerData.PlayerNumber >= nextPlayerNumber)
            {
                nextPlayerNumber = playerData.PlayerNumber + 1;
            }
        }
        return nextPlayerNumber;
    }

    public void StartGame()
    {
        StartGameServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void StartGameServerRpc()
    {
        if(GetPlayerAmount() < _minPlayers)
        {
            Debug.LogError("Not enough players");
            return;
        }

        if(!AreAllClientsReady())
        {
            Debug.LogError("not all players are ready");
            return;
        }

        SceneManager sceneManager = FindAnyObjectByType<SceneManager>();

        if(sceneManager != null)
        {
            PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();

            if(playerManager != null)
            {
                playerManager.StartLoadingGameScene();
            }

            sceneManager.LoadSceneNetwork(GAME_SCENE_NAME, false);
        }
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetPlayerReadyServerRpc(RpcParams rpcParams =default)
    {
        _playerReadyStatus[rpcParams.Receive.SenderClientId] = true;

        SetPlayerReadyClientRpc(rpcParams.Receive.SenderClientId);

        if(AreAllClientsReady())
        {
            Debug.LogError("All clients are ready");
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        _playerReadyStatus[clientId] = true;
        OnPlayerReadyChanged.Invoke();
    }


    private bool AreAllClientsReady()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (IsServer && clientId == OwnerClientId)
            {
                continue;
            }

            if (!_playerReadyStatus.ContainsKey(clientId) || !_playerReadyStatus[clientId])
            {
                return false;
            }
        }

        return true;
    }

    private int GetPlayerAmount()
    {
        // return 1 less than the amount of clients connected to not count the host
        return NetworkManager.Singleton.ConnectedClients.Count - 1;
    }
}
