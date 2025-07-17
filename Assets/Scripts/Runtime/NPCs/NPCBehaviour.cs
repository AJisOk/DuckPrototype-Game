using UnityEngine;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.Events;
using UnityEngine.UI;

//[RequireComponent(typeof(BoxCollider))]
//[RequireComponent(typeof(SpriteRenderer))]

public class NPCBehaviour : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected CanvasGroup _nPCCanvasGroup;
    [SerializeField] protected Animator _nPCAnimator;
    [SerializeField] protected SpriteRenderer _nPCSpriteRenderer;
    [SerializeField] protected float _animInterval = .5f;
    [SerializeField] protected Image _thoughtBubbleImage;
    [SerializeField] protected Sprite _desiredObjectSprite;
    [SerializeField] protected Sprite _correctObjectSprite;
    [SerializeField] protected Sprite _wrongObjectSprite;

    [Header("Deliverable")]
    [SerializeField] protected int _desiredDeliverableID = 0;
    [SerializeField] protected UnityEvent _onSuccesfulDelivery;

    [Header("Target Group")]
    [SerializeField] protected CinemachineTargetGroup _targetGroup;
    [SerializeField] protected float _nPCTargetGroupWeight = 2f;
    [SerializeField] protected float _nPCTargetGroupRadius = 0f;

    [Header("SFX")]
    [SerializeField] protected EventReference _defaultBeaverChitter;
    [SerializeField] protected EventReference _happyBeaverChitter;
    [SerializeField] protected EventReference _grumpyBeaverChitter;

    [Header("MoveTo Transform")]
    [SerializeField] protected Transform _moveToTransform;

    private Collider _triggerArea;
    //private SpriteRenderer _spriteRenderer;

    private DuckCharacterController _playerController;
    private NPCDeliverable _deliverable;

    private bool _hasMetPlayer = false;
    private bool _questComplete = false;
    private bool _isPlayerNearby = false;
    

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

            if(!_questComplete) _thoughtBubbleImage.sprite = _desiredObjectSprite;

            if (!_hasMetPlayer)
            {
                _hasMetPlayer = true;

                _playerController.UnGrab();
                _playerController.SetCanMove(false);
                _playerController.ForceMoveTo(_moveToTransform.position);

                StartCoroutine(FirstTimePlayerEncounter(_nPCSpriteRenderer.transform));

                return;
            }

            StartCoroutine(AddNPCToTG(_nPCSpriteRenderer.transform));
            RuntimeManager.PlayOneShot(_defaultBeaverChitter);
            _nPCAnimator.SetBool("ShowThoughtBubble", true);
        }

        if(other.gameObject.TryGetComponent<NPCDeliverable>(out _deliverable) && _hasMetPlayer)
        {
            //when deliverable enters and the player has met npc
            //add deliverable to target group?
            //check the index, if matching, npc is happy delivery is complete
            //if not matching, npc is not happy

            if(_desiredDeliverableID != _deliverable.DeliverableID)
            {
                //play wrong deliverable sfx
                //show wrong deliverable icon
                _thoughtBubbleImage.sprite = _wrongObjectSprite;
                
                _nPCAnimator.SetBool("ShowThoughtBubble", true);

                RuntimeManager.PlayOneShot(_grumpyBeaverChitter);

                return;
            }

            //play correct deliverable sfx and UI anim
            _thoughtBubbleImage.sprite = _correctObjectSprite;
            _nPCAnimator.SetBool("ShowThoughtBubble", true);

            RuntimeManager.PlayOneShot(_happyBeaverChitter);

            _questComplete = true;

            _onSuccesfulDelivery.Invoke();
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

        //play first time enter sfx
        RuntimeManager.PlayOneShot(_defaultBeaverChitter);

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
