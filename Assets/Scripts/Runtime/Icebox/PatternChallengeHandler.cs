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

    [Header("Player Pattern UI")]
    [SerializeField] protected CanvasGroup _playerAttemptCG;
    [SerializeField] protected List<TextMeshProUGUI> _playerAttemptTextList;

    private bool _isChallengeLive = false;
    private bool _isAcceptingAttempt = false;
    private CinemachineGroupFraming _cinemachineGroupFramingComponent;
    private List<int> _attemptPattern = new List<int>();

    public bool IsAcceptingAttempts { get => _isAcceptingAttempt; }

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
            _challengeRectTransform.position.z
            );

        _challengeCG.alpha = 1f;
        _playerAttemptCG.alpha = 1f;

        _isAcceptingAttempt = true;
    }

    public void AddNoteToAttempt(PatternChallengeNote noteToAdd)
    {
        Debug.Log("AddNoteToAttempt called");

        _attemptPattern.Add(noteToAdd.NoteID);

        Debug.Log(noteToAdd.NoteID + " Added to attempt.");
        Debug.Log("Current attempt count: " + _attemptPattern.Count);

        _playerAttemptTextList[_attemptPattern.Count - 1].text = noteToAdd.NoteID.ToString();


        if(_attemptPattern.Count >= _solutionPattern.Count)
        {
            _isAcceptingAttempt = false;
            TestAttempt();
        }

    }

    private void TestAttempt()
    {
        bool isAttemptCorrect = true;

        for (int i = 0; i < _solutionPattern.Count; i++)
        {
            if (_attemptPattern[i] != _solutionPattern[i])
            {
                isAttemptCorrect = false;
                break;
            }
        }

        if (isAttemptCorrect)
        {
            AttemptCorrect();
        }
        else
        {
            AttemptFailed();
        }

    }

    private void AttemptCorrect()
    {
        Debug.Log("attempt correct!");


    }

    private void AttemptFailed()
    {
        Debug.Log("attempt incorrect");

    }
}
