using UnityEngine;
using System.Collections;

public class ContinuousSFX : MonoBehaviour
{
    SFXTrigger[] triggers;
    AudioSource source;
    bool conflict = false;
    public float delay;

    float timer = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        triggers = GetComponents<SFXTrigger>();
        source = GetComponent<AudioSource>();
    }

    public void playSFX(string naming)
    {
        if (conflict)
        {
            return;
        }
        foreach (SFXTrigger trigger in triggers)
        {
            if (trigger.naming == naming)
            {
                Debug.Log("There is conflict");
                conflict = true;
                trigger.triggerAudio();
                timer = delay;
                return;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            Debug.Log(timer);
        } else if (conflict)
        {
            conflict = false;
            Debug.Log("Stopped conflict");
        }
    }

}
