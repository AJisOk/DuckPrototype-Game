using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class MouseClickFeedback : MonoBehaviour
{
    [SerializeField] protected GameObject _prefabToInstantiate;
    [SerializeField] protected LayerMask _waterLayerMask;
    [SerializeField] protected LayerMask _pullableLayerMask;
    [SerializeField] protected EventReference _waterClickSound;
    [SerializeField] protected EventReference _pullableClickSound;
    [SerializeField] protected Vector3 _instantiateOffset;
    public void OnMoveTo(InputValue value)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit,Mathf.Infinity, _pullableLayerMask, QueryTriggerInteraction.Ignore))
        {
            //play pullable click sound
            RuntimeManager.PlayOneShot(_pullableClickSound);

            return;
        }

        if (Physics.Raycast(ray, out hit,Mathf.Infinity, _waterLayerMask, QueryTriggerInteraction.Ignore ))
        {
            Instantiate(_prefabToInstantiate, hit.point + _instantiateOffset, Quaternion.identity);

            //play water click sound
            RuntimeManager.PlayOneShot(_waterClickSound);
        }
    }
}
