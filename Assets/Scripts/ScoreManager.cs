using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour // TODO: this mixes logic of UI and scoring for now
{
    public int barHeight = 45;

    public List<int> scoreTargets;
    public int currentTargetIdx = 0;
    public int score = 0;
    public int totalScore = 0;
    
    public GameObject outerScoreBar;
    public GameObject innerScoreBar;
    public GameObject currentScoreText;
    public GameObject targetScoreText;
    public GameObject[] elements;
    public GameObject[] buttons; // DO I NEED IT?AA
    public GameObject[] highlights;

    public Sprite[] spritesForElements;
    public PlayerController pc;

    public Database db;
    // Start is called before the first frame update
    void Start()
    {
        innerScoreBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, barHeight);
        PopulateNewElementsUI();
        //HighlightChosenSprite(0);
    }

    // Update is called once per frame
    void Update()
    {
        float proportionFilled = score / (float)scoreTargets[currentTargetIdx];
        // fill by the percentage of current maxSize
        innerScoreBar.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(proportionFilled * outerScoreBar.GetComponent<RectTransform>().sizeDelta.x, barHeight);

        currentScoreText.GetComponent<Text>().text = score.ToString();

        targetScoreText.GetComponent<Text>().text = scoreTargets[currentTargetIdx].ToString();
    }

    List<Element> ReturnNewElementsUI()
    {
        List<Element> newElements = new();
        for (int i = 0; i < 4; ++i)
            newElements.Insert(i, db.elements[Random.Range(0, db.elements.Capacity)]);
        return newElements;
    }

    void PopulateNewElementsUI()
    {
        int idx = 0;
        foreach (var el in ReturnNewElementsUI())
        {
            // lookup the tex for this element
            db.thisLevelChoice[idx] = db.elements[(int)el.type];
            elements[idx].GetComponent<Image>().sprite = spritesForElements[(int)el.type];
            EnableElementSprite(idx);
            ++idx;
        }
        pc.ResetIndex();
    }

    public void DisableElementSprite(int idx)
    {
        Color col = elements[idx].GetComponent<Image>().color;
        col.a = 0.3f;
        elements[idx].GetComponent<Image>().color = col;
    }
    public void EnableElementSprite(int idx)
    {
        Color col = elements[idx].GetComponent<Image>().color;
        col.a = 1.0f;
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

    public bool UpdateScore(int toAdd)
    {
        // TODO: rescale the score bar + Juicers
        if (score + toAdd > scoreTargets[currentTargetIdx])
        {
            if (currentTargetIdx >= scoreTargets.Capacity)
            {
                Debug.LogError("Error, add more score ranges!");
                return false;
            }
            // TODO: think about triggering map extension and other juicers

            ++currentTargetIdx;

            PopulateNewElementsUI();

            // rescale the bar and reset the proportion?
            totalScore += score;

            // reset the score and add the remainder
            int rem = score - scoreTargets[currentTargetIdx - 1] + toAdd;
            score = rem;
            // TODO: smarter rescaling without feeling of going backwards
            var delta = outerScoreBar.GetComponent<RectTransform>().sizeDelta;
            outerScoreBar.GetComponent<RectTransform>().sizeDelta =
                new Vector2(delta.x + 100, delta.y);
            float proportionFilled = score / (float)scoreTargets[currentTargetIdx];
            innerScoreBar.GetComponent<RectTransform>().sizeDelta = 
                new Vector2(proportionFilled * outerScoreBar.GetComponent<RectTransform>().sizeDelta.x, barHeight);
            return true;
        }
        else
            score += toAdd;
        return false;
    }
}
