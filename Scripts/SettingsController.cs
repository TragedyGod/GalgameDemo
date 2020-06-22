using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour {
    public static SettingsController _isntance;
    public GameObject Bgm, Voice, TextSpeed;
	// Use this for initialization
	void Awake () {
        _isntance = this;
        Bgm = GameObject.Find("BGMController");
        Voice = GameObject.Find("TreatmentController");
        TextSpeed = GameObject.Find("UIController");
	}
	
	

    public void ChangeBgmVolume(float volume)
    {
        Bgm.GetComponent<AudioSource>().volume = volume;
    }
    public void ChangeVoiceVolume(float volume)
    {
        Voice.GetComponent<AudioSource>().volume = volume;
    }
    public void ChangeTextSpeed(float textSpeed)
    {
        TextSpeed.GetComponent<UIController>().showTextSpeed = textSpeed;
    }
}
