using UnityEngine;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField]
    private Button _createGameButton;

    [SerializeField]
    private Button _joinGameButton;

    [SerializeField]
    private string _hostAddress = "192.168.1.4";

    private void Awake()
    {
        if(_createGameButton != null)
        {
            _createGameButton.onClick.AddListener(CreateGame);
        }

        if(_joinGameButton != null)
        {
            _joinGameButton.onClick.AddListener(JoinGame);
        }
    }

    private void CreateGame()
    {
        ConnectionHandler connectionHandler = FindAnyObjectByType<ConnectionHandler>();

        if(connectionHandler != null)
        {
            connectionHandler.ConnectHost();

            GameManager gameManager = FindAnyObjectByType<GameManager>();

            if(gameManager != null)
            {
                gameManager.LoadLobbyScene();
            }
        }
    }

    private void JoinGame()
    {
        ConnectionHandler connectionHandler = FindAnyObjectByType<ConnectionHandler>();

        if (connectionHandler != null)
        {
            connectionHandler.ConnectClient(_hostAddress);
        }
    }
}
