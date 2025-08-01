using UnityEngine;

public class SFXTrigger : MonoBehaviour
{
    public string naming;
    AudioSource sound;
    public AudioClip[] clips;
    //OptionsMenu menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sound = GetComponent<AudioSource>();
        //menu = FindAnyObjectByType<OptionsMenu>();
    }

    public void triggerAudio()
    {
        sound.clip = clips[Random.Range(0, clips.Length)];
        sound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
