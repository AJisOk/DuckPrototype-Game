using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NewPullable : Grabable
{
    [Header("Pullable")]
    [SerializeField] protected FixedJoint _fixedJoint;
    [SerializeField, Range(0, 5)] protected int _ducklingsRequiredToPull;
    [SerializeField] protected TextMeshProUGUI _ducklingsRequiredText;
    [SerializeField] float _pullingSpeed = 2f;
    [SerializeField] float _acceleration = 1f;

    private Rigidbody _duckRB;

    private bool _isBeingPulled = false;

    public bool IsBeingPulled { get => _isBeingPulled; }

    protected override void Awake()
    {
        base.Awake();

        if(_ducklingsRequiredText != null) _ducklingsRequiredText.text = "0/" + _ducklingsRequiredToPull.ToString();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        _duckRB = other.GetComponent<Rigidbody>();

        base.OnTriggerEnter(other);
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

        //if (_ducklingsRequiredToPull > 0) _playerDuckController.DucklingsSwarmPullable();

        //_isBeingPulled = true;

    }

    public override void TryUnGrab()
    {
        base.TryUnGrab();

        //old
        _fixedJoint.connectedBody = null;

        //_isBeingPulled = false;
    }

    private void UnableToPull()
    {
        //tell character they cant pull this object
        _playerDuckController.UnableToGrab();
    }

}
