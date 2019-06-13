using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMouseOptions : MonoBehaviour
{

    public Slider sensitivitySlider;
    public Slider smoothnessSlider;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        }

        if (PlayerPrefs.HasKey("MouseSmoothness"))
        {
            smoothnessSlider.value = PlayerPrefs.GetFloat("MouseSmoothness");
        }
    }


    public void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    public void SetSmoothness(float smoothness)
    {
        PlayerPrefs.SetFloat("MouseSmoothness", smoothness);
    }
}
