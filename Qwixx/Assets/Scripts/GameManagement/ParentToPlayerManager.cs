using Unity.Netcode;
using UnityEngine;

public class ParentToPlayerManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();

        if (playerManager != null)
        {
            transform.SetParent(playerManager.transform);
        }
    }
}

