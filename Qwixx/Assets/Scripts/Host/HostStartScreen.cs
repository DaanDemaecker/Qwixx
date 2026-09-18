using UnityEngine;

public class HostStartScreen : MonoBehaviour
{
    [SerializeField]
    private ConnectionHandler _connectionHandler;

    public void StartHost()
    {
        if (_connectionHandler)
        {
            _connectionHandler.ConnectHost();
        }
    }
}
