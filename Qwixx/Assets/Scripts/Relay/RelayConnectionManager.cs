using Unity.Netcode;
using UnityEngine;

public class RelayConnectionManager : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnected;
    }

    private void OnDisconnected(ulong clientId)
    {
        Debug.LogError($"Client {clientId} disconnected, {NetworkManager.Singleton.DisconnectReason}");
    }



    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        ulong clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        byte[] connectionData = request.Payload;

        string decodedData = System.Text.Encoding.Default.GetString(connectionData);

        if (NetworkManager.Singleton.ConnectedClients.Count - 1 >= GameRuleManager.Instance.MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "Max players reached";
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;

    }
}
