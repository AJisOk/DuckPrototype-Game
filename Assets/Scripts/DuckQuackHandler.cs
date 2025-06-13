using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

public class DuckQuackHandler : MonoBehaviour
{
    [SerializeField] protected string _ducklingsTagText;
    [SerializeField] protected bool _allDucklingsQuack = false;
    [SerializeField] protected float _ducklingQuackRange = 50f;


    public void OnQuack(InputValue value)
    {
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
                   DucklingBehaviour dB = duckling.GetComponent<DucklingBehaviour>();

                    dB.TryQuack();
                }

            }

        }
        else
        {
            //return the nearest lost duckling


        }


    }
}
