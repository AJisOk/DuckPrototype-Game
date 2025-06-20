using Unity.Cinemachine;
using UnityEngine;

public class TargetGroupAreaToFrame : MonoBehaviour
{
    [SerializeField] protected CinemachineTargetGroup _targetGroup;
    [SerializeField] protected float _areaTGWeight = 1f;
    [SerializeField] protected float _areaTGRadius = 10f;


    private Collider _areaTrigger;
    private DuckMovement _duckMovement;

    private void Awake()
    {
        _areaTrigger = GetComponent<Collider>();

        if (_targetGroup == null) _targetGroup = (CinemachineTargetGroup)FindFirstObjectByType(typeof(CinemachineTargetGroup));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<DuckMovement>(out _duckMovement))
        {
            //duck entered area > add desired area to frame
            _targetGroup.AddMember(transform, _areaTGWeight, _areaTGRadius);


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent<DuckMovement>(out _duckMovement))
        {
            _targetGroup.RemoveMember(transform);
        }
    }
}
