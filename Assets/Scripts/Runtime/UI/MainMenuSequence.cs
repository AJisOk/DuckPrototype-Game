using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class MainMenuSequence : MonoBehaviour
{
    //this script handles the title screen animation and transition to main menu canvas

    [Header("Title Screen")]
    [SerializeField] private Canvas _titleScreenCanvas;
    [SerializeField] private Image _transitionFill;
    [SerializeField] private CanvasGroup _clickAnywhereCG;
    [SerializeField] private AnimationCurve _transitionFillCurve;
    [SerializeField] private AnimationCurve _textFlashCurve;

    [Header("Main Menu")]
    [SerializeField] private Canvas _mainMenuCanvas;

    [Header("Sound FX")]
    [SerializeField] private EventReference _titleScreenContinueSound;
    [SerializeField] private EventReference _clickSound;

    private bool _hasPlayerClicked = false;
    private bool _acceptingInput = false;

    private Canvas _transitionCanvas;

    private void Awake()
    {
        _transitionCanvas = _transitionFill.GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        StartCoroutine(GameOpen());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
     
        if(_acceptingInput && !_hasPlayerClicked && Input.anyKeyDown)
        {
            _acceptingInput = false;
            _hasPlayerClicked = true;

            RuntimeManager.PlayOneShot(_titleScreenContinueSound);
        }

    }

    private IEnumerator GameOpen()
    {
        float timer = 0f;

        while(timer < 1f)
        {
            _transitionFill.fillAmount = _transitionFillCurve.Evaluate(timer);

            timer += Time.deltaTime;
            yield return null;
        }
        _transitionFill.fillAmount = 0f;
        _transitionCanvas.gameObject.SetActive(false);
        timer = 0f;

        yield return new WaitForSeconds(.2f);

        _acceptingInput = true;

        while (!_hasPlayerClicked)
        {
            if (timer >= 1f) timer = 0f;

            _clickAnywhereCG.alpha = _textFlashCurve.Evaluate(timer);

            timer += Time.deltaTime;
            yield return null;
        }
        _clickAnywhereCG.alpha = 0f;

        yield return new WaitForSeconds(.2f);

        timer = 1f;
        _transitionCanvas.gameObject.SetActive(true);
        while (timer > 0f)
        {
            _transitionFill.fillAmount = _transitionFillCurve.Evaluate(timer);

            timer -= Time.deltaTime;
            yield return null;
        }
        _transitionFill.fillAmount = 1f;
        timer = 0f;

        yield return new WaitForSeconds(.2f);

        _titleScreenCanvas.gameObject.SetActive(false);
        _mainMenuCanvas.gameObject.SetActive(true);

        while (timer < 1f)
        {
            _transitionFill.fillAmount = _transitionFillCurve.Evaluate(timer);

            timer += Time.deltaTime;
            yield return null;
        }
        _transitionFill.fillAmount = 0f;
        _transitionCanvas.gameObject.SetActive(false);
        timer = 0f;

        yield return null;
    }

    
}
