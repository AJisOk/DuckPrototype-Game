using System;
using System.Collections;
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
    [SerializeField] protected Vector3 _defaultCGPostion;


    private bool _isLost = true;
    private bool _isQuacking = false;
    private Vector3 _canvasPositionScreenPoint;
    private Vector3 _cappedCanvasScreenPosition;
    private Vector3 _cGWorldPosition;
    private RectTransform _lostCanvasRectTransform;

    private bool isQuackCanvasOffScreen 
    { 
        get => _canvasPositionScreenPoint.x <= 0 ||
            _canvasPositionScreenPoint.x >= Screen.width ||
            _canvasPositionScreenPoint.y <= 0 ||
            _canvasPositionScreenPoint.y >= Screen.height;
    }

    private void Awake()
    {
        _lostCanvasRectTransform = _lostQuackCG.GetComponent<RectTransform>();
    }

    private void Update()
    {
        _canvasPositionScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
        _cappedCanvasScreenPosition = _canvasPositionScreenPoint;

        if (isQuackCanvasOffScreen)
        {
            _cappedCanvasScreenPosition.x = (_cappedCanvasScreenPosition.x <= 0) ? 0f : _cappedCanvasScreenPosition.x;
            _cappedCanvasScreenPosition.x = (_cappedCanvasScreenPosition.x >= Screen.width) ? Screen.width : _cappedCanvasScreenPosition.x;
            _cappedCanvasScreenPosition.y = (_cappedCanvasScreenPosition.y <= 0) ? 0f : _cappedCanvasScreenPosition.y;
            _cappedCanvasScreenPosition.y = (_cappedCanvasScreenPosition.y >= Screen.height) ? Screen.height : _cappedCanvasScreenPosition.y;

            _cGWorldPosition = Camera.main.ScreenToWorldPoint(_cappedCanvasScreenPosition);
            _lostCanvasRectTransform.position = _cGWorldPosition;
            //_lostCanvasRectTransform.localPosition = new Vector3(_lostCanvasRectTransform.localPosition.x, _lostCanvasRectTransform.localPosition.y, 0f);
        }
        else
        {
            _lostCanvasRectTransform.localPosition = _defaultCGPostion;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter Called on: " + this.gameObject.name);

        if(other.TryGetComponent<DuckMovement>(out DuckMovement duckPlayer) &&
            _isLost)
        {
            _isLost = false;
            duckPlayer.AddDuckling(this.GetComponentInParent<NavMeshAgent>());

            _ducklingAttractionCollider.enabled = false;
        }
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

    private IEnumerator LostQuackAnim()
    {
        yield return new WaitForSeconds(_lostQuackAnimDelay);

        float timer = 0f;
        _isQuacking = true;

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
}
