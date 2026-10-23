using UnityEngine;

public class MainMenuDie : MonoBehaviour
{
    private Rigidbody _rb = null;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    public void Launch(Vector3 direction, float magnitude)
    {
        if(_rb != null)
        {
            _rb.constraints = RigidbodyConstraints.None;

            Vector3 force = direction.normalized * magnitude;

            _rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
