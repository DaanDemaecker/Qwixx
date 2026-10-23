using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuDiceHandler : MonoBehaviour
{
    private Vector2 _screenPos = Vector2.zero;

    [SerializeField]
    private float forceMagnitude = 1.0f;

    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(_screenPos.x, _screenPos.y, 1));

        Ray ray = new Ray
        {
            origin = Camera.main.transform.position,
            direction = pos - Camera.main.transform.position
        };

        RaycastHit result;

        if(Physics.Raycast(ray, out result))
        {
            if(result.collider.gameObject.TryGetComponent<MainMenuDie>(out MainMenuDie dieComponent))
            {
                Vector3 direction = result.point - Camera.main.transform.position;

                dieComponent.Launch(direction, forceMagnitude);
            }
        }
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        _screenPos = context.ReadValue<Vector2>();
    }
}
