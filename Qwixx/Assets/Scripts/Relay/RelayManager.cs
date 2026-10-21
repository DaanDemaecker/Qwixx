using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    private static RelayManager _instance = null;

    public static RelayManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private Allocation _serverAllocation = null;

    private JoinAllocation _joinAllocation = null;

    private RelayConnectionManager _connectionManager = null;

    private string _joinCode = string.Empty;

    public string JoinCode
    {
        get
        {
            return _joinCode;
        }
    }

    private void Start()
    {
        if (_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
        }
        _instance = this;

        if(!TryGetComponent<RelayConnectionManager>(out _connectionManager))
        {
            Debug.LogError("No connection manager found");
        }
    }

    public async Task<bool> OpenHost()
    {
        bool result = await CreateRelay();

        if(!result)
        {
            return false;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
            _serverAllocation.RelayServer.IpV4,
            (ushort)_serverAllocation.RelayServer.Port,
            _serverAllocation.AllocationIdBytes,
            _serverAllocation.Key,
            _serverAllocation.ConnectionData
            );

        NetworkManager.Singleton.StartHost();


        return true;
    }

    private async Task<bool> CreateRelay()
    {
        if(!UserManager.Instance.IsSignedIn)
        {
            return false;
        }

        try
        {
            _serverAllocation = await RelayService.Instance.CreateAllocationAsync(GameRuleManager.Instance.MaxPlayers + 1);

            _joinCode = await RelayService.Instance.GetJoinCodeAsync(_serverAllocation.AllocationId);
        }
        catch(RelayServiceException e)
        {
            Debug.LogError(e);
        }


        return true;
    }

    public async Task<bool> StartClient(string joinCode)
    {
        bool result = await JoinRelay(joinCode);

        if (!result)
        {
            return false;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
            _joinAllocation.RelayServer.IpV4,
            (ushort)_joinAllocation.RelayServer.Port,
            _joinAllocation.AllocationIdBytes,
            _joinAllocation.Key,
            _joinAllocation.ConnectionData,
            _joinAllocation.HostConnectionData
            );

        NetworkManager.Singleton.StartClient();

        return true;
    }

    private async Task<bool> JoinRelay(string joinCode)
    {
        if (!UserManager.Instance.IsSignedIn)
        {
            return false;
        }

        try
        {
            _joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }

        return true;
    }

    public void Disconnect()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if(NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }   
}
