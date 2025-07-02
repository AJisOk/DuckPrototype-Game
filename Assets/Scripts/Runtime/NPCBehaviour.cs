using UnityEngine;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

//[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]

public class NPCBehaviour : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected CanvasGroup _nPCCanvasGroup;
    [SerializeField] protected Animator _nPCAnimator;
    [SerializeField] protected SpriteRenderer _nPCSpriteRenderer;
    [SerializeField] protected float _animInterval = .5f;

    [Header("Target Group")]
    [SerializeField] protected CinemachineTargetGroup _targetGroup;
    [SerializeField] protected float _nPCTargetGroupWeight = 2f;
    [SerializeField] protected float _nPCTargetGroupRadius = 0f;

    [Header("SFX")]
    [SerializeField] protected EventReference _beaverChitter;

    [Header("MoveTo Transform")]
    [SerializeField] protected Transform _moveToTransform;

    private Collider _triggerArea;
    //private SpriteRenderer _spriteRenderer;

    private DuckCharacterController _playerController;

    private bool _hasMetPlayer = false;
    

    private void Awake()
    {
        _triggerArea = GetComponent<Collider>();
        //_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<DuckCharacterController>(out _playerController))
        {
            //when player enters trigger
            //turn off input, move to center of trigger and face npc
            //add npc to target group at a heavier weight than the player

            //on trigger exit, remove npc from target group

            if (_hasMetPlayer)
            {
                StartCoroutine(AddNPCToTG(_nPCSpriteRenderer.transform));
                _nPCAnimator.SetBool("ShowThoughtBubble", true);

                return;
            }

            _hasMetPlayer = true;

            _playerController.SetCanMove(false);
            _playerController.ForceMoveTo(_moveToTransform.position);

            StartCoroutine(FirstTimePlayerEncounter(_nPCSpriteRenderer.transform));

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<DuckCharacterController>(out _playerController))
        {
            StartCoroutine(RemoveNPCFromTG(_nPCSpriteRenderer.transform));

            _nPCAnimator.SetBool("ShowThoughtBubble", false);
        }
    }

    private IEnumerator FirstTimePlayerEncounter(Transform areaToAdd)
    {
        float timer = 0f;

        _targetGroup.AddMember(areaToAdd, 0f, 0f);
        int areaIndex = _targetGroup.FindMember(areaToAdd);

        AnimationCurve animCurveW = AnimationCurve.Linear(0f, 0f, 1f, _nPCTargetGroupWeight*2f);
        AnimationCurve animCurveR = AnimationCurve.Linear(0f, 0f, 1f, _nPCTargetGroupRadius);

        while (timer < 1f)
        {
            _targetGroup.Targets[areaIndex].Weight = animCurveW.Evaluate(timer);
            _targetGroup.Targets[areaIndex].Radius = animCurveR.Evaluate(timer);

            timer += Time.deltaTime;

            yield return null;
        }

        _targetGroup.Targets[areaIndex].Weight = _nPCTargetGroupWeight*2f;
        _targetGroup.Targets[areaIndex].Radius = _nPCTargetGroupRadius;

        yield return new WaitForSeconds(1f);

        _playerController.SetFacingTarget(_nPCSpriteRenderer.transform.position);

        //play animation on npc
        //when anim finishes, set a running thought bubble that appears whenm the player re enters
        //let player move

        _nPCAnimator.SetBool("ShowThoughtBubble", true);

        yield return new WaitForSeconds(_animInterval);

        timer = 0f;

        AnimationCurve animCurveW2 = AnimationCurve.Linear(0f, _nPCTargetGroupWeight*2f, 1f, _nPCTargetGroupWeight);

        while (timer < 1f)
        {
            _targetGroup.Targets[areaIndex].Weight = animCurveW2.Evaluate(timer);

            timer += Time.deltaTime;

            yield return null;
        }

        _targetGroup.Targets[areaIndex].Weight = _nPCTargetGroupWeight;


        _playerController.SetCanMove(true);

        yield return null;
    }

    private IEnumerator AddNPCToTG(Transform areaToAdd)
    {
        float timer = 0f;

        _targetGroup.AddMember(areaToAdd, 0f, 0f);
        int areaIndex = _targetGroup.FindMember(areaToAdd);

        AnimationCurve animCurveW = AnimationCurve.Linear(0f, 0f, 1f, _nPCTargetGroupWeight);
        AnimationCurve animCurveR = AnimationCurve.Linear(0f, 0f, 1f, _nPCTargetGroupRadius);

        while (timer < 1f)
        {
            _targetGroup.Targets[areaIndex].Weight = animCurveW.Evaluate(timer);
            _targetGroup.Targets[areaIndex].Radius = animCurveR.Evaluate(timer);

            timer += Time.deltaTime;

            yield return null;
        }

        _targetGroup.Targets[areaIndex].Weight = _nPCTargetGroupWeight;
        _targetGroup.Targets[areaIndex].Radius = _nPCTargetGroupRadius;

        yield return null;
    }

    private IEnumerator RemoveNPCFromTG(Transform areaToAdd)
    {
        float timer = 0f;
        int areaIndex = _targetGroup.FindMember(areaToAdd);
        Debug.Log(areaIndex);

        AnimationCurve animCurveW = AnimationCurve.Linear(0f, _nPCTargetGroupWeight, 1f, 0f);
        AnimationCurve animCurveR = AnimationCurve.Linear(0f, _nPCTargetGroupRadius, 1f, 0f);

        while (timer < 1f)
        {
            _targetGroup.Targets[areaIndex].Weight = animCurveW.Evaluate(timer);
            _targetGroup.Targets[areaIndex].Radius = animCurveR.Evaluate(timer);

            timer += Time.deltaTime;

            yield return null;
        }
        _targetGroup.Targets[areaIndex].Weight = 0f;
        _targetGroup.Targets[areaIndex].Radius = 0f;

        _targetGroup.RemoveMember(areaToAdd);

        yield return null;
    }
}
