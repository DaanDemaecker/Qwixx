using System;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

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

    private async void OnJoinLobbyButtonClicked()
    {
        if(_lobbyCodeInputField != null && !_lobbyCodeInputField.text.IsNullOrEmpty() && _playerNameInputField != null && !_playerNameInputField.text.IsNullOrEmpty())
        {
            if(await OnlineLobbyManager.Instance.JoinLobby(_lobbyCodeInputField.text))
            {                 
                Debug.LogError($"Lobby joined: with code {_lobbyCodeInputField.text}");

                OnlineLobbyManager.Instance.UpdatePlayerData(OnlineLobbyManager.PlayerNameKey, _playerNameInputField.text);

                SceneManager.Instance.LoadScene(ONLINE_LOBBY_SCENE_NAME, false);
            }
            else
            {
                Debug.LogError($"Failed to join lobby with code {_lobbyCodeInputField.text}");
            }
        }
    }

    private async void OnCreateLobbyButtonClicked()
    {
        if(await OnlineLobbyManager.Instance.CreateLobby())
        {
            Debug.LogError($"Lobby created: with code {OnlineLobbyManager.Instance.GetLobbyCode()}");

            SceneManager.Instance.LoadScene(ONLINE_LOBBY_SCENE_NAME, false);
        }
        else
        {
            Debug.LogError("Failed to create lobby");
        }
    }
}
