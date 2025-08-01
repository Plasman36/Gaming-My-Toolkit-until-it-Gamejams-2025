using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public bool isInOptions = false;


    public void setMasterVolume(float volume)
    {
        mixer.SetFloat("MasterVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void OpenOptions()
    {
        gameObject.SetActive(true);
        isInOptions = true;
    }

    public void CloseOptions()
    {
        gameObject.SetActive(false);
        isInOptions = false;
    }
}
