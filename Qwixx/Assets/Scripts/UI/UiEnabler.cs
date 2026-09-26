using Unity.Netcode;
using UnityEngine;

public class UiEnabler : MonoBehaviour
{
    [SerializeField]
    private GameObject _hostUi = null;

    [SerializeField]
    private GameObject _clientUi = null;

    private void Awake()
    {
        if(NetworkManager.Singleton == null)
        {
            return;
        }

        bool isHost = NetworkManager.Singleton.IsHost;

        if(_hostUi != null)
        {
            _hostUi.SetActive(isHost);
        }

        if(_clientUi != null)
        {
            _clientUi.SetActive(!isHost);
        }
    }
}
