using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SidePoints
{
    public TypeEnum neighborType;
    public int bonus = 40;
    // TODO: add neighborLikedSides
}

[System.Serializable]
public class SideData
{
    public SideEnum side;
    public List<SidePoints> likedTypes;
}

public class Element : MonoBehaviour
{
    public TypeEnum type;
    public List<SideData> sidesData;
    public int rotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int GetRotation()
    {
        // find how many increments of 90deg we moved away from the 0y rotation
        // 0 is FRONT, 3 is LEFT
        float rot = gameObject.GetComponent<Transform>().rotation.eulerAngles.y;
        rotation = (int)(rot / 90.0);
        return rotation;
    }
}
