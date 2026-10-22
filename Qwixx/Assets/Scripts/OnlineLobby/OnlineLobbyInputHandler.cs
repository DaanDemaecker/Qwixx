using UnityEngine;
using UnityEngine.UI;

public class OnlineLobbyInputHandler : MonoBehaviour
{
    [SerializeField]
    private Button _createLobbyButton = null;

    [SerializeField]
    private TMPro.TMP_InputField _playerNameInputField = null;

    [SerializeField]
    private TMPro.TMP_InputField _lobbyCodeInputField = null;

    [SerializeField]
    private Button _joinLobbyButton = null;

    private const string ONLINE_LOBBY_SCENE_NAME = "OnlineLobbyScene";

    private bool _shouldJoinLobby = false;
    private bool _tryingToJoin = false;

    private void Awake()
    {
        if (UserManager.Instance.IsHostDevice)
        {
            if (_createLobbyButton != null)
            {
                _createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
            }

            if (_joinLobbyButton != null)
            {
                _joinLobbyButton.gameObject.SetActive(false);
            }

            if(_playerNameInputField != null)
            {
                _playerNameInputField.gameObject.SetActive(false);
            }

            if(_lobbyCodeInputField != null)
            {
                _lobbyCodeInputField.gameObject.SetActive(false);
            }
        }
        else
        {
            if (_joinLobbyButton != null)
            {
                _joinLobbyButton.onClick.AddListener(OnJoinLobbyButtonClicked);
            }

            if(_createLobbyButton != null)
            {
                _createLobbyButton.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if(_shouldJoinLobby && !_tryingToJoin)
        {
            JoinConnection();
        }
    }

    private async void JoinConnection()
    {
        _tryingToJoin = true;

        string relayJoinKey = OnlineLobbyManager.Instance.GetLobbyData(OnlineLobbyManager.RELAY_JOIN_CODE_KEY);

        if (string.IsNullOrEmpty(relayJoinKey))
        {
            _tryingToJoin = false;
            return;
        }

        if (!await RelayManager.Instance.StartClient(relayJoinKey))
        {
            _tryingToJoin = false;
            return;
        }

        _shouldJoinLobby = false;
        _tryingToJoin = false;
    }

    private async void OnJoinLobbyButtonClicked()
    {
        if(_lobbyCodeInputField != null && !string.IsNullOrEmpty(_lobbyCodeInputField.text) && _playerNameInputField != null && !string.IsNullOrEmpty(_playerNameInputField.text))
        {
            if(await OnlineLobbyManager.Instance.JoinLobby(_lobbyCodeInputField.text))
            {                 
                OnlineLobbyManager.Instance.UpdatePlayerData(OnlineLobbyManager.PLAYER_NAME_KEY, _playerNameInputField.text);
                OnlineLobbyManager.Instance.UpdatePlayerData(OnlineLobbyManager.PLAYER_NAME_SET_KEY, true.ToString());

                string relayJoinKey = OnlineLobbyManager.Instance.GetLobbyData(OnlineLobbyManager.RELAY_JOIN_CODE_KEY);

                if (string.IsNullOrEmpty(relayJoinKey))
                {
                    _shouldJoinLobby = true;
                    return;
                }

                if (!await RelayManager.Instance.StartClient(relayJoinKey))
                {
                    _shouldJoinLobby = true;
                    return;
                }
            }
            else
            {
                Debug.LogError($"Failed to join lobby with code {_lobbyCodeInputField.text}");
            }
        }
    }

    private async void OnCreateLobbyButtonClicked()
    {
        if(!await RelayManager.Instance.OpenHost())
        {
            Debug.LogError("Failed to open host");
            return;
        }

        if(await OnlineLobbyManager.Instance.CreateLobby())
        {
            SceneManager.Instance.LoadSceneNetwork(ONLINE_LOBBY_SCENE_NAME, false);
        }
        else
        {
            Debug.LogError("Failed to create lobby");
        }
    }
}
