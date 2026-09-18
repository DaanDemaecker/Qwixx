using UnityEngine;
using WebSocketSharp;

public class ClientStartScreen : MonoBehaviour
{
    public class HostStartScreen : MonoBehaviour
    {
        [SerializeField]
        private ConnectionHandler _connectionHandler;

        [SerializeField]
        private TMPro.TMP_InputField _adressInput;

        public void StartClient()
        {
            if(_adressInput == null || _adressInput.text.IsNullOrEmpty())
            {
                return;
            }

            if (_connectionHandler)
            {
                _connectionHandler.ConnectClient(_adressInput.text);
            }
        }
    }
}
