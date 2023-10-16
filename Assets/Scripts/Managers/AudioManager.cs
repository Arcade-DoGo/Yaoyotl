using UnityEngine;

public class AudioManager : MonoBehaviour
{
    void Awake()
    {
        // General volume
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", AudioListener.volume);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            SetGlobalVolume(AudioListener.volume - 0.2f);
        else if(Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            SetGlobalVolume(AudioListener.volume + 0.2f);
        else if(Input.GetKeyDown(KeyCode.M))
            SetGlobalVolume(AudioListener.volume == 0 ? PlayerPrefs.GetFloat("Volume", 0) : 0);
    }

    public void SetGlobalVolume(float _volume)
    {
        AudioListener.volume = Mathf.Clamp01(_volume);
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);
    }
}