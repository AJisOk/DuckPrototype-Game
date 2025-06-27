using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class NewPullable : Grabable
{
    [Header("Pullable")]
    [SerializeField] protected FixedJoint _fixedJoint;
    [SerializeField, Range(0, 5)] protected int _ducklingsRequiredToPull;

    private Rigidbody _duckRB;

    private bool _isBeingPulled = false;

    public bool IsBeingPulled { get => _isBeingPulled; }

    protected override void OnTriggerEnter(Collider other)
    {
        _duckRB = other.GetComponent<Rigidbody>();

        base.OnTriggerEnter(other);
    }

    public override void TryGrab()
    {
        if (!_isTargeted) return;

        if (_characterController.DucklingsFollowingCount < _ducklingsRequiredToPull)
        {
            UnableToPull();
            return;
        }

        base.TryGrab();

        //check if player is strong enough to pull this pullable


        //old grabbing mechanic
        //_fixedJoint.connectedBody = _duckRB;


    }

    public override void TryUnGrab()
    {
        base.TryUnGrab();

        _fixedJoint.connectedBody = null;
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
