using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            Camera camera = GetComponent<Camera>();

            if (camera != null)
            {
                Camera.main.gameObject.SetActive(false);
                camera.enabled = true;
            }
        }
    }
}