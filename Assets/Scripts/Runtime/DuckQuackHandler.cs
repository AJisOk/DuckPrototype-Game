using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;


public class DuckQuackHandler : MonoBehaviour
{
    [Header("Duckling Locating")]
    [SerializeField] protected string _ducklingsTag;
    [SerializeField] protected float _ducklingQuackRange = 50f;

    [Header("Quacking Feedback")]
    [SerializeField] protected AnimationCurve _quackAnimCurve;
    [SerializeField] protected CanvasGroup _quackCG;
    [SerializeField] protected float _quackAnimDuration = 1f;
    [SerializeField] protected EventReference _duckQuackSound;

    [Header("Triggering Quack Listeners")]
    [SerializeField, Range(5f, 20f)] protected float _listenerTriggerRange = 20f;

    private bool _isQuacking = false;

    private DucklingBehaviour _closestLostDuckling;


    public void Quack()
    {
        //first, play the players quack animation
        StartCoroutine(QuackAnim());

        //then, find all ducklings
        GameObject[] ducklingGO;
        ducklingGO = GameObject.FindGameObjectsWithTag(_ducklingsTag);

        float distanceFromPlayerSqr;
        Vector3 diff;

        float closestToPlayerDistanceSqr = _ducklingQuackRange*_ducklingQuackRange;

        List<DucklingBehaviour> ducklingsInRange = new List<DucklingBehaviour>();

        foreach(GameObject duckling in ducklingGO)
        {
            //get duckling behaviour component
            DucklingBehaviour db = duckling.GetComponentInChildren<DucklingBehaviour>();

            //find the distance between the player and duckling
            diff = duckling.transform.position - transform.position;
            distanceFromPlayerSqr = diff.sqrMagnitude;

            //if this duckling is in range and the closest yet and is lost, add the previous closest to the muted quack list 
            //and store this one as the closest and update the closest distance
            if(db.IsLost && distanceFromPlayerSqr < closestToPlayerDistanceSqr)
            {
                if(_closestLostDuckling) ducklingsInRange.Add(_closestLostDuckling);
                _closestLostDuckling = db;
                closestToPlayerDistanceSqr = distanceFromPlayerSqr;
                continue;
            }

            //if the distance is less than the max attenuation range, add to list of ducklings to quack
            if(distanceFromPlayerSqr <= _ducklingQuackRange * _ducklingQuackRange)
            {
                ducklingsInRange.Add(duckling.GetComponentInChildren<DucklingBehaviour>());
            }

        }

        //now we have the closest lost duckling and a list of the rest (including not lost)
        //we want the closest to do a focused quack before clearing the variable
        //and we want the rest to do a muted quack

        _closestLostDuckling.TryFocusedQuack();

        foreach (DucklingBehaviour db in ducklingsInRange) db.TryMutedQuack();

        //ducklings quacking complete, time to clear variables
        _closestLostDuckling = null;

        

        //old, need to redo in a more optimal way

        //if (_isQuacking) return;

        //_isQuacking = true;

        //StartCoroutine(QuackAnim());

        //GameObject[] ducklingGO;
        //ducklingGO = GameObject.FindGameObjectsWithTag(_ducklingsTagText);

        //if (_allDucklingsQuack)
        //{
        //    //narrow down array to only ducklings within quack range

        //    float distanceFromPlayerDuckSquared;
        //    Vector3 diff;
        //    //DucklingBehaviour nearestDB;


        //    foreach(GameObject duckling in ducklingGO)
        //    {
        //        diff = duckling.transform.position - transform.position;

        //        distanceFromPlayerDuckSquared = diff.sqrMagnitude;

        //        //if its within the duckling detection range
        //        if(distanceFromPlayerDuckSquared <= (_ducklingQuackRange * _ducklingQuackRange))
        //        {
        //            DucklingBehaviour dB = duckling.GetComponentInChildren<DucklingBehaviour>();

        //            dB.TryQuack();
        //        }

        //    }

        //}
        //else
        //{
        //    //return the nearest lost duckling

        //    float distanceFromPlayerDuckSquared;
        //    Vector3 diff;
        //    DucklingBehaviour nearestDB = new DucklingBehaviour();
        //    float nearestDBDistanceSquared = _ducklingQuackRange*_ducklingQuackRange;

        //    foreach(GameObject duckling in ducklingGO)
        //    {
        //        diff = duckling.transform.position - transform.position;
        //        distanceFromPlayerDuckSquared = diff.sqrMagnitude;

        //        if(distanceFromPlayerDuckSquared <= (_ducklingQuackRange * _ducklingQuackRange))
        //        {
        //            DucklingBehaviour db = duckling.GetComponent<DucklingBehaviour>();

        //            //tell db to play quack sound

        //            if(distanceFromPlayerDuckSquared < nearestDBDistanceSquared)
        //            {
        //                nearestDBDistanceSquared = distanceFromPlayerDuckSquared;
        //                nearestDB = db;
        //            }
        //        }
        //    }

        //    nearestDB.TryQuack();

        //}

    }

    //TODO: triggers quack listeners within an adjustable range, called when quack is called
    private void TriggerNearbyListeners()
    {
        //collider check?
    }

    private IEnumerator QuackAnim()
    {
        _isQuacking = true;

        float timer = 0f;

        //FOR BRIAN player duck quacks here
        RuntimeManager.PlayOneShot(_duckQuackSound);

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
