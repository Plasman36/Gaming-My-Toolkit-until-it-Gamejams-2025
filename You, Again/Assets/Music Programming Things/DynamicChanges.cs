using UnityEngine;
using System.Collections;

public class DynamicChanges : MonoBehaviour
{
    ReplayManager rm;
    BackgroundMusic musicPlayer;
    Explosive explosive;

    bool startWithBomb = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicPlayer = GetComponent<BackgroundMusic>();

        rm = FindAnyObjectByType<ReplayManager>();
        explosive = FindAnyObjectByType<Explosive>();
        if (explosive != null)
        {
            startWithBomb = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        musicPlayer.goFast = rm.clones.Count > 3;
        
    }
    IEnumerator explosiveExploded()
    {

    }
}
