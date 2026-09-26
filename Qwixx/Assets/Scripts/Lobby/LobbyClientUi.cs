using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LobbyClientUi : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_InputField _nameInputField = null;

    [SerializeField]
    private GameObject _buttonsContainer = null;

    public UnityEvent<string> onNameChangedEvent;

    public UnityEvent<Color> onColorChangedEvent;

    public UnityEvent onPlayerReadyEvent;

    [SerializeField]
    private List<LobbyColorButton> _colorButtons = new();

    private void Awake()
    {
        if(_nameInputField != null)
        {
            _nameInputField.onValueChanged.AddListener(LobbyClientUi_OnNameInputFieldChanged);
        }

        foreach(LobbyColorButton button in _colorButtons)
        {
            if(button == null)
            {
                continue;
            }

            button.onColorSelected.AddListener(LobbyClientUi_OnColorChanged);
        }
    }

    private void LobbyClientUi_OnNameInputFieldChanged(string newName)
    {
        onNameChangedEvent.Invoke(newName);
    }

    private void LobbyClientUi_OnColorChanged(Color newColor)
    {
        onColorChangedEvent.Invoke(newColor);
    }

    public void OnReadyClicked()
    {
        if (_nameInputField != null)
        {
            _nameInputField.gameObject.SetActive(false);
        }

        if(_buttonsContainer != null)
        {
            _buttonsContainer.SetActive(false);
        }

        onPlayerReadyEvent.Invoke();
    }
}
