using TMPro;
using UnityEngine;

public class Pullable : MonoBehaviour
{
    [SerializeField] protected FixedJoint _fixedJoint;
    [SerializeField] protected int _duckLayer = 7;
    [SerializeField] protected Material _grabbedMaterial;
    [SerializeField] protected int _weightValue = 0;
    [SerializeField] protected TextMeshProUGUI _debugWeightText;

    private Material _defaultMaterial;
    private bool _isTargeted = false;
    private MeshRenderer _meshRenderer;
    

    public bool IsTargeted {get => _isTargeted; private set => _isTargeted = value; }
    public bool IsGrabbed { get => (_fixedJoint.connectedBody != null); }

    private void Awake()
    {
        if(_fixedJoint == null) _fixedJoint = GetComponent<FixedJoint>();
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _defaultMaterial = _meshRenderer.material;
        }

        if(_debugWeightText != null) _debugWeightText.text = _weightValue.ToString();
    }


    public void OnTargeted()
    {
        _isTargeted = true;

    }

    public void OnUntargeted()
    {
        if (IsGrabbed) _meshRenderer.material = _defaultMaterial;
        _isTargeted = false;
        _fixedJoint.connectedBody = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isTargeted) return;
        if (collision.gameObject.layer != _duckLayer) return;
        print("Duck Collided with Pullable");

        DuckMovement playerDuck = collision.gameObject.GetComponent<DuckMovement>();

        if (playerDuck.PullingStrength < _weightValue)
        {
            //object to heavy feedback
            playerDuck.PullableTooHeavy();

            return;
        }


        _meshRenderer.material = _grabbedMaterial;
        _fixedJoint.connectedBody = (Rigidbody)collision.body;
    }
}
