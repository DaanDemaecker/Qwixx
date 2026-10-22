using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHostUi : NetworkBehaviour
{
    [SerializeField]
    private GameObject _buttonContainer = null;

    [SerializeField]
    private Button _button = null;

    void Start()
    {
        if (!LobbyManager.Instance.IsHostDevice)
        {
            gameObject.SetActive(false);
            return;
        }


        if (_buttonContainer != null)
        {
            _buttonContainer.SetActive(false);
        }

        LobbyManager.Instance.AllPlayersReadyEvent.AddListener(AllPlayersReady);
        if (_button != null)
        {
            _button.onClick.AddListener(StartGame);
        }
    }

    public override void OnDestroy()
    {
        if (LobbyManager.Instance.IsHostDevice)
        {
            LobbyManager.Instance.AllPlayersReadyEvent.RemoveListener(AllPlayersReady);
        }

        if (_button != null)
        {
            _button.onClick.RemoveListener(StartGame);
        }
    }


    private void AllPlayersReady()
    {
        if(_buttonContainer != null)
        {
            _buttonContainer.SetActive(true);
        }
    }

    private void StartGame()
    {
        GameManager.Instance.StartLoadingGameScene();
    }
}
