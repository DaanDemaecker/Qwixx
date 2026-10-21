using Unity.Netcode;
using UnityEngine;

public class OnlinePlayerSpawner : NetworkBehaviour
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

        if (player.Data.ContainsKey(OnlineLobbyManager.PLAYER_NAME_KEY))
        {
            playerName = player.Data[OnlineLobbyManager.PLAYER_NAME_KEY].Value;
        }

        if (player.Data.ContainsKey(OnlineLobbyManager.PLAYER_COLOR_KEY))
        {
            playerColor = player.Data[OnlineLobbyManager.PLAYER_COLOR_KEY].Value;
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

        OnlineScoreManager.Instance.RegisterPlayer(UserManager.Instance.GetId(), playerName, colorOutput);

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
        OnlineTurnManager turnManager = null;

        if (!TryGetComponent<OnlineTurnManager>(out turnManager))
        {
            Debug.LogError("OnlineTurnManager component not found on the GameObject.");
            return;
        }

        OnlinePlayerManager playerManager = null;

        if (!TryGetComponent<OnlinePlayerManager>(out playerManager))
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
