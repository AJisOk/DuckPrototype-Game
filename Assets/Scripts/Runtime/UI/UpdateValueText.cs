using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateValueText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textToUpdate;
    [SerializeField] private Slider _slider;


    public void OnSliderUpdate()
    {
        _textToUpdate.text = _slider.value.ToString();
    }
}
