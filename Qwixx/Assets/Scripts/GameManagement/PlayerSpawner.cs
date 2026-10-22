using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    // Host
    [SerializeField]
    private GameObject _hostPrefab = null;


    // Player
    [SerializeField]
    private GameObject _playerPrefab = null;


    public Player SpawnPlayer(Unity.Services.Lobbies.Models.Player player)
    {
        GameObject playerGameObject = Instantiate(_playerPrefab);

        Player playerObject = playerGameObject.GetComponent<Player>();


        string playerName = string.Empty;
        string playerColor = string.Empty;
        Color colorOutput = Color.white;

        if (player.Data.ContainsKey(LobbyManager.PLAYER_NAME_KEY))
        {
            playerName = player.Data[LobbyManager.PLAYER_NAME_KEY].Value;
        }

        if (player.Data.ContainsKey(LobbyManager.PLAYER_COLOR_KEY))
        {
            playerColor = player.Data[LobbyManager.PLAYER_COLOR_KEY].Value;
        }
        UnityEngine.ColorUtility.TryParseHtmlString($"#{playerColor}", out colorOutput);

        playerObject.InitializeValues(playerName, colorOutput);

        if (playerObject.ScoreSheet != null)
        {
            SetupCallbacks(playerObject.ScoreSheet);
        }
        else
        {
            Debug.LogError("ScoreSheet component not found on the Player GameObject.");
        }

        ScoreManager.Instance.RegisterPlayer(UserManager.Instance.GetId(), playerName, colorOutput);

        return playerObject;
    }

    public Host SpawnHost(Unity.Services.Lobbies.Models.Player player)
    {
        GameObject hostGameObject = Instantiate(_hostPrefab);

        Host hostComponent = hostGameObject.GetComponent<Host>();

        hostComponent.SpawnDice();

        return hostComponent;
    }

    private void SetupCallbacks(ScoreSheet scoreSheet)
    {
        TurnManager turnManager = null;

        if (!TryGetComponent<TurnManager>(out turnManager))
        {
            Debug.LogError("OnlineTurnManager component not found on the GameObject.");
            return;
        }

        PlayerManager playerManager = null;

        if (!TryGetComponent<PlayerManager>(out playerManager))
        {
            Debug.LogError("OnlinePlayerManager component not found on the GameObject.");
            return;
        }


        scoreSheet.OnRollClickedEvent.AddListener(playerManager.OnRollClicked);

        scoreSheet.OnReadyClickedEvent.AddListener(turnManager.OnTurnConfirmed);
        scoreSheet.OnRowLockedEvent.AddListener(turnManager.OnRowLocked);
        scoreSheet.OnPlayerDiedEvent.AddListener(turnManager.OnPlayerDied);

        turnManager.AddPlayerRpc(UserManager.Instance.GetId());
    }
}
