using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField]
    private GameObject _canvas;

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

        if(_canvas != null)
        {
            _canvas.SetActive(IsOwner);
        }
    }
}