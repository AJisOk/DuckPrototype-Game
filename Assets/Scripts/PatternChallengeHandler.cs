using Unity.Cinemachine;
using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;


public class PatternChallengeHandler : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _challengeCG;
    [SerializeField] protected RectTransform _challengeRectTransform;
    [SerializeField] protected float _cRectTranformXOffset = 200f;
    [SerializeField] protected float _cRectTranformYOffset = 320f;
    [SerializeField] protected CinemachineCamera _cinemachineCamera;
    [SerializeField] protected CinemachineTargetGroup _cinemachinTargetGroup;
    [SerializeField] protected float _baseOrthoSize = 6f;
    [SerializeField] protected float _focusedOrthoSize = 4f;
    [SerializeField] protected List<int> _solutionPattern;

    private bool _isChallengeLive = false;
    private CinemachineGroupFraming _cinemachineGroupFramingComponent;

    private void Awake()
    {
        if(_challengeRectTransform == null) _challengeRectTransform = GetComponent<RectTransform>();
        if(_cinemachineGroupFramingComponent == null) _cinemachineGroupFramingComponent = _cinemachineCamera.GetComponent<CinemachineGroupFraming>();

    }

    private void Update()
    {

    }


    public void StartChallenge(Transform ducklingWorldTransform)
    {
        _isChallengeLive = true;

        _cinemachinTargetGroup.AddMember(ducklingWorldTransform, 1f, 0f);

        _cinemachineGroupFramingComponent.OrthoSizeRange.Set(_focusedOrthoSize, 20f);

        _challengeRectTransform.position = Camera.main.WorldToScreenPoint(ducklingWorldTransform.position);
        _challengeRectTransform.position = new Vector3(
            _challengeRectTransform.position.x + _cRectTranformXOffset,
            _challengeRectTransform.position.y + _cRectTranformYOffset,
            _challengeRectTransform.position.z);

        _challengeCG.alpha = 1f;


    }
}
