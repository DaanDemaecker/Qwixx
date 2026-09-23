using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField]
    private GameObject _canvas;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Camera camera = GetComponent<Camera>();

            if (camera != null)
            {
                if(IsServer)
                {
                    camera.transform.position = Camera.main.transform.position;
                    camera.transform.rotation = Camera.main.transform.rotation;
                }

                Destroy(Camera.main.gameObject);
                camera.enabled = true;
            }
        }
        else
        {
            AudioListener listener = GetComponent<AudioListener>();
            listener.enabled = false;
        }

        if(_canvas != null)
        {
            _canvas.SetActive(IsOwner);
        }
    }
}