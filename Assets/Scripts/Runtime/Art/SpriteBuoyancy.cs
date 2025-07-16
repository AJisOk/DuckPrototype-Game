using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SpriteBuoyancy : MonoBehaviour
{
    //script for the bobbing motion of sprites

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer _sprite;

    [Header("Motion")]
    [SerializeField] private float _interval = 2f;
    [SerializeField] private float _offsetDistance = .3f;
    [SerializeField] private AnimationCurve _motionCurve;

    private float timer = 0f;
    private float initialYPosition;
    

    private void Awake()
    {
        if(_sprite == null) _sprite = GetComponent<SpriteRenderer>();
        initialYPosition = transform.position.y;
    }

    private void Start()
    {
        StartCoroutine(BuoyancyCoroutine());
    }

    private IEnumerator BuoyancyCoroutine()
    {
        //offset start time so sprites with same interval don't fully mirror eachother

        timer = Random.Range(0f, _interval);
        Vector3 offset;
        float yClampedPosition;
        float yTargetPosition;
        float yOffset;
        while (true)
        {
            yClampedPosition = _motionCurve.Evaluate(Mathf.InverseLerp(0f, _interval, timer));
            yTargetPosition = yClampedPosition * _offsetDistance + initialYPosition;
            yOffset = yTargetPosition - _sprite.transform.position.y;
            offset = new Vector3(0f, yOffset, 0f);

            _sprite.transform.Translate(offset);

            timer += Time.deltaTime;
            if (timer >= _interval) timer = 0f;

            
            yield return null;
        }

        yield return null;
    }
}
