using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyUiManager : NetworkBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _lobbyCodeText = null;

    [SerializeField]
    private Transform _playerContainer = null;

    [SerializeField]
    private GameObject _playerPrefab = null;

    private List<LobbyPlayerUi> _nonInitPlayers = new();
    private List<LobbyPlayerUi> _allPlayers = new();

    private bool _joinCodeSet = false;

    private void Start()
    {
        Setup();

        if(!IsSpawned)
        {
            NetworkObject.Spawn();
        }
    }

    private void OnEnable()
    {
        LobbyManager.Instance.LobbyUpdatedEvent.AddListener(UpdateUi);
    }

    private void OnDisable()
    {
        LobbyManager.Instance.LobbyUpdatedEvent.RemoveListener(UpdateUi);
    }

    private void Setup()
    {
        if (_lobbyCodeText != null)
        {
            _lobbyCodeText.text = LobbyManager.Instance.GetLobbyCode();
        }
        SetupUi();
    }

    private void Update()
    {
        if(!_joinCodeSet)
        {
            if(!string.IsNullOrEmpty(RelayManager.Instance.JoinCode))
            {
                LobbyManager.Instance.UpdateLobbyData(LobbyManager.RELAY_JOIN_CODE_KEY, RelayManager.Instance.JoinCode, Unity.Services.Lobbies.Models.DataObject.VisibilityOptions.Member);
                _joinCodeSet = true;
            }
        }
    }

    private void SetupUi()
    {
        var players = LobbyManager.Instance.GetPlayerList();

        List<Unity.Services.Lobbies.Models.Player> newPlayers = new();

        foreach(var player in players)
        {
            string isHostString = player.Data[LobbyManager.IS_HOST_DEVICE_KEY].Value;
            
            // If player is host, don't create a player
            if (bool.TryParse(isHostString, out bool result) && result)
            {
                continue;
            }

            LobbyPlayerUi playerUi = GetPlayer(player.Id);

            // If no player is found in existing players that matches, create a new one
            if(playerUi == null)
            {
                newPlayers.Add(player);
            }
        }

        CreateNewPlayers(newPlayers);
        UpdateNonInitPlayers();
    }

    private void UpdateUi()
    {
        List<string> usedIds = new();
        var players = LobbyManager.Instance.GetPlayerList();

        List<Unity.Services.Lobbies.Models.Player> newPlayers = new();

        foreach (var player in players)
        {
            string isHostString = player.Data[LobbyManager.IS_HOST_DEVICE_KEY].Value;

            // If player is host, don't create a player
            if (bool.TryParse(isHostString, out bool result) && result)
            {
                continue;
            }

            usedIds.Add(player.Id);

            LobbyPlayerUi playerUi = GetPlayer(player.Id);

            // If no player is found in existing players that matches, create a new one
            if (playerUi == null)
            {
                newPlayers.Add(player);
            }
        }

        RemoveUnusedPlayers(usedIds);
        CreateNewPlayers(newPlayers);
        UpdateNonInitPlayers();
    }

    private void UpdateNonInitPlayers()
    {
        var players = LobbyManager.Instance.GetPlayerList();

        foreach (var player in players)
        {
            string isHostString = player.Data[LobbyManager.IS_HOST_DEVICE_KEY].Value;

            if (bool.TryParse(isHostString, out bool result) && result)
            {
                continue;
            }

            LobbyPlayerUi playerUi = GetNonInitPlayer(player.Id);

            if (playerUi == null)
            {
                continue;
            }
            else
            {
                UpdatePlayer(playerUi, player);
            }
        }
    }

    private void RemoveUnusedPlayers(List<string> usedIds)
    {
        List<LobbyPlayerUi> toRemoveList = new();

        foreach(var player in _allPlayers)
        {
            if(!usedIds.Contains(player.PlayerId))
            {
                toRemoveList.Add(player);
            }
        }

        foreach(var toRemove in toRemoveList)
        {
            _nonInitPlayers.Remove(toRemove);
            _allPlayers.Remove(toRemove);

            Destroy(toRemove.gameObject);
        }
    }

    private void CreateNewPlayers(List<Unity.Services.Lobbies.Models.Player> newPlayers)
    {
        foreach(var player in newPlayers)
        {
            CreatePlayer(player);
        }
    }

    private void CreatePlayer(Unity.Services.Lobbies.Models.Player player)
    {
        if(_playerPrefab != null)
        {
            GameObject playerObject = Instantiate(_playerPrefab, _playerContainer);

            if(playerObject.TryGetComponent<LobbyPlayerUi>(out LobbyPlayerUi component))
            {
                _allPlayers.Add(component);
                _nonInitPlayers.Add(component);

                component.PlayerId = player.Id;
            }
        }
    }

    private void UpdatePlayer(LobbyPlayerUi playerUi, Unity.Services.Lobbies.Models.Player player)
    {
        string playerNameSet = string.Empty;

        if(player.Data.ContainsKey(LobbyManager.PLAYER_NAME_SET_KEY))
        {
            playerNameSet = player.Data[LobbyManager.PLAYER_NAME_SET_KEY].Value;

            if(bool.TryParse(playerNameSet, out bool playerNameSetResult))
            {
                if(!playerNameSetResult)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }


        string playerName = string.Empty;

        if (player.Data.ContainsKey(LobbyManager.PLAYER_NAME_KEY))
        {
            playerName = player.Data[LobbyManager.PLAYER_NAME_KEY].Value;
        }

        string playerColor = Color.white.ToString();

        if(player.Data.ContainsKey(LobbyManager.PLAYER_COLOR_KEY))
        {
            playerColor = player.Data[LobbyManager.PLAYER_COLOR_KEY].Value;
        }

        string playerReady = false.ToString();

        if(player.Data.ContainsKey(LobbyManager.PLAYER_READY_KEY))
        {
            playerReady = player.Data[LobbyManager.PLAYER_READY_KEY].Value;
        }

        playerUi.SetData(playerName, playerColor, playerReady);

        PlayerInitialized(playerUi);
    }

    private LobbyPlayerUi GetNonInitPlayer(string playerId)
    {
        foreach (var player in _nonInitPlayers)
        {
            if (player.PlayerId == playerId)
            {
                return player;
            }
        }

        return null;
    }


    private LobbyPlayerUi GetPlayer(string playerId)
    {
        foreach(var player in _allPlayers)
        {
            if(player.PlayerId == playerId)
            {
                return player;
            }
        } 

        return null;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerNameClientRpc(string id, string newName)
    {
        LobbyPlayerUi player = GetPlayer(id);

        if(player != null)
        {
            player.UpdateName(newName);
            PlayerInitialized(player);
        }    
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerColorClientRpc(string id, Color newColor)
    {
        LobbyPlayerUi player = GetPlayer(id);

        if (player != null)
        {
            player.UpdateColor(newColor);
            PlayerInitialized(player);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerReadyClientRpc(string id, bool ready)
    {
        LobbyPlayerUi player = GetPlayer(id);

        if (player != null)
        {
            player.UpdateReady(ready);
            PlayerInitialized(player);
        }
    }

    private void PlayerInitialized(LobbyPlayerUi player)
    {
        if(_nonInitPlayers.Contains(player))
        {
            _nonInitPlayers.Remove(player);
        }
    }
}
