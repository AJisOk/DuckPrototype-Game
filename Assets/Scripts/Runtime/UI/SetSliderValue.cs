using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetSliderValue : MonoBehaviour
{
    [SerializeField] private FloatSetting _setting;
    [SerializeField] private TextMeshProUGUI _textToSet;

    private void OnEnable()
    {
        GetComponent<Slider>().SetValueWithoutNotify(_setting.GetSliderValue());
        _textToSet.text = GetComponent<Slider>().value.ToString();
    }
}
