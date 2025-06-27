using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class NewPullable : Grabable
{
    [Header("Pullable")]
    [SerializeField] protected FixedJoint _fixedJoint;
    [SerializeField, Range(0, 5)] protected int _ducklingsRequiredToPull;
    [SerializeField] float _pullingSpeed = 2f;
    [SerializeField] float _acceleration = 1f;

    private Rigidbody _duckRB;

    private bool _isBeingPulled = false;

    public bool IsBeingPulled { get => _isBeingPulled; }

    protected override void OnTriggerEnter(Collider other)
    {
        _duckRB = other.GetComponent<Rigidbody>();

        base.OnTriggerEnter(other);
    }

    private void FixedUpdate()
    {
        //if duck is moving and this is being pulled
        if (!_isBeingPulled) return;
        if (!_playerDuckController.IsMoving) return;

        //move in direction towards duck
        
        Vector3 direction = (_duckRB.transform.position - transform.position).normalized;
        Vector3 targetVelocity = direction * _pullingSpeed;
        Vector3 diff = targetVelocity - _rigidbody.linearVelocity;
        diff.y = 0f;
        Vector3 forceToAdd = targetVelocity * _acceleration;

        _rigidbody.AddForce(forceToAdd);

        Debug.Log("force applied to pullable" + forceToAdd);
    }

    public override void TryGrab()
    {
        if (!_isTargeted) return;

        if (_playerDuckController.DucklingsFollowingCount < _ducklingsRequiredToPull)
        {
            UnableToPull();
            return;
        }

        base.TryGrab();

        //old grabbing mechanic
        _fixedJoint.connectedBody = _duckRB;

        //_isBeingPulled = true;

    }

    public override void TryUnGrab()
    {
        base.TryUnGrab();

        //old
        _fixedJoint.connectedBody = null;

        //_isBeingPulled = false;
    }

    private void StartPulling()
    {

    }

    private void StopPulling()
    {

    }

    private void UnableToPull()
    {
        //tell character they cant pull this object

    }

}
