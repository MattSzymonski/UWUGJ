using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSlot 
{
    public Element element;
}
public class Map : MonoBehaviour
{
    public const int MAP_SIZE = 100;
    public ElementSlot[,,] map;
    public Database database;
    // Start is called before the first frame update
    void Start()
    {
        map = new ElementSlot[MAP_SIZE, MAP_SIZE, MAP_SIZE / 2];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanPlaceElement(Element el, (int, int, int) coords)
    {
        // for every side (first rotated), get neighbor, loop through their liked TypeEnums and if it is on their list, add bonus points for this side
        // if during this calculation we find that the neighbor does not like this Element's type, we return false a

        return true;
    }

    public void PlaceElement(Element el, (int, int,int) coords)
    {
        // first calculate neighbors according to current rotation of this element 
        // int starting from front in increments of enum.
        // CanPlaceElement?
        // now calculate points
        // again we loop like in CanPlaceElement and add points for every neighbor
        // add points or return them somewhere

        map[coords.Item1, coords.Item2, coords.Item3].element = el;

    }
}
