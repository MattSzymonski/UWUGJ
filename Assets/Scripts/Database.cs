using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public List<Element> elements;
    public List<Element> thisLevelChoice; // randomize on start thisLevelChoice?

    void Start()
    {
        for (int i = 0; i < 4; ++i)
            thisLevelChoice.Insert(i, elements[Random.Range(0, elements.Capacity)]);
        
    }
}
