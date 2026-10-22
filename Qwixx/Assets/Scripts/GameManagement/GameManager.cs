using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public const string GAME_SCENE_NAME = "GameScene";
    private const string LOBBY_HANDLER_SCENE_NAME = "OnlineLobbyHandlerScene";
    private const string GAME_OVER_SCENE_NAME = "EndGameScene";


    private void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }


    public void Start()
    {
        ScoreManager.Instance.AllPlayerReadyEvent.AddListener(AllPlayersReadyEndGame);
        LoadLobbyScene();
    }

    public void LoadLobbyScene()
    {
        SceneManager.Instance.LoadScene(LOBBY_HANDLER_SCENE_NAME, false);
    }

    public void StartLoadingGameScene()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnGameSceneLoadEventCompleted;

        SceneManager.Instance.LoadSceneNetwork(GAME_SCENE_NAME, false);
    }

    private void SceneManager_OnGameSceneLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        UnityEngine.Debug.LogError($"Finished loading scene {sceneName}");

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnGameSceneLoadEventCompleted;

        PlayerManager.Instance.SpawnPlayerRpc();



        LeaveLobbyRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void LeaveLobbyRpc()
    {
        _ = LobbyManager.Instance.LeaveLobby();
    }

    private void AllPlayersReadyEndGame()
    {
        SceneManager.Instance.LoadSceneNetwork(GAME_OVER_SCENE_NAME, false);
    }

    public void ResetGame()
    {
        ScoreManager.Instance.ResetGame();
        PlayerManager.Instance.ResetGame();
        LoadLobbyScene();
    }
}
