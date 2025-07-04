using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class MouseClickFeedback : MonoBehaviour
{
    [SerializeField] protected GameObject _prefabToInstantiate;
    [SerializeField] protected LayerMask _layerMask;
    [SerializeField] protected EventReference _clickSound;

    public void OnMoveTo(InputValue value)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,Mathf.Infinity, _layerMask, QueryTriggerInteraction.Ignore ))
        {
            //todo replace with VFX
            Instantiate(_prefabToInstantiate, hit.point, Quaternion.identity);

            //play water click sound
            RuntimeManager.PlayOneShot(_clickSound);
        }
    }
}
