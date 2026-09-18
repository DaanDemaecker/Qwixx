using UnityEngine;
using WebSocketSharp;

public class StartScreen : MonoBehaviour
{
    [SerializeField]
    private ConnectionHandler _connectionHandler;

    [SerializeField]
    private TMPro.TMP_InputField _adressInput;


    public void StartHost()
    {
        if (_connectionHandler)
        {
            _connectionHandler.ConnectHost();
        }
    }

    public void StartClient()
    {
        if (_adressInput == null || _adressInput.text.IsNullOrEmpty())
        {
            return;
        }

        if (_connectionHandler)
        {
            _connectionHandler.ConnectClient(_adressInput.text);
        }
    }
}
