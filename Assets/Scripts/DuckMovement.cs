using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;


public class DuckMovement : MonoBehaviour
{
    [Header("Tweakables")]
    [SerializeField] protected float _followIntervalTime = .5f;
    [SerializeField] protected float _maxDucklings = 5;
    [SerializeField] protected bool _instantiatePrefabOnMove = false;
    [SerializeField] protected float _remainingDistanceStopThreshold = 0.2f;

    [Header("Core Pieces")]
    [SerializeField] protected LayerMask _pullableLayerMask;
    [SerializeField] protected int _pullableLayer = 6;
    [SerializeField] protected NavMeshAgent _duckAgent;
    [SerializeField] protected GameObject _gOToInstantiate;
    //[SerializeField] protected float _currentDucklings = 1;
    [SerializeField] protected List<NavMeshAgent> _ducklingAgents = new List<NavMeshAgent>();
    [SerializeField] protected List<Vector3> _ducklingNextPositions = new List<Vector3>();

    private Transform _targetLocation;
    private float _timer;
    private bool _isMoving = false;
    private bool _isPulling = false;
    private Pullable _currentTargetPullable = null;

    public bool IsPulling { get => _isPulling; set => _isPulling = value; }

    private void Awake()
    {
        if (_duckAgent == null)
        {
            _duckAgent = GetComponent<NavMeshAgent>();
        }
    }

    private void FixedUpdate()
    {
        print("Duck isMoving: " + _isMoving);

        if(_timer >= _followIntervalTime)
        {
            _timer = 0f;

            //used to visualize duck's trail
            if(_instantiatePrefabOnMove) Instantiate(_gOToInstantiate, _duckAgent.nextPosition, Quaternion.identity);

            //inserts ducks nextposition at the top of the ducklings nextposition list
            _ducklingNextPositions.Insert(0, _duckAgent.nextPosition);
            //removes the last position from the ducklings nextposition list
            if(_ducklingNextPositions.Count > _ducklingAgents.Count) _ducklingNextPositions.RemoveAt(_ducklingAgents.Count);

            //if (_ducklingNextPositions.Count < _ducklingAgents.Count) return;

            if(_isMoving)
            {
                for (int i = 0; i < _ducklingAgents.Count; i++)
                {   
                    _ducklingAgents[i].SetDestination(_ducklingNextPositions[i]);
                }

            }

            if (_duckAgent.remainingDistance <= _remainingDistanceStopThreshold) _isMoving = false;
        }
        _timer += Time.deltaTime;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_currentTargetPullable == null) return;
        if (collision.gameObject.layer != _pullableLayer) return;

        if (!_isPulling)
        {
            StopMovement();
            _duckAgent.updateRotation = false;
        }
        
        _isPulling = true;

    }

    //on right click, tries to move to target location if its on navmesh
    public void OnMoveTo(InputValue value)
    {
        RaycastHit hit;
        if(_currentTargetPullable!=null && !_isPulling) OnUntargetPullable();

        //casts ray in game world based on mouse position
        //if there is a hit, move to the point that was hit
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            _duckAgent.SetDestination(hit.point);
            _isMoving = true;
        }

    }

    private void StopMovement()
    {
        _duckAgent.ResetPath();
    }

    public void AddDuckling(NavMeshAgent ducklingAgentToAdd)
    {
        _ducklingAgents.Add(ducklingAgentToAdd);
    }

    public void OnTryGrab(InputValue value)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, _pullableLayerMask))
        {
            print("pullable clicked on");

            TryTargetPullable(hit.collider.GetComponent<Pullable>());

            //_currentTargetPullable = hit.collider.GetComponent<Pullable>();
            //OnTargetPullable();
            return;
        }

        print("no pullable found");
    }

    private void TryTargetPullable(Pullable targetPullable)
    {
        if (!_isPulling && _currentTargetPullable == null)
        {
            OnTargetPullable(targetPullable);
            return;
        }

        if (!_isPulling && _currentTargetPullable != null)
        {
            OnUntargetPullable();
            OnTargetPullable(targetPullable);
            return;
        }

        if(_isPulling && _currentTargetPullable == targetPullable)
        {
            //stop pulling + targeting current target
            OnUntargetPullable();
            _isPulling = false;
            return;
        }

        if(_isPulling && _currentTargetPullable != targetPullable)
        {
            //stop pulling + targeting current target and start targeting new target pullable
            OnUntargetPullable();
            _isPulling = false;
            OnTargetPullable(targetPullable);
            return;
        }
    }

    private void OnTargetPullable(Pullable targetPullable)
    {
        print("OnTargetPullable called");

        _currentTargetPullable = targetPullable;


        //if (_currentTargetPullable.IsTargeted)
        //{
        //    //OnUntargetPullable();
        //    //TryUntargetPullable();
        //    return;
        //}

        _currentTargetPullable.OnTargeted();

        RaycastHit hit;

        Vector3 targetDirection = (_currentTargetPullable.transform.position - transform.position).normalized;

        if(Physics.Raycast(transform.position, targetDirection, out hit, Mathf.Infinity) &&
            hit.collider.gameObject.layer == _pullableLayer )
        {
            _duckAgent.SetDestination(hit.point);
            
        }

    }

    private void OnUntargetPullable()
    {
        _currentTargetPullable.OnUntargeted();
        _currentTargetPullable = null;
        _duckAgent.updateRotation = true;
        return;
    }

    public void OnTogglePlayerTrail(InputValue value)
    {
        _instantiatePrefabOnMove = !_instantiatePrefabOnMove;
    }

    

}
