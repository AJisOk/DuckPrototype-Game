using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Grabable : MonoBehaviour
{
    //base class for grabables and pullables the player interacts with

    [Header("Interaction")]
    [SerializeField] protected Collider _grabRangeTrigger;
    [SerializeField] protected int _duckLayer = 7;
    [SerializeField] protected CanvasGroup _popupCanvasGroup;
    [SerializeField] protected Material _grabbedMaterial;


    protected bool _isTargeted = false;
    protected bool _isHighlighted = false;
    protected bool _isGrabbed = false;
    protected bool _isPlayerNearby = false;

    protected DuckCharacterController _playerDuckController;
    protected MeshRenderer _renderer;
    protected Material _defaultMaterial;
    protected Rigidbody _rigidbody;

    public UnityEvent OnGrabbed;

    
    public List<Transform> DucklingGrabTransforms = new List<Transform>();
    public bool IsPlayerNearby { get => _isPlayerNearby; }

    protected virtual void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _defaultMaterial = _renderer.material;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void OnTarget()
    {
        _isTargeted = true;
    }

    public virtual void OnUntarget()
    {
        if(_isGrabbed) TryUnGrab();
        _isTargeted = false;
    }

    public virtual void OnHighlight()
    {
        _isHighlighted = true;
        _popupCanvasGroup.alpha = 1f;
    }

    public virtual void OnUnhighlight()
    {
        _isHighlighted = false;
        _popupCanvasGroup.alpha = 0f;
    }

    public virtual void TryGrab()
    {
        if (!_isTargeted) return;
        
        _isGrabbed = true;
        OnGrabbed.Invoke();

        _renderer.material = _grabbedMaterial;

        //Debug.Log("TryGrab called on Grabable");
    }

    public virtual void TryUnGrab()
    {
        if (!_isTargeted) return;
        _isGrabbed = false;

        _renderer.material = _defaultMaterial;

        //Debug.Log("TryUnGrab Called on Grabable");
    }

    protected virtual void OnMouseEnter()
    {
        OnHighlight();
    }

    protected virtual void OnMouseExit()
    {
        OnUnhighlight();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

        //K+GP > if is highlighted, grabable becomes targeted
        //M > if targeted, grabable is grabbed by player

        if (other.gameObject.layer != _duckLayer) return;

        _isPlayerNearby = true;
        _isHighlighted = true;

        _playerDuckController = other.gameObject.GetComponent<DuckCharacterController>();

        if (_isTargeted)
        {
            //Grab object
            //TryGrab();
            _playerDuckController.Grab();
            //return
            return;
        }

        if(_isHighlighted)
        {
            //target object
            //OnTarget();
            _playerDuckController.OnTargetGrabable(this);
            //return
            return;
        }

    }

    protected virtual void OnTriggerExit(Collider other)
    {
        //k+GP > if targeted > untarget
        //M > same
        
        if (other.gameObject.layer != _duckLayer) return;

        _isPlayerNearby = false;
        _isHighlighted = false;

        //if (_isGrabbed) _playerDuckController.UnGrab();

        //if(_isTargeted)
        //{
        //    _playerDuckController.OnUntargetCurrentGrabable(this);  
        //    //OnUntarget();
        //}
        

    }
}
