using Unity.VisualScripting;
using UnityEngine;

public class StopPlaying : MonoBehaviour
{
    private EndGoal goal;
    public AudioSource audio;

    private void Start()
    {
        goal = FindAnyObjectByType<EndGoal>();
    }

    private void Update()
    {
        if (goal.hasWon)
        {
            audio.volume = 0;
        }
    }
}
