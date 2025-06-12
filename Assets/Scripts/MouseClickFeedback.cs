using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickFeedback : MonoBehaviour
{
    [SerializeField] protected GameObject _prefabToInstantiate;

    public void OnMoveTo(InputValue value)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Instantiate(_prefabToInstantiate, hit.point, Quaternion.identity);
        }
    }
}
