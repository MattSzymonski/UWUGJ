using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour // TODO: this mixes logic of UI and scoring for now
{
    public int barHeight = 25;

    public List<int> scoreTargets;
    public int currentTargetIdx = 0;
    public int score = 0;
    public int lastScore = 0;
    public int totalScore = 0;
    
    public GameObject outerScoreBar;
    public GameObject innerScoreBar;
    public GameObject currentScoreText;
    public GameObject targetScoreText;
    public GameObject[] elements;
    public GameObject[] buttons; // DO I NEED IT?AA
    public GameObject[] highlights;

    public Sprite[] spritesForElements;

    public Database db;
    // Start is called before the first frame update
    void Start()
    {
        innerScoreBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, barHeight);
        PopulateNewElements();
        //HighlightChosenSprite(0);
    }

    // Update is called once per frame
    void Update()
    {
        // redraw score if changed? in UI
        if (score > lastScore)
        {
            lastScore = score;

            // rescale the bar

            float proportionFilled = score / (float)scoreTargets[currentTargetIdx];
            // fill by the percentage of current maxSize
            innerScoreBar.GetComponent<RectTransform>().sizeDelta = 
                new Vector2(proportionFilled * outerScoreBar.GetComponent<RectTransform>().sizeDelta.x, barHeight);

            currentScoreText.GetComponent<Text>().text = score.ToString();

            // TODO: rescale the score bar + Juicers
            if (score > scoreTargets[currentTargetIdx])
            {
                if (currentTargetIdx >= scoreTargets.Capacity)
                {
                    Debug.LogError("Error, add more score ranges!");
                    return;
                }
                // TODO: think about triggering map extension and other juicers

                ++currentTargetIdx;

                PopulateNewElements();

                // rescale the bar and reset the proportion?
                totalScore += score;
                score = 0;
                lastScore = 0;

                targetScoreText.GetComponent<Text>().text = scoreTargets[currentTargetIdx].ToString();
                // TODO: smarter rescaling without feeling of going backwards
                var delta = outerScoreBar.GetComponent<RectTransform>().sizeDelta;
                outerScoreBar.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(delta.x + 100, delta.y);
                proportionFilled = score / (float)scoreTargets[currentTargetIdx];
                innerScoreBar.GetComponent<RectTransform>().sizeDelta = 
                    new Vector2(proportionFilled * outerScoreBar.GetComponent<RectTransform>().sizeDelta.x, barHeight);
            }
        }
    }

    List<Element> ReturnNewElements()
    {
        List<Element> newElements = new();
        for (int i = 0; i < 4; ++i)
            newElements.Insert(i, db.elements[Random.Range(0, db.elements.Capacity)]);
        return newElements;
    }

    void PopulateNewElements()
    {
        int idx = 0;
        foreach (var el in ReturnNewElements())
        {
            // lookup the tex for this element
            elements[idx].GetComponent<Image>().sprite = spritesForElements[(int)el.type];
            ++idx;
        }
    }

    public void DisableElementSprite(int idx)
    {
        Color col = elements[idx].GetComponent<Image>().color;
        col.a = 0.3f;
        elements[idx].GetComponent<Image>().color = col;
    }

    public bool IsElementDisabled(int idx)
    {
        Color col = elements[idx].GetComponent<Image>().color;
        return col.a != 1.0f;
    }

    public void HighlightChosenSprite(int idx)
    {
        for (int i = 0; i < 4; ++i)
        {
            if (i == idx)
                highlights[i].GetComponent<Image>().color = new Color(0.0f, 0.5f, 0.0f, 0.5f);
            else
                highlights[i].GetComponent<Image>().color = new Color();
        }
    }
}
