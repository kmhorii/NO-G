using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class VolumeSlider : MonoBehaviour
{
    public Slider volume;
    public Text sliderText;
    [SerializeField]
    float valueOffset;
    [SerializeField]
    float valueScale;

    float newVolume;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnSliderChanged()
    {
        sliderText.text = ((int) volume.value * 100).ToString();
    }
}
