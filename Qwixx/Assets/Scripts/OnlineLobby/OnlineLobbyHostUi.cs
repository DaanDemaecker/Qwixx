using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OnlineLobbyHostUi : NetworkBehaviour
{
    [SerializeField]
    private GameObject _buttonContainer = null;

    [SerializeField]
    private Button _button = null;

    void Start()
    {
        if (!OnlineLobbyManager.Instance.IsHostDevice)
        {
            gameObject.SetActive(false);
            return;
        }


        if (_buttonContainer != null)
        {
            _buttonContainer.SetActive(false);
        }

        OnlineLobbyManager.Instance.AllPlayersReadyEvent.AddListener(AllPlayersReady);
        if (_button != null)
        {
            _button.onClick.AddListener(StartGame);
        }
    }

    public override void OnDestroy()
    {
        if (OnlineLobbyManager.Instance.IsHostDevice)
        {
            OnlineLobbyManager.Instance.AllPlayersReadyEvent.RemoveListener(AllPlayersReady);
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
        OnlineGameManager.Instance.StartLoadingGameScene();
    }
}
