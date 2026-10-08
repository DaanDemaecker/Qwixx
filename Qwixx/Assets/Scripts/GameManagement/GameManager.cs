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

    private const string START_MENU_SCENE_NAME = "StartMenuScene";

    private const string LOBBY_SCENE_NAME = "LobbyScene";

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
        }
        _instance = this;

    }

    public void Start()
    {
        _sceneManager.LoadScene(START_MENU_SCENE_NAME, false);
    }

    public void LoadLobbyScene()
    {
        if(IsServer)
        {
            _sceneManager.LoadSceneNetwork(LOBBY_SCENE_NAME, false);
        }
    }
}
