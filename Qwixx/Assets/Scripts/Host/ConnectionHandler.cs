using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    [SerializeField]
    private List<uint> _alternatePrefabs = new();

    private const int HOST_PREFAB_INDEX = 0;

    private const ushort PORT = 7777;
    private const string HOST_ADRESS = "0.0.0.0";

    private const string HOST_CONNECTION_DATA = "Host:\"True\"";
    private const string CLIENT_CONNECTION_DATA = "Host:\"False\"";

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
            if (_alternatePrefabs.Count >= HOST_PREFAB_INDEX)
            {
                response.PlayerPrefabHash = _alternatePrefabs[HOST_PREFAB_INDEX];
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

            _networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(HOST_CONNECTION_DATA);
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
            _networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(CLIENT_CONNECTION_DATA);
            _networkManager.StartClient();
        }
    }
}