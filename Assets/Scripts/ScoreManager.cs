using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour // TODO: this mixes logic of UI and scoring for now
{
    public List<int> scoreCaps;
    public int currentCapIdx = 0;
    public int score = 0;

    public GameObject scoreUI;
    public GameObject[] elements;
    public GameObject[] buttons; // DO I NEED IT?A

    public Sprite[] spritesForElements;

    public Database db;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // redraw score if changed? in UI
        // rescale the bar
        if (score > scoreCaps[currentCapIdx])
        {
            if (currentCapIdx >= scoreCaps.Capacity)
            {
                Debug.LogError("Error, add more score ranges!");
                return;
            }
            // TODO: think about triggering map extension and other juicers

            ++currentCapIdx;

            int idx = 0;
            foreach (var el in ReturnNewElements())
            {
                // lookup the tex for this element
                elements[idx].GetComponent<Image>().sprite = spritesForElements[(int)el.type];
                ++idx;
            }

            // rescale the score bar + Juicers
            
        }
    }

    List<Element> ReturnNewElements()
    {
        List<Element> newElements = new();
        for (int i = 0; i < 4; ++i)
            newElements.Insert(i, db.elements[Random.Range(0, db.elements.Capacity)]);
        return newElements;
    }
}
