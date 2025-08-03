using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    public bool goFast = false;
    public bool uniqueInstruments = false;
    public bool addSnare = false;
    public bool addBass = false;

    public AudioSource channel1;
    public AudioSource channel2;
    public AudioSource channel3;

    TrackData music = new TrackData();

    public AudioClip melodyC;
    //public AudioClip melodyCHigh;

    public AudioClip otherC;
    //public AudioClip otherCHigh;

    public AudioClip bassC;
    //public AudioClip bassCHigh;

    public AudioClip snare;

    public float standardTempo;
    public float quickTempo;

    private float standardSPM;
    private float quickSPM;

    private bool newMeasure = false;
    private int[] currentNotes = { 0, 0, 0 };
    private int currentMeasure = 0;
    private float measureTimer = 0.0f;
    private float usedSPM;

    void Start()
    {
        standardSPM = (1/(standardTempo / 60))/4;
        quickSPM = (1 / (quickTempo / 60)) / 4;
        channel2.clip = bassC;
        channel3.clip = snare;

        StartCoroutine(delayStart());
    }

    void Update()
    {
        if (newMeasure)
        {
            if (uniqueInstruments)
            {
                channel1.clip = otherC;
            }
            else
            {
                channel1.clip = melodyC;
            }
            if (goFast)
            {
                usedSPM = quickSPM;
            }
            else
            {
                usedSPM = standardSPM;
            }

            measureTimer = 16 * usedSPM;

            StartCoroutine(playMeasure(channel1, currentNotes[0], currentNotes[0] + music.notePerMeasure[0, currentMeasure], 0, usedSPM));
            if (addBass)
            {
                StartCoroutine(playMeasure(channel2, currentNotes[1], currentNotes[1] + music.notePerMeasure[1, currentMeasure], 1, usedSPM));
            }
            if (addSnare)
            {
                StartCoroutine(playMeasure(channel3, currentNotes[2], currentNotes[2] + music.notePerMeasure[2, currentMeasure], 2, usedSPM));
            }

            for (int i = 0; i < 3; i++)
            {
                currentNotes[i] += music.notePerMeasure[i, currentMeasure];
            }
            currentMeasure++;
            if (currentMeasure >= music.totalMeasures)
            {
                currentMeasure = 0;
                for (int j = 0; j < 3; j++)
                {
                    currentNotes[j] = 0;
                }
            }
            newMeasure = false;
        } else
        {
            measureTimer -= Time.deltaTime;
            if (measureTimer <= 0)
            {
                newMeasure = true;
            }
        }
        Debug.Log($"Notes positioning is {currentNotes[0]}, {currentNotes[1]}, {currentNotes[2]}");
    }

    IEnumerator playMeasure(AudioSource channel, int start, int stop, int track, float spm)
    {
        for (int i = start; i < stop; i++)
        {
            if (music.notes[track, i] != 32)
            {
                channel.pitch = Mathf.Pow(1.05944654665f, music.notes[track, i]);
                channel.Play();
            } else
            {
                channel.Stop();
            }

            yield return new WaitForSecondsRealtime(spm * music.lengthInSixteenths[track, i]);
        }
    }
    IEnumerator delayStart()
    {
        measureTimer = 9;
        yield return new WaitForSecondsRealtime(4);
        newMeasure = true;
    }
}
