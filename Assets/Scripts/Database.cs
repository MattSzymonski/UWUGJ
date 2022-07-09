using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePoints
{
    public TypeEnum neighborType;
    public int bonus;
}

public class SideData
{
    public SideEnum side;
    public List<SideData> likedTypes;
}

[System.Serializable]
public class ElementDefinition
{
    public string name;
    public List<SideData> sidesData;
    //public int heightBonus;
}
public class Database : MonoBehaviour
{
    public List<ElementDefinition> elementDefinitions;
}
