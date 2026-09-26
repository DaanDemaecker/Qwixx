using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerLobbyUi : MonoBehaviour
{
    [SerializeField]
    private Button _readyButton = null;

    [SerializeField]
    private TMPro.TMP_InputField _inputField = null;

    public UnityEvent<string> OnInputfieldChanged;

    public void Awake()
    {
        if (_readyButton != null)
        {
            _readyButton.onClick.AddListener(Ready);
        }

        if(_inputField != null)
        {
            _inputField.onValueChanged.AddListener(PlayerLobbyUi_OnInputfieldChanged);
        }
    }

    private void PlayerLobbyUi_OnInputfieldChanged(string newText)
    {
        OnInputfieldChanged.Invoke(newText);
    }


    private void Ready()
    {
        if(!CanBeReady())
        {
            return;
        }

        PlayerSelectReady playerSelectReady = FindAnyObjectByType<PlayerSelectReady>();

        if(playerSelectReady != null)
        {
            playerSelectReady.SetPlayerReady();
        }

        if (_inputField != null)
        {
            _inputField.gameObject.SetActive(false);
        }
    }

    private bool CanBeReady()
    {
        return true;
    }
}
