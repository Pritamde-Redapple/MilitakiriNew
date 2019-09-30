using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetter : MonoBehaviour
{
    public Slider thisSlider;
    public AudioSetType thisType;

    float lastValue;


    private void OnEnable()
    {
        if (thisType == AudioSetType.SOUND)
            lastValue = PlayerPrefs.GetFloat(thisType.ToString());
        else
            lastValue = PlayerPrefs.GetFloat(thisType.ToString());

        thisSlider.value = lastValue;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(thisType.ToString(), thisSlider.value);
    }
    public void OnSoundChanged()
    {
        MilitakiriAudioManager.Instance.OnSoundVolumeChanged(thisSlider.value);
    }

    public void OnMusicChanged()
    {
        MilitakiriAudioManager.Instance.OnMusicVolumeChanged(thisSlider.value);
    }
}
