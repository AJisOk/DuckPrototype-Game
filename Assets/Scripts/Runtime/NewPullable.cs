using System.Runtime.CompilerServices;
using UnityEngine;

public class NewPullable : Grabable
{
    [Header("Pullable")]
    [SerializeField] protected FixedJoint _fixedJoint;
    [SerializeField, Range(0, 5)] protected int _ducklingsRequiredToPull;

    private Rigidbody _duckRB;

    protected override void OnTriggerEnter(Collider other)
    {
        _duckRB = other.GetComponent<Rigidbody>();

        base.OnTriggerEnter(other);
    }

    public override void TryGrab()
    {
        base.TryGrab();

        //check if player is strong enough to pull this pullable

        _fixedJoint.connectedBody = _duckRB;
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


}
