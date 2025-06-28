using Unity.Cinemachine;
using UnityEngine;

public class TargetGroupAreaToFrame : MonoBehaviour
{
    [SerializeField] protected CinemachineTargetGroup _targetGroup;
    [SerializeField] protected float _areaTGWeight = 1f;
    [SerializeField] protected float _areaTGRadius = 10f;
    //[SerializeField] protected FMODUnity.EventReference _musicSound;

    private Collider _areaTrigger;
    private DuckCharacterController _playerDuck;

    private void Awake()
    {
        _areaTrigger = GetComponent<Collider>();

        if (_targetGroup == null) _targetGroup = (CinemachineTargetGroup)FindFirstObjectByType(typeof(CinemachineTargetGroup));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<DuckCharacterController>(out _playerDuck))
        {
            //duck entered area > add desired area to frame
            _targetGroup.AddMember(transform, _areaTGWeight, _areaTGRadius);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent<DuckCharacterController>(out _playerDuck))
        {
            _targetGroup.RemoveMember(transform);
        }
    }
}
