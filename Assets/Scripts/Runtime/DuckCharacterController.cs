using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.Cinemachine;

public class DuckCharacterController : MonoBehaviour
{
    //Character controller will handle input and movement
    //2 input modes - moveto and directional
    //1 input actions

    [Header("Components")]
    [SerializeField] protected NavMeshAgent _duckAgent;
    [SerializeField] protected Animator _spriteAnimator;
    [SerializeField] protected DuckQuackHandler _quackHandler;

    [Header("Movement")]
    [SerializeField] protected LayerMask _moveToLayerMask;
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _acceleration = 10f;
    [SerializeField] protected float _turnSpeed = 5f;
    [SerializeField] protected float _stoppingDistance = .2f;
    //[SerializeField] protected bool _mouseMovement = false;

    [Header("Camera")]
    [SerializeField] protected CinemachineTargetGroup _targetGroup;
    [SerializeField] protected float _ducklingTGWeight = 1f;
    [SerializeField] protected float _ducklingTGRadius = 0f;

    [Header("Grabbing + Pulling")]
    [SerializeField] protected LayerMask _grabableLayerMask;
    [SerializeField] protected int _grabableLayerIndex;

    [Header("Ducklings")]
    [SerializeField] protected List<DucklingBehaviour> _ducklingsFollowing = new List<DucklingBehaviour>();
    [SerializeField] protected float _ducklingFollowIntervalTime = .5f;

    //private variables
    private PlayerInput _playerInput;

    private List<Vector3> _ducklingNextFollowPositions = new List<Vector3>();
    private Vector3 _moveToDestination;
    private Grabable _currentHighlightedGrabable = null;
    private Grabable _currentTargetGrabable = null;

    private float _timer = 0f;
    private bool _isGrabbing = false;
    private bool _canMove = true;
    private bool _hasMoveInput = false;

    public int DucklingsFollowingCount { get => _ducklingsFollowing.Count; }
    public bool IsMoving
    {
        get => (_hasMoveInput || (_duckAgent.remainingDistance > _stoppingDistance && _duckAgent.isStopped == false));
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

    }

    private void LateUpdate()
    {
        //update duck sprite based on rotation
        float normalizedEulerAngle = Mathf.InverseLerp(0f, 360f, transform.rotation.eulerAngles.y);
        _spriteAnimator.SetFloat("NormalizedEulerAngle", normalizedEulerAngle);

    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer >= _ducklingFollowIntervalTime)
        {
            UpdateDucklingFollowPositions();

            if (_isGrabbing) DucklingsSwarmGrabable();
        }

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

    private void OnQuack(InputValue inputValue)
    {
        _quackHandler.Quack();
    }

    private void TryTargetGrabable(Grabable targetGrabable)
    {
        //Mouse > checks if player is already grabbing something or is targeting a different grabable

        if(!_isGrabbing && _currentTargetGrabable == null)
        {
            //begin targeting target grabable and move to it
            OnTargetGrabable(targetGrabable);
            MoveToTargetGrabable();
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

    public void UnableToGrab()
    {
        UnGrab();
    }

    public void DucklingsSwarmGrabable()
    {
        for (int i = 0; i < _ducklingsFollowing.Count; i++)
        {
            _ducklingsFollowing[i].MoveToGrabable(_currentTargetGrabable.DucklingGrabTransforms[i].position,
                _currentTargetGrabable.transform.position);
        }
    }

    public void DucklingStartsFollowing(DucklingBehaviour ducklingToAdd)
    {
        _ducklingsFollowing.Add(ducklingToAdd);

        _targetGroup.AddMember(ducklingToAdd.transform,_ducklingTGWeight, _ducklingTGRadius);
    }

    private void UpdateDucklingFollowPositions()
    {
        _timer = 0f;

        //update duckling follow positions list
        _ducklingNextFollowPositions.Insert(0, _duckAgent.transform.position);
        if (_ducklingNextFollowPositions.Count > _ducklingsFollowing.Count) _ducklingNextFollowPositions.RemoveAt(_ducklingsFollowing.Count);

        if (!IsMoving || _isGrabbing) return;

        for (int i = 0; i < _ducklingsFollowing.Count; i++)
        {
            _ducklingsFollowing[i].MoveTo(_ducklingNextFollowPositions[i]);
        }

    }
}
