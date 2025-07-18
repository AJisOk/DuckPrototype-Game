using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Grabable : MonoBehaviour
{
    //base class for grabables and pullables the player interacts with

    [Header("Interaction")]
    [SerializeField] protected Collider _grabRangeTrigger;
    [SerializeField] protected int _duckLayer = 7;

    [Header("Highlight")]
    [SerializeField] protected CanvasGroup _popupCanvasGroup;
    [SerializeField] protected Material _grabbedMaterial;
    [SerializeField] protected Material _higlightMaterial;

    protected bool _isTargeted = false;
    protected bool _isHighlighted = false;
    protected bool _isGrabbed = false;
    protected bool _isPlayerNearby = false;

    protected DuckCharacterController _playerDuckController;
    protected MeshRenderer _renderer;
    protected Material _defaultMaterial;
    protected Rigidbody _rigidbody;

    protected List<Material> baseMaterials = new List<Material>();
    protected List<Material> highlightMaterials = new List<Material>();
    protected List<Material> grabbedMaterials = new List<Material>();

    public UnityEvent OnGrabbed;
    public List<Transform> DucklingGrabTransforms = new List<Transform>();
    public bool IsPlayerNearby { get => _isPlayerNearby; }

    protected virtual void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        //_renderer.GetMaterials(baseMaterials);
        //higlightMaterials = baseMaterials;
        //grabbedMaterials = baseMaterials;

        foreach(Material mat in _renderer.materials)
        {
            baseMaterials.Add(mat);
            highlightMaterials.Add(mat);
            grabbedMaterials.Add(mat);
        }

        highlightMaterials.Add(_higlightMaterial);
        grabbedMaterials.Add(_grabbedMaterial);

        _rigidbody = GetComponent<Rigidbody>();

        _playerDuckController = FindAnyObjectByType(typeof(DuckCharacterController)).GetComponent<DuckCharacterController>();
    }

    public virtual void OnTarget()
    {
        _isTargeted = true;

        //if player is still next to grabable, forego waiting for it to enter trigger and tell it to try and grab

        if (_isPlayerNearby) _playerDuckController.Grab();
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

        if(!_isGrabbed) _renderer.SetMaterials(highlightMaterials);
        
    }

    public virtual void OnUnhighlight()
    {
        _isHighlighted = false;
        _popupCanvasGroup.alpha = 0f;

        if (!_isGrabbed) _renderer.SetMaterials(baseMaterials);
    }

    public virtual void TryGrab()
    {
        if (!_isTargeted) return;
        
        _isGrabbed = true;
        OnGrabbed.Invoke();

        _renderer.SetMaterials(grabbedMaterials);
    }

    public virtual void TryUnGrab()
    {
        if (!_isTargeted) return;
        _isGrabbed = false;

        _renderer.SetMaterials(baseMaterials);
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
        //M > if targeted, grabable is grabbed by player

        if (other.gameObject.layer != _duckLayer) return;

        _isPlayerNearby = true;

        _playerDuckController = other.gameObject.GetComponent<DuckCharacterController>();

        if (_isTargeted)
        {
            //Grab object
            //TryGrab();
            _playerDuckController.Grab();
            //return
            return;
        }

    }

    protected virtual void OnTriggerExit(Collider other)
    {
        //M > same
        
        if (other.gameObject.layer != _duckLayer) return;

        _isPlayerNearby = false;

        if (_isGrabbed) _playerDuckController.UnGrab();

        //if(_isTargeted)
        //{
        //    _playerDuckController.OnUntargetCurrentGrabable(this);  
        //    //OnUntarget();
        //}


    }
}
