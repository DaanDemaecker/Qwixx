using System.Collections.Generic;
using UnityEngine;

public class OnlineLobbyUiManager : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _lobbyCodeText = null;

    [SerializeField]
    private Transform _playerContainer = null;

    [SerializeField]
    private GameObject _playerPrefab = null;

    private List<OnlineLobbyPlayerUi> _players = new();

    private float _updateTimer = 0;
    private float _updateTime = 1f;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        if (_lobbyCodeText != null)
        {
            _lobbyCodeText.text = OnlineLobbyManager.Instance.GetLobbyCode();
        }
    }

    private void Update()
    {
        _updateTimer -= Time.deltaTime;

        if(_updateTimer <= 0)
        {
            _updateTimer = _updateTime;
            UpdateUi();
        }
    }

    private void UpdateUi()
    {
        var players = OnlineLobbyManager.Instance.GetPlayerList();

        List<string> usedIds = new();
        List<Unity.Services.Lobbies.Models.Player> newPlayers = new();

        foreach(var player in players)
        {
            string isHostString = player.Data[OnlineLobbyManager.IS_HOST_DEVICE_KEY].Value;
            
            // If player is host, don't create a player
            if (bool.TryParse(isHostString, out bool result) && result)
            {
                continue;
            }

            usedIds.Add(player.Id);

            OnlineLobbyPlayerUi playerUi = GetPlayer(player.Id);

            // If no player is found in existing players that matches, create a new one
            // If it is found, update it
            if(playerUi == null)
            {
                newPlayers.Add(player);
            }
            else
            {
                UpdatePlayer(playerUi, player);
            }
        }

        RemoveUnusedPlayers(usedIds);
        CreateNewPlayers(newPlayers);
    }

    private void RemoveUnusedPlayers(List<string> usedIds)
    {
        List<OnlineLobbyPlayerUi> toRemoveList = new();

        foreach(var player in _players)
        {
            if(!usedIds.Contains(player.PlayerId))
            {
                toRemoveList.Add(player);
            }
        }

        foreach(var toRemove in toRemoveList)
        {
            _players.Remove(toRemove);

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

            if(playerObject.TryGetComponent<OnlineLobbyPlayerUi>(out OnlineLobbyPlayerUi component))
            {
                _players.Add(component);

                component.PlayerId = player.Id;

                UpdatePlayer(component, player);
            }
        }
    }

    private void UpdatePlayer(OnlineLobbyPlayerUi playerUi, Unity.Services.Lobbies.Models.Player player)
    {
        string playerName = string.Empty;

        if (player.Data.ContainsKey(OnlineLobbyManager.PLAYER_NAME_KEY))
        {
            playerName = player.Data[OnlineLobbyManager.PLAYER_NAME_KEY].Value;
        }

        string playerColor = Color.white.ToString();

        if(player.Data.ContainsKey(OnlineLobbyManager.PLAYER_COLOR_KEY))
        {
            playerColor = player.Data[OnlineLobbyManager.PLAYER_COLOR_KEY].Value;
        }

        string playerReady = false.ToString();

        if(player.Data.ContainsKey(OnlineLobbyManager.PLAYER_READY_KEY))
        {
            playerReady = player.Data[OnlineLobbyManager.PLAYER_READY_KEY].Value;
        }

        playerUi.SetData(playerName, playerColor, playerReady);
    }

    private OnlineLobbyPlayerUi GetPlayer(string playerId)
    {
        foreach(var player in _players)
        {
            if(player.PlayerId == playerId)
            {
                return player;
            }
        }

        return null;
    }
}
