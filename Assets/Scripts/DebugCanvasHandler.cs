using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugCanvasHandler : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _debugMenuCG;
    [SerializeField] protected string _prototypeSceneName;
    
    public void OnToggleDebugMenu(InputValue value)
    {
        if (_debugMenuCG.alpha == 1f) _debugMenuCG.alpha = 0f;
        else _debugMenuCG.alpha = 1f;
    }

    public void OnReloadScene(InputValue value)
    {
        SceneManager.LoadScene(_prototypeSceneName);
    }

    public void OnCloseDemo(InputValue value)
    {
        Application.Quit();
    }

}
