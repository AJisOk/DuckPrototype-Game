using Unity.VisualScripting;
using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour
{
    //[SerializeField] bool _rotateY = false;
    //[SerializeField] bool _rotateX = false;

    private SpriteRenderer _spriteToFace;

    private void Awake()
    {
        if (_spriteToFace == null ) _spriteToFace = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _spriteToFace.transform.rotation = Camera.main.transform.rotation;
        

        //if(_rotateY)
        //{
        //    _spriteToFace.transform.rotation.Equals(new Quaternion(
        //        _spriteToFace.transform.rotation.x,
        //        Camera.main.transform.rotation.y, 
        //        _spriteToFace.transform.rotation.z,
        //        _spriteToFace.transform.rotation.w
        //        ));
        //}
        //if(_rotateX)
        //{
        //    _spriteToFace.transform.rotation.Equals(new Quaternion(
        //        Camera.main.transform.rotation.x,
        //        _spriteToFace.transform.rotation.y, 
        //        _spriteToFace.transform.rotation.z,
        //        _spriteToFace.transform.rotation.w
        //        ));
        //}
    }
}
