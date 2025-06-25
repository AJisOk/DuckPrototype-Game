using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class DuckCharacterController : MonoBehaviour
{
    //Character controller will handle input and movement
    //2 input modes - moveto and directional
    //1 input actions

    [Header("Components")]
    [SerializeField] protected NavMeshAgent _duckAgent;
    [SerializeField] protected Animator _spriteAnimator;

    [Header("Movement")]
    [SerializeField] protected bool _mouseMovement = false;

    private PlayerInput _playerInput;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

    }

    private void OnMoveTo(InputValue inputValue)
    {
        if (!_mouseMovement) return;
    }

    private void OnTryGrab(InputValue inputValue)
    {
        if (!_mouseMovement) return;
    }
    
    private void OnTryRelease(InputValue inputValue)
    {

    }

    private void OnQuack(InputValue inputValue)
    {

    }

    private void OnMove(InputValue inputValue)
    {

    }
}
