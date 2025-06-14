using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;

public class DuckQuackHandler : MonoBehaviour
{
    [SerializeField] protected string _ducklingsTagText;
    [SerializeField] protected bool _allDucklingsQuack = false;
    [SerializeField] protected float _ducklingQuackRange = 50f;
    [SerializeField] protected AnimationCurve _quackAnimCurve;
    [SerializeField] protected CanvasGroup _quackCG;
    [SerializeField] protected float _quackAnimDuration = 1f;

    private bool _isQuacking = false;


    public void OnQuack(InputValue value)
    {
        if (_isQuacking) return;

        _isQuacking = true;

        StartCoroutine(QuackAnim());

        GameObject[] ducklingGO;
        ducklingGO = GameObject.FindGameObjectsWithTag(_ducklingsTagText);

        if (_allDucklingsQuack)
        {
            //narrow down array to only ducklings within quack range

            float distanceFromPlayerDuckSquared;
            Vector3 diff;

            foreach(GameObject duckling in ducklingGO)
            {
                diff = duckling.transform.position - transform.position;

                distanceFromPlayerDuckSquared = diff.sqrMagnitude;

                //if its within the duckling detection range
                if(distanceFromPlayerDuckSquared <= (_ducklingQuackRange * _ducklingQuackRange))
                {
                   DucklingBehaviour dB = duckling.GetComponentInChildren<DucklingBehaviour>();

                    dB.TryQuack();
                }

            }

        }
        else
        {
            //return the nearest lost duckling


        }

    }

    private IEnumerator QuackAnim()
    {
        float timer = 0f;

        while (timer < _quackAnimDuration)
        {
            _quackCG.alpha = _quackAnimCurve.Evaluate(timer);
            timer += Time.deltaTime;
            yield return null;
        }
        _quackCG.alpha = 0f;
        _isQuacking = false;
        yield return null;
    }
}
