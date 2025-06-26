using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DuckCharacterController : MonoBehaviour
{
    //Character controller will handle input and movement
    //2 input modes - moveto and directional
    //1 input actions

    [Header("Components")]
    [SerializeField] protected NavMeshAgent _duckAgent;
    [SerializeField] protected Animator _spriteAnimator;

    [Header("Movement")]
    [SerializeField] protected LayerMask _moveToLayerMask;
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _acceleration = 10f;
    [SerializeField] protected float _turnSpeed = 5f;
    //[SerializeField] protected bool _mouseMovement = false;

    [Header("Grabbing + Pulling")]
    [SerializeField] protected LayerMask _grabableLayerMask;
    [SerializeField] protected int _grabableLayerIndex;

    //private variables
    private PlayerInput _playerInput;

    private Vector3 _moveToDestination;
    private Grabable _currentHighlightedGrabable = null;
    private Grabable _currentTargetGrabable = null;
    private Vector3 _moveInput;
    private Vector3 _localMoveInput;
    private Vector3 _lookDirection;


    private bool _isMoving;
    private bool _isGrabbing = false;
    private bool _canMove = true;
    private bool _hasMoveInput = false;
    private bool _hasTurnInput = false;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _lookDirection = transform.forward;

    }

    private void Update()
    {
        //rotate character rowards movement direction
        if (_hasTurnInput)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
            Quaternion rotation = Quaternion.Slerp(_rigidbody.transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
            _rigidbody.transform.rotation = rotation;
        }
    }

    private void LateUpdate()
    {
        //update duck sprite based on rotation
        float normalizedEulerAngle = Mathf.InverseLerp(0f, 360f, transform.rotation.eulerAngles.y);
        _spriteAnimator.SetFloat("NormalizedEulerAngle", normalizedEulerAngle);

    }

    private void FixedUpdate()
    {
        //calculate movement for keyboard+controller
        _duckAgent.nextPosition = _rigidbody.transform.position;
        
        SetLookDirection(_moveInput);

        Vector3 targetVelocity = _moveInput * _speed;

        Vector3 velocityDiff = targetVelocity - _rigidbody.linearVelocity;
        velocityDiff.y = 0f;

        Vector3 acceleration = velocityDiff * _acceleration;

        _rigidbody.AddForce(acceleration);
    }

    private void OnMoveTo(InputValue inputValue)
    {
        //if (!_mouseMovement) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //check if a grabable was clicked on
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, _grabableLayerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("grabable clicked on");
            //grabable clicked on
            Grabable targetGrabable = hit.collider.GetComponent<Grabable>();

            TryTargetGrabable(targetGrabable);
            return;
        }

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, _moveToLayerMask,QueryTriggerInteraction.Ignore))
        {
            //water splash at click location

            Debug.Log("Ground Clicked On");

            //if player was targetting grabable, stop
            if (!_isGrabbing && _currentTargetGrabable != null) OnUntargetCurrentGrabable(_currentTargetGrabable);
            
            //move duck to position on nav mesh
            _moveToDestination = hit.point;
            _duckAgent.SetDestination(_moveToDestination);

        }
    }

    private void OnTryGrab(InputValue inputValue)
    {
        //check if there is a target grabable and its in range
        //if true
        if(_currentTargetGrabable == null) return;

        Grab();
    }
    
    private void OnTryRelease(InputValue inputValue)
    {
        if (!_isGrabbing) return;

        UnGrab();
    }

    private void OnQuack(InputValue inputValue)
    {
         
    }

    private void OnMove(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();
        _moveInput = new Vector3(input.x, 0f, input.y);


        if (!_canMove)
        {
            _moveInput = Vector3.zero;
            return;
        }

        _moveInput = Vector3.ClampMagnitude(_moveInput, 1f);

        //Debug.Log(_moveInput);

        _hasMoveInput = _moveInput.magnitude > 0.1f;
        _moveInput = _hasMoveInput ? _moveInput : Vector3.zero;

        _localMoveInput = transform.InverseTransformDirection(_moveInput);
    }

    private void SetLookDirection(Vector3 direction)
    {
        if(!_canMove || direction.magnitude < 0.1f)
        {
            _hasTurnInput = false;
            return;
        }

        _hasTurnInput = true;
        _lookDirection = new Vector3(direction.x, 0f, direction.z).normalized;
    }

    private void TryTargetGrabable(Grabable targetGrabable)
    {
        //Mouse > checks if player is already grabbing something or is targeting a different grabable

        if(!_isGrabbing && _currentTargetGrabable == null)
        {
            //begin targeting target grabable and move to it
            OnTargetGrabable(targetGrabable);
            MoveToTargetGrabable();

            if (targetGrabable.IsPlayerNearby) Grab();

            return;
        }

        if(!_isGrabbing && _currentTargetGrabable != null)
        {
            //stop targeting current grabable and target new grabable
            OnUntargetCurrentGrabable(_currentTargetGrabable);
            OnTargetGrabable(targetGrabable);
            MoveToTargetGrabable();
            return;
        }

        if(_isGrabbing && _currentTargetGrabable == targetGrabable)
        {
            //ungrab and untarget currently grabbed grabable
            UnGrab();
            OnUntargetCurrentGrabable(_currentTargetGrabable);
            return;
        }

        if(_isGrabbing && _currentTargetGrabable != targetGrabable)
        {
            //ungrab and untarget currently grabbed grabable
            UnGrab();
            OnUntargetCurrentGrabable(_currentTargetGrabable);
            //target and move towards new grabable
            OnTargetGrabable(targetGrabable);
            MoveToTargetGrabable();
            return;
        }
    }

    private void MoveToTargetGrabable()
    {
        RaycastHit hit;

        Vector3 targetDirection = (_currentTargetGrabable.transform.position - transform.position).normalized;

        if(Physics.Raycast(transform.position, targetDirection, out hit, Mathf.Infinity, _grabableLayerMask ,QueryTriggerInteraction.Ignore))
        {
            _moveToDestination = hit.point;
            _duckAgent.SetDestination(_moveToDestination);
        }
    }

    public void OnTargetGrabable(Grabable grabableToTarget)
    {
        _currentTargetGrabable = grabableToTarget;
        _currentTargetGrabable.OnTarget();
        return;
    }

    public void OnUntargetCurrentGrabable(Grabable grabableToUntarget)
    {
        if (grabableToUntarget != _currentTargetGrabable) return;

        _currentTargetGrabable.OnUntarget();
        _currentTargetGrabable = null;
        return;
    }

    public void Grab()
    {
        Debug.Log("Grab called on character controller");
        _isGrabbing = true;
        _currentTargetGrabable.TryGrab();  
    }

    public void UnGrab()
    {
        Debug.Log("UnGrab called on Character Controller");
        _isGrabbing = false;
        _currentTargetGrabable.TryUnGrab();
    }
}
