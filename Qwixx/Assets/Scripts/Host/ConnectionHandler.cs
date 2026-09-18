using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    [SerializeField]
    private List<uint> _alternatePrefabs = new();

    private const ushort PORT = 2222;
    private const string HOST_ADRESS = "0.0.0.0";

    private NetworkManager _networkManager;
    private UnityTransport _unityTransport;

    private void Start()
    {
        _networkManager = GetComponent<NetworkManager>();

        _unityTransport = GetComponent<UnityTransport>();

        if (_networkManager != null)
        {
            _networkManager.ConnectionApprovalCallback = ApprovalCheck;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        ulong clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        byte[] connectionData = request.Payload;

        string decodedData = System.Text.Encoding.Default.GetString(connectionData);

        if(IsRequestHost(decodedData))
        {
            Debug.Log("Connection is host");

            response.Approved = true;
            response.CreatePlayerObject = true;
            if (_alternatePrefabs.Count > 0)
            {
                response.PlayerPrefabHash = _alternatePrefabs[0];
            }
        }
        else
        {
            Debug.Log("Connection is client");
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.PlayerPrefabHash = null;
        }
    }


    private bool IsRequestHost(string decodedData)
    {
        Debug.Log(decodedData);

        Regex captureRegex = new Regex("Host:\"(.*)\"");

        Match match = captureRegex.Match(decodedData);

        if (match.Success)
        {
            GroupCollection groups = match.Groups;

            if (groups.Count >= 2)
            {
                bool isHost = false;

                bool.TryParse(groups[1].Value, out isHost);

                return isHost;
            }
        }

        return false;
    }


    public void ConnectHost()
    {
        if(_networkManager != null)
        {
            if(_unityTransport != null)
            {
                _unityTransport.SetConnectionData(HOST_ADRESS, PORT);
            }

            _networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("Host:\"True\"");
            _networkManager.StartHost();
        }
    }

    public void ConnectClient(string hostAddress)
    {
        if (_networkManager != null)
        {
            if (_unityTransport != null)
            {
                _unityTransport.SetConnectionData(hostAddress, PORT);
            }
            _networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("Host:\"False\"");
            _networkManager.StartClient();
        }
    }
}