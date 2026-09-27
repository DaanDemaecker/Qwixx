using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyDataManager : NetworkBehaviour
{
    private NetworkList<LobbyPlayerData> _lobbyPlayerDatas;

    [SerializeField]
    private List<LobbyPlayerUi> _lobbyPlayerUIs = new();

    [SerializeField]
    private LobbyClientUi _clientUi = null;

    [SerializeField]
    private LobbyHostUi _hostUi = null;

    [SerializeField]
    private int _minPlayers = 2;

    public const string GAME_SCENE_NAME = "GameScene";

    private void Awake()
    {
        _lobbyPlayerDatas = new();
        _lobbyPlayerDatas.OnListChanged += LobbyDataManager_OnListChanged;

        if(_clientUi != null)
        {
            _clientUi.onNameChangedEvent.AddListener(LobbyDataManager_OnNameChanged);
            _clientUi.onColorChangedEvent.AddListener(LobbyDataManager_OnColorChanged);
            _clientUi.onPlayerReadyEvent.AddListener(LobbyDataManager_OnPlayerReady);
        }

        if (_hostUi != null)
        {
            _hostUi.onStartGameClicked.AddListener(LobbyDataManager_OnStartGameClicked);
        }
    }

    private void LobbyDataManager_OnStartGameClicked()
    {
        if(_lobbyPlayerDatas.Count < _minPlayers)
        {
            Debug.LogError("Not enough players");
            return;
        }

        foreach(LobbyPlayerData data in _lobbyPlayerDatas)
        {
            if(!data.IsReady)
            {
                Debug.LogError("Not all players are ready");
                return;
            }
        }

        SceneManager sceneManager = FindAnyObjectByType<SceneManager>();

        if (sceneManager != null)
        {
            PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();

            if (playerManager != null)
            {
                playerManager.StartLoadingGameScene(_lobbyPlayerDatas);
            }

            sceneManager.LoadSceneNetwork(GAME_SCENE_NAME, false);
        }
    }

    private void LobbyDataManager_OnPlayerReady()
    {
        int playerToEditIndex = GetCurrentPlayer();

        if (playerToEditIndex >= 0)
        {
            LobbyPlayerData data = _lobbyPlayerDatas[playerToEditIndex];

            data.IsReady = true;

            ChangePlayerEntryServerRpc(playerToEditIndex, data);
        }
    }

    private void LobbyDataManager_OnColorChanged(Color newColor)
    {
        int playerToEditIndex = GetCurrentPlayer();

        if (playerToEditIndex >= 0)
        {
            LobbyPlayerData data = _lobbyPlayerDatas[playerToEditIndex];

            data.Color = newColor;

            ChangePlayerEntryServerRpc(playerToEditIndex, data);
        }
    }

    private void LobbyDataManager_OnNameChanged(string newName)
    {
        int playerToEditIndex = GetCurrentPlayer();

        if(playerToEditIndex >= 0)
        {
            LobbyPlayerData data = _lobbyPlayerDatas[playerToEditIndex];

            data.Name = newName;

            ChangePlayerEntryServerRpc(playerToEditIndex, data);
        }
    }

    private int GetCurrentPlayer()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;

        for (int i = 0; i < _lobbyPlayerDatas.Count; i++)
        {
            if (_lobbyPlayerDatas[i].ClientId == clientId)
            {
                return i;
            }
        }

        return -1;
    }

    [Rpc(SendTo.Server)]
    private void ChangePlayerEntryServerRpc(int index, LobbyPlayerData data)
    {
        _lobbyPlayerDatas.Set(index, data, true);
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += LobbyDataManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += LobbyDataManager_OnClientDisconnectedCallback;
        }
    }

    private void LobbyDataManager_OnClientDisconnectedCallback(ulong clientId)
    {
        int toRemove = -1;

        for(int i = 0; i < _lobbyPlayerDatas.Count; ++i)
        {
            if (_lobbyPlayerDatas[i].ClientId == clientId)
            {
                toRemove = i;
                break;
            }
        }

        if(toRemove >= 0)
        {
            _lobbyPlayerDatas.RemoveAt(toRemove);
        }
    }

    private void LobbyDataManager_OnListChanged(NetworkListEvent<LobbyPlayerData> changeEvent)
    {
        foreach(LobbyPlayerUi player in _lobbyPlayerUIs)
        {
            if(player == null)
            {
                continue;
            }
            
            if(!SetPlayerData(player))
            { 
                player.SetActive(false);
            }
        }
    }

    private bool SetPlayerData(LobbyPlayerUi player)
    {
        foreach (LobbyPlayerData data in _lobbyPlayerDatas)
        {
            if (player.PlayerNumber == data.PlayerNumber)
            {
                player.SetInfo(data);
                return true;
            }
        }

        return false;
    }



    private void LobbyDataManager_OnClientConnectedCallback(ulong clientId)
    {
        // Don't create a player for the host
        if(clientId == OwnerClientId)
        {
            return;
        }

        int playerNumber = GetNextPlayerNumber();

        LobbyPlayerData playerData = new LobbyPlayerData
        {
            ClientId = clientId,
            PlayerNumber = playerNumber,
            Name = $"Player{playerNumber}",
            Color = Color.white,
            IsReady = false
        };

        _lobbyPlayerDatas.Add(playerData);
    }

    private int GetNextPlayerNumber()
    {
        int nextPlayerNumber = 1;

        while(true)
        {
            if(IsPlayerNumberAvailable(nextPlayerNumber))
            {
                break;
            }
            else
            {
                nextPlayerNumber++;
            }    
        }

        return nextPlayerNumber;
    }

    private bool IsPlayerNumberAvailable(int number)
    {
        foreach (LobbyPlayerData data in _lobbyPlayerDatas)
        {
            if (number == data.PlayerNumber)
            {
                return false;

            }
        }

        return true;
    }

}
