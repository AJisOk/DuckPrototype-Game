using UnityEngine;
using UnityEngine.Events;

public class Grabable : MonoBehaviour
{
    //base class for grabables and pullables the player interacts with

    [Header("Interaction")]
    [SerializeField] protected Collider _grabRangeTrigger;
    [SerializeField] protected int _duckLayer = 7;
    [SerializeField] protected CanvasGroup _popupCanvasGroup;

    private bool _isTargeted = false;
    private bool _isHighlighted = false;
    private bool _isGrabbed = false;

    public UnityEvent OnGrabbed;

    private void Awake()
    {
        
    }

    public void OnTarget()
    {
        _isTargeted = true;
    }

    public void OnUntarget()
    {
        if(_isGrabbed) TryUnGrab();
        _isTargeted = false;
    }

    public void OnHighlight()
    {
        _isHighlighted = true;
        _popupCanvasGroup.alpha = 1f;
    }

    public void OnUnhighlight()
    {
        _isHighlighted = false;
        _popupCanvasGroup.alpha = 0f;
    }

    public void TryGrab()
    {
        if (!_isTargeted) return;
        
        _isGrabbed = true;
        OnGrabbed.Invoke();
    }

    public void TryUnGrab()
    {
        if (!_isTargeted) return;
        _isGrabbed = false;
    }

    private void OnMouseEnter()
    {
        OnHighlight();
    }

    private void OnMouseExit()
    {
        OnUnhighlight();
    }

    private void OnTriggerEnter(Collider other)
    {
        //K+GP > if is highlighted, grabable becomes targeted
        //M > if targeted, grabable is grabbed by player

        if (other.gameObject.layer != _duckLayer) return;

        if (_isTargeted)
        {
            //Grab object
            TryGrab();
            //return
            return;
        }

        if(_isHighlighted)
        {
            //target object
            OnTarget();
            //return
            return;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        //k+GP > if targeted - untarget
        //M > same
        
        if (other.gameObject.layer != _duckLayer) return;

        if(_isTargeted) OnUntarget();
    }
}
