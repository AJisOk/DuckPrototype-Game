using Unity.Cinemachine;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
            //_targetGroup.AddMember(transform, _areaTGWeight, _areaTGRadius);

            StartCoroutine(AddAreaToTG(transform));

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent<DuckCharacterController>(out _playerDuck))
        {
            //_targetGroup.RemoveMember(transform);
            StartCoroutine(RemoveAreaFromTG(transform));
        }

    }

    private IEnumerator AddAreaToTG(Transform areaToAdd)
    {
        float timer = 0f;

        _targetGroup.AddMember(areaToAdd, 0f, 0f);
        int areaIndex = _targetGroup.FindMember(areaToAdd);

        AnimationCurve animCurveW = AnimationCurve.Linear(0f, 0f, 1f, _areaTGWeight);
        AnimationCurve animCurveR = AnimationCurve.Linear(0f, 0f, 1f, _areaTGRadius);

        while (timer < 1f)
        {
            _targetGroup.Targets[areaIndex].Weight = animCurveW.Evaluate(timer);
            _targetGroup.Targets[areaIndex].Radius = animCurveR.Evaluate(timer);

            timer += Time.deltaTime;

            yield return null;
        }

        _targetGroup.Targets[areaIndex].Weight = _areaTGWeight;
        _targetGroup.Targets[areaIndex].Radius = _areaTGRadius;

        yield return null;
    }

    private IEnumerator RemoveAreaFromTG(Transform areaToAdd)
    {
        float timer = 0f;
        int areaIndex = _targetGroup.FindMember(areaToAdd);
        Debug.Log(areaIndex);

        AnimationCurve animCurveW = AnimationCurve.Linear(0f, _areaTGWeight, 1f, 0f);
        AnimationCurve animCurveR = AnimationCurve.Linear(0f, _areaTGRadius, 1f, 0f);

        while (timer < 1f)
        {
            _targetGroup.Targets[areaIndex].Weight = animCurveW.Evaluate(timer);
            _targetGroup.Targets[areaIndex].Radius = animCurveR.Evaluate(timer);

            timer += Time.deltaTime;

            yield return null;
        }
        _targetGroup.Targets[areaIndex].Weight = 0f;
        _targetGroup.Targets[areaIndex].Radius = 0f;

        yield return new WaitForEndOfFrame();

        _targetGroup.RemoveMember(areaToAdd);

        yield return null;
    }
}
