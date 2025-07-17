using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



[CreateAssetMenu(fileName = "GammaSetting", menuName = "Scriptable Objects/GammaSetting")]
public class GammaSetting : FloatSetting
{
    [Header("Gamma")]
    [SerializeField] private VolumeProfile _volumeProfile;

    public override void SetValue(float newValue)
    {
        base.SetValue(newValue);


        if (_volumeProfile.TryGet(out UnityEngine.Rendering.Universal.LiftGammaGain urpLiftGammaGain))
        {
            urpLiftGammaGain.gamma.value = Vector4.one * newValue;
        }
        else
        {
            Debug.LogWarning($"Volume Profile: {_volumeProfile.name} is missing Lift Gamma Gain component.", _volumeProfile);
        }



    }
}
