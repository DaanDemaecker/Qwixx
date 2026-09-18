using System;
using Unity.Netcode;
using Unity.Netcode.Transports.SinglePlayer;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace HelloWorld
{
    /// <summary>
    /// Add this component to the same GameObject as
    /// the NetworkManager component.
    /// </summary>
    public class HelloWorldManager : MonoBehaviour
    {
        private NetworkManager m_NetworkManager;
        private UnityTransport _unityTransport;
        private SinglePlayerTransport _singlePlayerTransport;

        public enum StartType
        {
            SinglePlayer,
            Client,
            Host,
            Server
        }

        private string _serverIP = string.Empty;

        private void Awake()
        {
            m_NetworkManager = GetComponent<NetworkManager>();

            if (m_NetworkManager != null)
            {

                m_NetworkManager.ConnectionApprovalCallback = ApprovalCheck;

                m_NetworkManager.OnClientDisconnectCallback += OnCLientDisconnectedCallback;
            }

            _singlePlayerTransport = GetComponent<SinglePlayerTransport>();
            _unityTransport = GetComponent<UnityTransport>();
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            // The client identifier to be authenticated
            ulong clientId = request.ClientNetworkId;

            // Additional connection data defined by user code
            byte[] connectionData = request.Payload;

            string playerName = System.Text.Encoding.Default.GetString(connectionData);

            // Your approval logic determines the following values
            response.Approved = true;
            response.CreatePlayerObject = true;

            // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
            response.PlayerPrefabHash = null;

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = Vector3.zero;

            // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
            response.Rotation = Quaternion.identity;

            // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
            // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
            response.Reason = $"I just really don't like {playerName}";

            // If additional approval steps are needed, set this to true until the additional steps are complete
            // once it transitions from true to false the connection approval response will be processed.
            response.Pending = false;
        }

        private void OnCLientDisconnectedCallback(ulong obj)
        {
            if(!m_NetworkManager.IsServer && m_NetworkManager.DisconnectReason != string.Empty)
            {
                Debug.LogError($"Approval Declined Reason: {m_NetworkManager.DisconnectReason}");
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        private void StartButtons()
        {
            if (GUILayout.Button("SinglePlayer", GUILayout.Height(50)))
            {
                StartSession(StartType.SinglePlayer);
            }

            if (GUILayout.Button("Host", GUILayout.Height(50)))
            {
                StartSession(StartType.Host);
            }

            _serverIP = GUILayout.TextField(_serverIP, 25, GUILayout.Height(50));

            if (GUILayout.Button("Client", GUILayout.Height(50)))
            {
                m_NetworkManager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(_serverIP);

                StartSession(StartType.Client);
            }
            if (GUILayout.Button("Server", GUILayout.Height(50)))
            {
                StartSession(StartType.Server);
            }
        }

        private void StartSession(StartType type)
        {
            bool startStatus = false;

            m_NetworkManager.NetworkConfig.NetworkTransport = type == StartType.SinglePlayer ? _singlePlayerTransport : _unityTransport;
            
            switch(type)
            {
                case StartType.SinglePlayer:
                    startStatus = m_NetworkManager.StartHost();
                    break;
                case StartType.Host:
                    _unityTransport.SetConnectionData("0.0.0.0", 7777);
                    startStatus = m_NetworkManager.StartHost();
                    break;
                case StartType.Server:
                    _unityTransport.SetConnectionData("0.0.0.0", 7777);
                    startStatus = m_NetworkManager.StartServer();
                    break;
                case StartType.Client:
                    _unityTransport.SetConnectionData(_serverIP, 7777);
                    startStatus = m_NetworkManager.StartClient();
                    break;
            }
        }

        private void StatusLabels()
        {
            var mode = m_NetworkManager.IsHost ?
                "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        private void SubmitNewPosition()
        {
            if (GUILayout.Button(m_NetworkManager.IsServer ? "Move" : "Request Position Change"))
            {
                if (m_NetworkManager.IsServer && !m_NetworkManager.IsClient)
                {
                    foreach (ulong uid in m_NetworkManager.ConnectedClientsIds)
                        m_NetworkManager.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                }
                else
                {
                    var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }
    }
}
