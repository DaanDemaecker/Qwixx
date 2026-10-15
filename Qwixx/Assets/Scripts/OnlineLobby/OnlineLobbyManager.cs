using IngameDebugConsole;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;

public class OnlineLobbyManager : NetworkBehaviour
{
    private static OnlineLobbyManager _instance = null;

    public static OnlineLobbyManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private Lobby _lobby = null;

    private bool _isHostDevice = false;

    public bool IsHostDevice
    {
        get
        {
            return _isHostDevice;
        }
    }


    private bool _tryingToQuit = false;

    private float _heartbeatTimer = 0f;

    private float _heartbeatTime = 20f;



    private float _updateLobbyTimer = 0f;

    private float _updateLobbyTime = 2f;


    public const string PLAYER_NUMBER_KEY = "PlayerNumber";
    public const string IS_HOST_DEVICE_KEY = "IsHostDevice";
    public const string PLAYER_NAME_KEY = "PlayerName";
    public const string PLAYER_COLOR_KEY = "PlayerColor";
    public const string PLAYER_READY_KEY = "PlayerReady";


    private const string HOST_NAME = "Host";

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
        }
        _instance = this;
    }

    private void OnEnable()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    private void OnDisable()
    {
        Application.wantsToQuit -= WantsToQuit;
    }

    private bool WantsToQuit()
    {
        if(_lobby == null)
        {
            return true;
        }

        _ = LeaveLobby();

        _tryingToQuit = true;

        return false;
    }

    public async Task<bool> CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = GameRuleManager.Instance.MaxPlayers + 1;

            var player = GetPlayer(true);

            player.Data.Add(PLAYER_NUMBER_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "0"));
            player.Data.Add(PLAYER_NAME_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, HOST_NAME));

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = player
            };

            _lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            _isHostDevice = true;

            return true;
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        return false;
    }

    public string GetLobbyCode()
    {
        if(_lobby == null)
        {
            return string.Empty;
        }

        return _lobby.LobbyCode;
    }

    public async Task<bool> JoinLobby(string code)
    {
        try
        {
            JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer(false)
            };

            _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinOptions);

            UpdatePlayerData(PLAYER_NUMBER_KEY, GetNextPlayerNumber().ToString());

            Debug.Log("Joined lobby");

            _isHostDevice = false;

            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }


        return false;
    }

    public void Update()
    {
        HandleQuit();
        HandleHeartbeat();
        HandleLobbyUpdate();
    }

    private void HandleQuit()
    {
        if(_tryingToQuit && _lobby == null)
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }

    private async void HandleLobbyUpdate()
    {
        try
        {
            if (_lobby != null)
            {
                _updateLobbyTimer -= Time.deltaTime;

                if (_updateLobbyTimer <= 0)
                {
                    _updateLobbyTimer = _updateLobbyTime;

                    _lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
                }
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void HandleHeartbeat()
    {
        try
        {
            if (_isHostDevice && _lobby != null)
            {
                _heartbeatTimer -= Time.deltaTime;

                if (_heartbeatTimer <= 0)
                {
                    _heartbeatTimer = _heartbeatTime;

                    await LobbyService.Instance.SendHeartbeatPingAsync(_lobby.Id);
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private Unity.Services.Lobbies.Models.Player GetPlayer(bool isHost)
    {
        Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {IS_HOST_DEVICE_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isHost.ToString()) }
            }
        };

        return player;
    }


#if DEBUG
    [ConsoleMethod("PrintPlayers", "Print a list of all connected players")]
    public static void PrintPlayers()
    {
        _instance.PrintPlayers(_instance._lobby);
    }
#endif

    private void PrintPlayers(Lobby lobby)
    {
        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            Debug.Log($"{player.Data[PLAYER_NAME_KEY].Value}   {player.Data[IS_HOST_DEVICE_KEY].Value}     {player.Data[PLAYER_NUMBER_KEY].Value}");
        }
    }

#if DEBUG
    [ConsoleMethod("LeaveLobbyNow", "Leave the lobby immediately")]
    public static async Task LeaveLobbyNow()
    {
        await _instance.LeaveLobby();
    }
#endif

    public async Task LeaveLobby()
    {
        try
        {
            if (_lobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, UserManager.Instance.GetId());
                _lobby = null;
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void UpdateLobbyData(string key, string value, DataObject.VisibilityOptions visibility = DataObject.VisibilityOptions.Public)
    {
        if (_lobby == null)
        {
            return;
        }

        try
        {

            await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
            {
                {key, new DataObject(visibility, value) }
            }
            });

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void UpdatePlayerData(string key, string value, PlayerDataObject.VisibilityOptions visitbility = PlayerDataObject.VisibilityOptions.Public)
    {
        if(_lobby == null)
        {
            return;
        }

        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, UserManager.Instance.GetId(), new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
            {
                {key, new PlayerDataObject(visitbility, value) }
            }
            });

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public string GetLobbyData(string key)
    {
        if(_lobby == null || !_lobby.Data.ContainsKey(key))
        {
            return string.Empty;
        }

        return _lobby.Data[key].Value;
    }

    public List<Unity.Services.Lobbies.Models.Player> GetPlayerList()
    {
        if(_lobby == null)
        {
            return new();
        }

        return _lobby.Players;
    }

    public string GetPlayerData(string key)
    {
        if(_lobby == null)
        {
            return string.Empty;
        }

        foreach(Unity.Services.Lobbies.Models.Player player in _lobby.Players)
        {
            if(player.Id == UserManager.Instance.GetId())
            {
                if(player.Data.ContainsKey(key))
                {
                    return player.Data[key].Value;
                }
            }
        }
        return string.Empty;
    }

    private int GetNextPlayerNumber()
    {
        if(_lobby == null)
        {
            return 0;
        }

        List<int> usedPlayernumbers = new();

        foreach(var player in _lobby.Players)
        {
            if (player.Data.ContainsKey(PLAYER_NUMBER_KEY))
            {
                int currentPlayerNumber = 0;

                if(int.TryParse(player.Data[PLAYER_NUMBER_KEY].Value, out currentPlayerNumber))
                {
                    usedPlayernumbers.Add(currentPlayerNumber);
                }
            }
        }

        int playerNumber = 0;

        while(usedPlayernumbers.Contains(playerNumber))
        {
            playerNumber++;
        }

        return playerNumber;
    }
}
