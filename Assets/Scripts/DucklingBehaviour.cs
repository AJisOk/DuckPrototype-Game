using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class DucklingBehaviour : MonoBehaviour
{
    //when duck gets close enouhg, check if anything in between duckling and duck, if not, start following duck

    [SerializeField] protected SphereCollider _ducklingAttractionCollider;
    [SerializeField] protected CanvasGroup _lostQuackCG;
    [SerializeField] protected CanvasGroup _happyQuackCG;
    [SerializeField] protected AnimationCurve _quackAnimCurve;
    [SerializeField] protected float _lostQuackAnimDuration;
    [SerializeField] protected float _lostQuackAnimDelay = .5f;
    [SerializeField] protected float _happyQuackAnimDuration;
    [SerializeField] protected Vector3 _lostImageLocalOffset;
    [SerializeField] protected RectTransform _lostImageRectTransform;
    [SerializeField] protected float _offScreenBufferDistanceX = 120f;
    [SerializeField] protected float _offScreenBufferDistanceY = 70f;
    [SerializeField] protected Animator _spriteAnimator;
    [SerializeField] protected Transform _ducklingTransform;
    [SerializeField] protected float _spottedAnimDuration;
    [SerializeField] protected AnimationCurve _spottedAnimCurve;
    [SerializeField] protected CanvasGroup _spottedCG;

    //[SerializeField] protected bool _hasPatternChallenge = false;

    [Header("Pattern Challenge")]
    [SerializeField] protected PatternChallengeHandler _patternChallengeHandler;


    private bool _isLost = true;
    private bool _isQuacking = false;
    private bool _hasBeenSpotted = false;
    private Vector3 _canvasPositionScreenPoint;
    private Vector3 _cappedCanvasScreenPosition;
    private Vector3 _ducklingPositionScreenPoint;


    private bool _isDucklingOnScreen
    {
        get => _ducklingPositionScreenPoint.x > 0 &&
            _ducklingPositionScreenPoint.x < Screen.width &&
            _ducklingPositionScreenPoint.y > 0 &&
            _ducklingPositionScreenPoint.y < Screen.height;
    }
    private bool isQuackCanvasOffScreen 
    { 
        get => _canvasPositionScreenPoint.x <= 0 ||
            _canvasPositionScreenPoint.x >= Screen.width ||
            _canvasPositionScreenPoint.y <= 0 ||
            _canvasPositionScreenPoint.y >= Screen.height;
    }

    private void Awake()
    {
        //_lostImageRectTransform = _lostQuackCG.GetComponentInChildren<RectTransform>();
        //if(_ducklingTransform == null) _ducklingTransform = GetComponentInParent<Transform>();

        

    }

    private void Update()
    {
        _ducklingPositionScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
        if(!_hasBeenSpotted && _isDucklingOnScreen) OnDucklingFirstSpotted();


        _canvasPositionScreenPoint = Camera.main.WorldToScreenPoint((transform.position + _lostImageLocalOffset));
        _cappedCanvasScreenPosition = _canvasPositionScreenPoint;

        if (isQuackCanvasOffScreen)
        {
            _cappedCanvasScreenPosition.x = (_cappedCanvasScreenPosition.x <= 0) ? 0f + _offScreenBufferDistanceX : _cappedCanvasScreenPosition.x;
            _cappedCanvasScreenPosition.x = (_cappedCanvasScreenPosition.x >= Screen.width) ? Screen.width - _offScreenBufferDistanceX : _cappedCanvasScreenPosition.x;
            _cappedCanvasScreenPosition.y = (_cappedCanvasScreenPosition.y <= 0) ? 0f + _offScreenBufferDistanceY : _cappedCanvasScreenPosition.y;
            _cappedCanvasScreenPosition.y = (_cappedCanvasScreenPosition.y >= Screen.height) ? Screen.height - _offScreenBufferDistanceY : _cappedCanvasScreenPosition.y;

            _lostImageRectTransform.position = _cappedCanvasScreenPosition;
            //_cGWorldPosition = Camera.main.ScreenToWorldPoint(_cappedCanvasScreenPosition);
            //_lostCanvasRectTransform.localPosition = new Vector3(_lostCanvasRectTransform.localPosition.x, _lostCanvasRectTransform.localPosition.y, 0f);
        }
        else
        {
            _lostImageRectTransform.position = _cappedCanvasScreenPosition;
        }


        //if duckling is on screen > move the image to the screen position of the world point above the ducklings head
        //if duckling is OFF screen > move the image to the screen position closest to the world point position of the duckling

    }

    private void LateUpdate()
    {
        float normalizedEulerAngle = Mathf.InverseLerp(0f, 360f, _ducklingTransform.rotation.eulerAngles.y);

        _spriteAnimator.SetFloat("NormalizedEulerAngle", normalizedEulerAngle);

    }

    private void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter Called on: " + this.gameObject.name);

        if(other.TryGetComponent<DuckMovement>(out DuckMovement duckPlayer) && _isLost)
        {
            StartFollowPlayerDuck(duckPlayer);
        }
    }

    private void OnDucklingFirstSpotted()
    {
        _hasBeenSpotted = true;

        //FOR BRIAN - Duckling first spotted event (on duckling)

        StartCoroutine(PlayerDuckSpotted());
    }

    public void TryQuack()
    {
        if (_isLost)
        {
            //play sound and trigger ui
            if (_isQuacking)
            {
                //StopCoroutine(LostQuackAnim());
                //_isQuacking = false;
                //_lostQuackCG.alpha = 0f;
                return;
            }

            //StopCoroutine(LostQuackAnim());
            StartCoroutine(LostQuackAnim());
        }
        else
        {
            //play kinder sound and animation
            if (_isQuacking)
            {
                //StopCoroutine(HappyQuackAnim());
                //_isQuacking = false;
                //_happyQuackCG.alpha = 0f;
                return;
            }

            StartCoroutine(HappyQuackAnim());
        }
    }

    private IEnumerator PlayerDuckSpotted()
    {
        float timer = 0f;

        while (timer < _spottedAnimDuration)
        {
            _spottedCG.alpha = _spottedAnimCurve.Evaluate(timer);
            timer+= Time.deltaTime;
            yield return null;
        }

        _spottedCG.alpha = 0f;

        yield return null;
    }

    private IEnumerator LostQuackAnim()
    {
        yield return new WaitForSeconds(_lostQuackAnimDelay);

        float timer = 0f;
        _isQuacking = true;

        //FOR BRIAN duckling quacking when not collected here

        while (timer <= _lostQuackAnimDuration)
        {
            _lostQuackCG.alpha = _quackAnimCurve.Evaluate(timer);

            timer += Time.deltaTime;

            yield return null;
        }

        _lostQuackCG.alpha = 0f;
        _isQuacking = false;
        yield return null;
    }

    private IEnumerator HappyQuackAnim()
    {
        float timer = 0f;
        _isQuacking = true;

        //FOR BRIAN duckling quacking when IS collected here

        while (timer <= _happyQuackAnimDuration)
        {
            _happyQuackCG.alpha = _quackAnimCurve.Evaluate(timer);

            timer += Time.deltaTime;

            yield return null;
        }

        _happyQuackCG.alpha = 0f;
        _isQuacking = false;
        yield return null;
    }

    //private void StartPatternChallenge(DuckMovement duckPlayer)
    //{
    //    duckPlayer.StartPatternChallenge(_patternChallengeHandler);
    //    //face player

    //    //show canvas with required pattern
    //    _patternChallengeHandler.StartChallenge(transform);

    //    //play anim


    //}

    private void StartFollowPlayerDuck(DuckMovement duckPlayer)
    {
        _isLost = false;
        duckPlayer.AddDuckling(this.GetComponentInParent<NavMeshAgent>());

        _ducklingAttractionCollider.enabled = false;
    }
}
