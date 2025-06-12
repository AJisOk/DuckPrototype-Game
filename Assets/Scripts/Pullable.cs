using UnityEngine;

public class Pullable : MonoBehaviour
{
    [SerializeField] protected FixedJoint _fixedJoint;
    [SerializeField] protected int _duckLayer = 7;

    private bool _isTargeted = false;

    public bool IsTargeted {get => _isTargeted; private set => _isTargeted = value; }

    private void Awake()
    {
        if(_fixedJoint == null) _fixedJoint = GetComponent<FixedJoint>();
    }

    public void OnTargeted()
    {
        _isTargeted = true;


    }

    public void OnUntargeted()
    {
        _isTargeted = false;

        _fixedJoint.connectedBody = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isTargeted) return;
        if (collision.gameObject.layer != _duckLayer) return;

        print("Duck Collided with Pullable");

        

        _fixedJoint.connectedBody = (Rigidbody)collision.body;
    }
}
