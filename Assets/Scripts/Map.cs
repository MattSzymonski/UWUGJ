using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class ElementSlot 
{
    public Element element;
}
public class Map : MonoBehaviour
{
    public const int MAP_SIZE = 100;
    public ElementSlot[,,] map;
    public Database database;
    private List<(int, int)> sideToMapOffset;
    // Start is called before the first frame update
    void Start()
    {
        map = new ElementSlot[MAP_SIZE, MAP_SIZE / 2, MAP_SIZE]; // x, y, z
        sideToMapOffset[0] = ( 0, -1); // x, z
        sideToMapOffset[1] = ( 1,  0);
        sideToMapOffset[2] = ( 0,  1);
        sideToMapOffset[3] = (-1,  0);
    }

    // Update is called once per frame
    void Update()
    {
        CanPlaceElement(GameObject.Find("Kuba").GetComponent<Element>(), (50, 0, 50));
    }

    public bool CanPlaceElement(Element el, (int, int, int) coords) // TODO : to vector3
    {
        // operate in global coordinates
        int rot = el.GetRotation();
        (int, int) mapOffset;

        var elementDef = database.elementDefinitions.Find(x => x.type == el.type);

        for (int i = 0; i < 4; ++i)
        {
            mapOffset = sideToMapOffset[(i + rot) % 4];
            var neighbor = map[coords.Item1 + mapOffset.Item1, coords.Item2, coords.Item3 + mapOffset.Item2].element;
            print("Element indexes : " +  coords + " neigh indexes x: " + (coords.Item1 + mapOffset.Item1) + " y: " + (coords.Item2) + " z: " + (coords.Item3 + mapOffset.Item2) + " rot: " + rot);
            if (!elementDef.sidesData[((i + rot) % 4)].likedTypes.Exists(x => x.neighborType == neighbor.type))
            {
                return false;
            }
        }
        //var neighborDef = database.elementDefinitions.Find(x => x.type == neighbor.type);
        // for now ignore out of bounds

        // check front (rotated)
        // for every side (first rotated), get neighbor, loop through their liked TypeEnums and if it is on their list, add bonus points for this side
        // if during this calculation we find that the neighbor does not like this Element's type, we return false 

        // the second (NOT IMPLEMENTED) option is to check if the element that we are checking also accepts this side Element
        // TODO: we need this for the edge case when the neighbor has walls that do not match ours
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
