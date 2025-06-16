using System.Net.Sockets;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField, Range(0, 1f)] protected float _parallaxEffectMultiplier;

    private Vector3 _startPos;

    private void Awake()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        transform.position = new Vector3(
            (Camera.main.transform.position.x * _parallaxEffectMultiplier) + _startPos.x,
            transform.position.y,
            transform.position.z
            );
    }
}
