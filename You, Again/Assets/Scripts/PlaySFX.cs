using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    SFXTrigger[] triggers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        triggers = GetComponents<SFXTrigger>();
    }
    public void playSFX(string naming)
    {
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
