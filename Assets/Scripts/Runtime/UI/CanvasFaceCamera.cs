using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    [SerializeField] protected Canvas _canvasToFace;

    private void Update()
    {
        if (_canvasToFace == null) return;

        _canvasToFace.transform.rotation = Camera.main.transform.rotation;

    }
}
