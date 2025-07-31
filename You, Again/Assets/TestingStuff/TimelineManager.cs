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
    private int labelCount;
    private List<Button> buttons = new List<Button>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        replayManager = GetComponentInChildren<ReplayManager>();
        lastCount = replayManager.clones.Count;
        labelCount = 1;
    }

    void deleteClone(int index)
    {
        Debug.Log($"deleted {index}");
        Destroy(buttons[index].gameObject);
        Destroy(replayManager.clones[index]);
        buttons.RemoveAt(index);
        replayManager.clones.RemoveAt(index);
        replayManager.allRecordedSegments.RemoveAt(index);
        for (int i = index; i < buttons.Count; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
            int currentI = i;
            buttons[i].onClick.AddListener(delegate { deleteClone(currentI); });
            buttons[i].GetComponent<RectTransform>().position += new Vector3(0, listOrigin.rect.height, 0);
        }
        lastCount--;
    }

    // Update is called once per frame
    void Update()
    {
       if (lastCount < replayManager.clones.Count)
        {

            buttons.Add(Instantiate(buttonPrefab, canvas));
            buttons.Last().gameObject.GetComponent<RectTransform>().position = listOrigin.position - new Vector3(0, listOrigin.rect.height, 0)*(buttons.Count - 1);
            buttons.Last().GetComponentInChildren<TMPro.TMP_Text>().text = $"Clone {labelCount}";
            int currentI = buttons.Count - 1;
            buttons.Last().onClick.AddListener(delegate { deleteClone(currentI); } );
            lastCount = replayManager.clones.Count;
            labelCount++;
        }
    }
}


