using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SpriteBuoyancy : MonoBehaviour
{
    //script for the bobbing motion of sprites

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer _sprite;

    [Header("Motion")]
    [SerializeField] private float _interval = 1f;
    [SerializeField] private float _offset = 1f;
    [SerializeField] private AnimationCurve _motionCurve;

    private float timer = 0f;

    private void Start()
    {
        StartCoroutine(BuoyancyCoroutine());
    }

    private IEnumerator BuoyancyCoroutine()
    {
        
        
        //move up

        //move back down



        yield return null;
    }
}
