using UnityEngine;
using System.Collections;

public class DynamicChanges : MonoBehaviour
{
    ReplayManager rm;
    BackgroundMusic musicPlayer;
    EndGoal goal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicPlayer = GetComponent<BackgroundMusic>();
        rm = FindAnyObjectByType<ReplayManager>();

        if (FindAnyObjectByType<RayGunScript>() != null || FindAnyObjectByType<GunController>() != null)
        {
            musicPlayer.uniqueInstruments = true;
        }
        if (FindAnyObjectByType<Explosive>() != null || FindAnyObjectByType<EnemyController>() != null)
        {
            musicPlayer.addSnare = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        goal = FindAnyObjectByType<EndGoal>();

        musicPlayer.addBass = rm.clones.Count > 0;
        musicPlayer.goFast = rm.clones.Count > 5;
        if (goal.hasWon)
        {
            musicPlayer.stop = true;
        } else {
            musicPlayer.stop = false;
        }
    }
}
