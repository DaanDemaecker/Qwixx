using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MainMenuLaunchable : MonoBehaviour
{
    private Rigidbody _rb = null;

    private Vector3 _startPos = Vector3.zero;
    private Quaternion _startRot = Quaternion.identity;
    private Vector3 _startScale = Vector3.zero;

    private float _launchDuration = 2f;
    private float _returnDuration = 0.5f;

    private float _cameraOffset = 2f;

    private bool _isLaunching = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        _startRot = transform.rotation;
        _startScale = transform.localScale;
    }


    public void Launch(Vector3 direction, float magnitude)
    {
        if(_isLaunching)
        {
            return;
        }

        if(_rb != null)
        {
            _rb.constraints = RigidbodyConstraints.None;

            Vector3 force = direction.normalized * magnitude;

            _rb.AddForce(force, ForceMode.Impulse);

            _isLaunching = true;

            StartCoroutine(HandleLaunch());
        }
    }

    private IEnumerator HandleLaunch()
    {
        float launchTimer = _launchDuration;

        while (launchTimer > 0)
        {
            launchTimer -= Time.deltaTime;

            float t = 1 - launchTimer / _launchDuration;

            transform.localScale = Vector3.Lerp(_startScale, Vector3.zero, t);

            yield return null;
        }

        StartCoroutine(HandleReturn());
    }

    private IEnumerator HandleReturn()
    {
        float returnTimer = _returnDuration;

        Vector3 returnStartPos = Camera.main.transform.position - Camera.main.transform.forward * _cameraOffset;
        Vector3 returnStartScale = transform.localScale;
        Quaternion returnStartRotation = transform.rotation;

        while (returnTimer > 0)
        {
            returnTimer -= Time.deltaTime;

            float t =  1 - returnTimer / _returnDuration;

            transform.position = Vector3.Lerp(returnStartPos, _startPos, t);
            transform.localScale = Vector3.Lerp(returnStartScale, _startScale, t);
            transform.rotation = Quaternion.Lerp(returnStartRotation, _startRot, t);


            yield return null;
        }

        transform.position = _startPos;
        transform.localScale = _startScale;
        transform.rotation = _startRot;
        _isLaunching = false;
        if(_rb != null)
        {
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
