using UnityEngine;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField]
    private Button _createGameButton;

    [SerializeField]
    private Button _joinGameButton;

    //private string _hostAddress = "192.168.1.12";
    //private string _hostAddress = "192.168.1.4";
    private string _hostAddress = "127.0.0.1";

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
        ConnectionHandler.Instance.ConnectHost();

        GameManager.Instance.LoadLobbyScene();
        
    }

    private void JoinGame()
    {
        ConnectionHandler.Instance.ConnectClient(_hostAddress);
    }
}
