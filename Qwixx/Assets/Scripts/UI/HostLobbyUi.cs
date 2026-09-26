using UnityEngine;
using UnityEngine.UI;

public class HostLobbyUi : MonoBehaviour
{
    [SerializeField]
    private Button _startGameButton = null;

    private PlayerSelectReady _playerSelectReady = null;

    private void Awake()
    {
        if (_startGameButton != null)
        {
            _startGameButton.onClick.AddListener(StartGame);
        }
    }

    private void StartGame()
    {
        PlayerSelectReady playerSelectReady = GetPlayerSelectReady();

        if (playerSelectReady != null)
        {
            playerSelectReady.StartGame();
        }
    }
    
    private PlayerSelectReady GetPlayerSelectReady()
    {
        if(_playerSelectReady == null)
        {
            _playerSelectReady = FindAnyObjectByType<PlayerSelectReady>();
        }

        return _playerSelectReady;
    }
}
