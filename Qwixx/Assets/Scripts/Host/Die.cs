using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Die : MonoBehaviour
{
    [SerializeField]
    private List<DieSide> _sides = new();

    [SerializeField]
    private DiceColor _color = DiceColor.Color0;

    private Rigidbody _rigidBody = null;

    private const float MAX_ANGLE = 30;

    private const float FORCE_STRENTH = 50;

    private const float TORQUE_STRENGTH = 10;

    private bool _isMoving = false;

    private float _isMovingTimer = 0f;

    private const float MIN_STOP_MOVING_TIME = 0.5f;

    private const float MOVEMENT_DELTA = 0.5f;

    private const float MAX_SLOPE_ANGLE = 5f;

    private Vector3 _beginPosition = Vector3.zero;

    public Vector3 BeginPosition
    {
        set
        {
            _beginPosition = value;
        }
    }

    public UnityEvent<DiceColor, int> RollCompleteEvent = new UnityEvent<DiceColor, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_rigidBody != null)
        {
            if(_rigidBody.linearVelocity.magnitude > MOVEMENT_DELTA || _rigidBody.angularVelocity.magnitude > MOVEMENT_DELTA)
            {
                _isMovingTimer = MIN_STOP_MOVING_TIME;
            }
            else
            {
                _isMovingTimer -= Time.deltaTime;
            }

            bool isMoving = _isMovingTimer > 0;

            if(!isMoving && _isMoving)
            {
                RollFinished();
            }
            _isMoving = isMoving;
        }
    }

    public void Roll()
    {
        if(_rigidBody == null)
        {
            return;
        }

        UnfreezeRigidBody();

        float yAngle = Random.Range(-Mathf.PI, Mathf.PI);
        float xAngle = Random.Range(0, MAX_ANGLE);

        Vector3 force = Vector3.up;

        force = Quaternion.AngleAxis(xAngle, transform.right) * force;

        force = Quaternion.AngleAxis(yAngle, transform.up) * force;

        force.Normalize();

        force *= FORCE_STRENTH;

        _rigidBody.AddForce(force);

        Vector3 torque = new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

        torque.Normalize();

        torque *= TORQUE_STRENGTH;
        _rigidBody.AddTorque(torque);
    }

    private void RollFinished()
    {
        if(_sides.Count <= 0)
        {
            Debug.Log("No sides found");
            return;
        }

        int mostUpSide = -1;
        float smallesAngle = float.MaxValue;

        for(int i = 0; i < _sides.Count; ++i)
        {
            float angle = Vector3.Angle(_sides[i].transform.forward, Vector3.up);

            if(angle < smallesAngle)
            {
                mostUpSide = i;
                smallesAngle = angle;
            }
        }

        if(mostUpSide >= 0)
        {
            if(smallesAngle > MAX_SLOPE_ANGLE)
            {
                Roll();
                return;
            }

            FreezeRigidBody();
            RollCompleteEvent.Invoke(_color, _sides[mostUpSide].Value);
        }
    }

    public void MoveToStartPosition()
    {
        if(_beginPosition != Vector3.zero)
        {
            transform.position = _beginPosition;
        }
    }

    private void FreezeRigidBody()
    {
        if (_rigidBody != null)
        {
            _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void UnfreezeRigidBody()
    {
        if(_rigidBody != null)
        {
            _rigidBody.constraints = 0;
        }
    }
}
