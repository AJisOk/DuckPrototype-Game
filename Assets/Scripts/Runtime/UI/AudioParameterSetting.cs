using UnityEngine;
using UnityEngine.Audio;
using Debug = UnityEngine.Debug;
using FMOD;
using FMODUnity;



[CreateAssetMenu(menuName = "Settings/Audio Parameter Setting (FMOD + Unity Mixer)")]
public class AudioParameterSetting : FloatSetting
{
    [Header("Mixer")]
    [SerializeField, ParamRef] private string _fmodParameter = "";      //FMOD parameter selector in inspector

    public override void SetValue(float newValue)
    {
        base.SetValue(newValue);

        if (!string.IsNullOrEmpty(_fmodParameter))
        {
            RESULT result = RuntimeManager.StudioSystem.setParameterByName(_fmodParameter, newValue);
            if (result != RESULT.OK)
            {
                Debug.LogError(string.Format("[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}", _fmodParameter, result));
            }
        }
    }
}
