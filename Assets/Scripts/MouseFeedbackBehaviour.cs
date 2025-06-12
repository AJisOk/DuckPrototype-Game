using UnityEngine;

public class MouseFeedbackBehaviour : MonoBehaviour
{
    [SerializeField] protected float _duration = 1f;

    private float _timer = 0f;
    private void FixedUpdate()
    {
        _timer += Time.deltaTime;

        if( _timer >= _duration)
        {
            Destroy(gameObject);
        }
    }
}
