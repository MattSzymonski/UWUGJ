using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public List<int> scoreCaps;
    public int currentCapIdx = 0;
    public int score = 0;
    public List<Element> newChoiceElements;
    public Database db;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (score > scoreCaps[currentCapIdx])
        {
            if (currentCapIdx >= scoreCaps.Capacity)
            {
                Debug.LogError("Error, add more score ranges!");
                return;
            }
            // TODO: think about triggering map extension and other juicers

            ++currentCapIdx;

            newChoiceElements = ReturnNewElements();
        }
    }

    private List<Element> ReturnNewElements()
    {
        List<Element> newElements = new();
        for (int i = 0; i < 4; ++i)
            newElements.Insert(i, db.elements[Random.Range(0, db.elements.Capacity)]);
        return newElements;
    }
}
