using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickFeedback : MonoBehaviour
{
    [SerializeField] protected GameObject _prefabToInstantiate;
    [SerializeField] protected LayerMask _layerMask;

    public void OnMoveTo(InputValue value)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,Mathf.Infinity, _layerMask, QueryTriggerInteraction.Ignore ))
        {
            Instantiate(_prefabToInstantiate, hit.point, Quaternion.identity);
        }
    }
}
