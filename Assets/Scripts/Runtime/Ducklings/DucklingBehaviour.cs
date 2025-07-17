using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using FMODUnity;
public class DucklingBehaviour : MonoBehaviour
{
    //when duck gets close enouhg, check if anything in between duckling and duck, if not, start following duck

    [Header("Components")]
    [SerializeField] protected SphereCollider _ducklingAttractionCollider;
    [SerializeField] protected Animator _spriteAnimator;
    //[SerializeField] protected Transform _ducklingTransform;
    [SerializeField] protected NavMeshAgent _navMeshAgent;

    [Header("Quacking UI")]
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

    [Header("Player Spotted UI")]
    [SerializeField] protected float _spottedAnimDuration;
    [SerializeField] protected AnimationCurve _spottedAnimCurve;
    [SerializeField] protected CanvasGroup _spottedCG;

    [Header("Sound FX")]
    [SerializeField] protected EventReference _ducklingQuackSound;
    [SerializeField] protected EventReference _ducklingFoundSoundEvent;
    //[SerializeField] protected bool _hasPatternChallenge = false;

    [Header("VFX")]
    [SerializeField] protected GameObject _vFXToInstantiate;
    [SerializeField] protected float _vFXSpawnInterval = .2f;
    [SerializeField] protected Transform _vFXSpawnTransform;

    public bool IsLost { get => _isLost; }

    private float _vFXTimer = 0f;
    private float stoppingDistance = .6f;
    private Transform _ducklingTransform;
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

    public bool IsMoving
    {
        get => _navMeshAgent.remainingDistance > stoppingDistance && _navMeshAgent.isStopped == false;
    }

    private void Awake()
    {
        //_lostImageRectTransform = _lostQuackCG.GetComponentInChildren<RectTransform>();
        //if(_ducklingTransform == null) _ducklingTransform = GetComponentInParent<Transform>();

        _ducklingTransform = GetComponentInParent<Transform>();
        transform.rotation = Quaternion.LookRotation(Vector3.back);
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

        Debug.Log(_navMeshAgent.remainingDistance);

        if (IsMoving && _vFXTimer >= _vFXSpawnInterval)
        {
            Instantiate(_vFXToInstantiate, _vFXSpawnTransform.transform.position, Quaternion.identity);
            _vFXTimer = 0f;
        }

        _vFXTimer += Time.deltaTime;

    }

    private void OnTriggerEnter(Collider other)
    {
        //old
        //if(other.TryGetComponent<DuckMovement>(out DuckMovement duckPlayer) && _isLost)
        //{
        //    StartFollowPlayerDuck(duckPlayer);
        //}

        if(_isLost && other.TryGetComponent<DuckCharacterController>(out DuckCharacterController duckPlayer))
        {
            //duckling found by player!
            StartFollowPlayerDuck(duckPlayer);
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        _navMeshAgent.SetDestination(targetPosition);

        Vector3 facingDircetion = (targetPosition - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(facingDircetion);
    }

    public void MoveToGrabable(Vector3 targetPosition, Vector3 facingTarget)
    {
        _navMeshAgent.SetDestination(targetPosition);
        if(_navMeshAgent.remainingDistance < .5f)
        {
            Vector3 facingDirection = (facingTarget - transform.position).normalized;

            transform.rotation = Quaternion.LookRotation(facingDirection);
        }
    }

    private void OnDucklingFirstSpotted()
    {
        _hasBeenSpotted = true;

        //FOR BRIAN - Duckling first spotted event (on duckling)

        StartCoroutine(PlayerDuckSpotted());
    }

    public void TryFocusedQuack()
    {
        if (_isQuacking) return;
        StartCoroutine(LostQuackAnim());
    }

    public void TryMutedQuack()
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
            StartCoroutine(MutedLostQuackAnim());

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

    //old original quack function
    //public void TryQuack()
    //{
    //    if (_isLost)
    //    {
    //        //play sound and trigger ui
    //        if (_isQuacking)
    //        {
    //            //StopCoroutine(LostQuackAnim());
    //            //_isQuacking = false;
    //            //_lostQuackCG.alpha = 0f;
    //            return;
    //        }

    //        //StopCoroutine(LostQuackAnim());
    //        StartCoroutine(LostQuackAnim());
    //    }
    //    else
    //    {
    //        //play kinder sound and animation
    //        if (_isQuacking)
    //        {
    //            //StopCoroutine(HappyQuackAnim());
    //            //_isQuacking = false;
    //            //_happyQuackCG.alpha = 0f;
    //            return;
    //        }

    //        StartCoroutine(HappyQuackAnim());
    //    }
    //}

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

        //FOR BRIAN duckling quack wehn it is closest to the player
        RuntimeManager.PlayOneShot(_ducklingQuackSound);

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

    private IEnumerator MutedLostQuackAnim()
    {
        yield return new WaitForSeconds(_lostQuackAnimDelay);

        //float timer = 0f;
        _isQuacking = true;

        //FOR BRIAN queiter duckling quack for when its not the closest to the player
        RuntimeManager.PlayOneShot(_ducklingQuackSound);

        //while (timer <= _lostQuackAnimDuration)
        //{
        //    _lostQuackCG.alpha = _quackAnimCurve.Evaluate(timer);

        //    timer += Time.deltaTime;

        //    yield return null;
        //}

        //_lostQuackCG.alpha = 0f;
        _isQuacking = false;
        yield return null;
    }

    private IEnumerator HappyQuackAnim()
    {
        float timer = 0f;
        _isQuacking = true;

        //FOR BRIAN duckling quacking when IS collected here
        RuntimeManager.PlayOneShot(_ducklingQuackSound);

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

    private void StartFollowPlayerDuck(DuckCharacterController duckPlayer)
    {
        //duckling has been found!!
        _isLost = false;

        //play celebratory feedback on duckling here
        //TODO create a short sequence when we discover a duckling

        RuntimeManager.PlayOneShot(_ducklingFoundSoundEvent);

        //old
        //duckPlayer.AddDuckling(this.GetComponentInParent<NavMeshAgent>());

        duckPlayer.DucklingStartsFollowing(this);
        _ducklingAttractionCollider.enabled = false;
    }
}
