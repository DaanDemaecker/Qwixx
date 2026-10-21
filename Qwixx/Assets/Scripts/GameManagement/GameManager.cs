using Unity.Netcode;
using UnityEngine;

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

    [SerializeField]
    private SceneManager _sceneManager = null;

    private const string LOBBY_HANDLER_SCENE_NAME = "OnlineLobbyHandlerScene";

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        LoadLobbyScene();
    }

    public void LoadLobbyScene()
    {
        _sceneManager.LoadScene(LOBBY_HANDLER_SCENE_NAME, false);
    }
}
