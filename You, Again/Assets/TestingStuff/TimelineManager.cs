using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TimelineManager : MonoBehaviour
{
    public Button buttonPrefab;
    public Transform canvas;
    public RectTransform listOrigin;

    private ReplayManager replayManager;
    private int lastCount;
    private List<Button> buttons = new List<Button>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        replayManager = GetComponentInChildren<ReplayManager>();
        lastCount = replayManager.clones.Count;
    }

    void deleteClone(int index)
    {
        //Destroy(buttons)
        //buttons.RemoveAt(index);
        //NOT YET DONE
    }

    // Update is called once per frame
    void Update()
    {
       if (lastCount < replayManager.clones.Count)
        {

            buttons.Add(Instantiate(buttonPrefab, canvas));
            buttons.Last().gameObject.GetComponent<RectTransform>().position = listOrigin.position - new Vector3(0, listOrigin.rect.height, 0)*lastCount;
            buttons.Last().GetComponentInChildren<TMPro.TMP_Text>().text = $"Clone {lastCount+1}";
            buttons.Last().onClick.AddListener(() => deleteClone(lastCount));
            lastCount = replayManager.clones.Count;
        }
    }
}


