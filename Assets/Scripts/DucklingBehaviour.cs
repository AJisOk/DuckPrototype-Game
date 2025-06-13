using System;
using UnityEngine;
using UnityEngine.AI;

public class DucklingBehaviour : MonoBehaviour
{
    //when duck gets close enouhg, check if anything in between duckling and duck, if not, start following duck

    [SerializeField] protected SphereCollider _ducklingAttractionCollider;

    private bool _isLost = true;

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
        }
        else
        {
            //play kinder sound and animation
        }
    
    }

}
