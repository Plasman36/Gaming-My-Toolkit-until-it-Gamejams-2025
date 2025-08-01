using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    SFXTrigger[] triggers;
    AudioSource source;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        triggers = GetComponents<SFXTrigger>();
        source = GetComponent<AudioSource>();

    }
    public void playSFX(string naming)
    {
        if (source.isPlaying)
        {
            return;
        }
        foreach (SFXTrigger trigger in triggers)
        {
            if (trigger.naming == naming)
            {
                trigger.triggerAudio();
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
