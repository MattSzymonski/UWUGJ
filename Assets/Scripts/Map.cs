using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementSlot 
{
    public Element element;
}
public class Map : MonoBehaviour
{
    public const int MAP_SIZE = 10;
    public ElementSlot[,,] map;
    public Database database;
    public ScoreManager scoreManager;

    private List<(int, int)> sideToMapOffset;
    // Start is called before the first frame update
    void Start()
    {
        map = new ElementSlot[MAP_SIZE, MAP_SIZE / 2, MAP_SIZE]; // x, y, z
        for (int i = 0; i < MAP_SIZE; ++i)
            for (int j = 0; j < MAP_SIZE / 2; ++j)
                for (int k = 0; k < MAP_SIZE; ++k)
                    map[i, j, k] = new ElementSlot();

        sideToMapOffset = new List<(int, int)>(4);
        sideToMapOffset.Insert(0, ( 0, 1)); // x, z
        sideToMapOffset.Insert(1, ( 1, 0));
        sideToMapOffset.Insert(2, ( 0,-1));
        sideToMapOffset.Insert(3, (-1, 0));
        // TODO: for now placing els by hand, Map is not serializable
        //map[3, 1, 3].element = GameObject.Find("ElementB").GetComponent<Element>();
        //map[5, 1, 3].element = GameObject.Find("ElementA").GetComponent<Element>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool CanPlaceElement(Element el, Vector3 coords) 
    {
        // operate in global coordinates
        int rot = el.GetRotation();
        (int, int) mapOffset;

        // check if already occupied
        if (map[(int)(coords.x), (int)coords.y, (int)(coords.z)].element != null)
        {
            print("Element at indexes " + coords + " already exists");
            return false;
        }

        var element = database.elements.Find(x => x.type == el.type);

        for (int i = 0; i < 4; ++i)
        {
            mapOffset = sideToMapOffset[(i + rot) % 4];
            var neighbor = map[(int)(coords.x + mapOffset.Item1), (int)coords.y, (int)(coords.z + mapOffset.Item2)].element;
            print("Element indexes : " +  coords + " neigh indexes x: " + (coords.x + mapOffset.Item1) + " y: " + (coords.y) + " z: " + (coords.z + mapOffset.Item2) + " rot: " + rot);

            // no neighbor, no problem :)
            if (!neighbor)
                continue;

            print("This type: " + el.type + " Neighbor type: " + neighbor.type);
            print("Neighbor exists x: " + (coords.x + mapOffset.Item1) + " y: " + (coords.y) + " z: " + (coords.z + mapOffset.Item2) + " rot: " + rot);
            if (!element.sidesData[i].likedTypes.Exists(x => x.neighborType == neighbor.type))
            {
                print("This type :" + el.type + " does not like the neighbor: " + neighbor.type + " x: " + (coords.x + mapOffset.Item1) + " y: " + (coords.y) + " z: " + (coords.z + mapOffset.Item2) + " rot: " + rot);
                return false;
            }
        }
        //var neighborDef = database.elementinitions.Find(x => x.type == neighbor.type);
        // for now ignore out of bounds

        // check front (rotated)
        // for every side (first rotated), get neighbor, loop through their liked TypeEnums and if it is on their list, add bonus points for this side
        // if during this calculation we find that the neighbor does not like this Element's type, we return false 

        // the second (NOT IMPLEMENTED) option is to check if the element that we are checking also accepts this side Element
        // TODO: we need this for the edge case when the neighbor has walls that do not match ours
        
        if (coords.y > 0.0f) 
        {
            var neighbor = map[(int)(coords.x), (int)coords.y - 1, (int)(coords.z)].element;
            if (!neighbor)
            {
                Debug.LogError("Tried to place a block above empty space!");
                return false;
            }

            var side = element.sidesData.FirstOrDefault(x => x.side == SideEnum.DOWN);

            if (side != null)
            {
                if (!side.likedTypes.Exists(x => x.neighborType == neighbor.type)) {
                    Debug.Log("DOWN: This type :" + el.type + " does not like the neighbor: " + neighbor.type + " x: " + coords.x + " y: " + coords.y + " z: " + coords.z + " rot: " + rot);
                    return false;
                }
            } 
        }
        return true;
    }

    public void PlaceElement(Element el, Vector3 coords)
    {
        // first calculate neighbors according to current rotation of this element 
        // now calculate points
        // again we loop like in CanPlaceElement and add points for every neighbor
        // add points or return them somewhere
        int points = 0;
        int rot = el.GetRotation();
        (int, int) mapOffset;

        var element = database.elements.Find(x => x.type == el.type);

        for (int i = 0; i < 4; ++i)
        {
            mapOffset = sideToMapOffset[(i + rot) % 4];
            var neighbor = map[(int)(coords.x + mapOffset.Item1), (int)coords.y, (int)(coords.z + mapOffset.Item2)].element;
            print("Element indexes : " + coords + " neigh indexes x: " + (coords.x + mapOffset.Item1) + " y: " + (coords.y) + " z: " + (coords.z + mapOffset.Item2) + " rot: " + rot);

            // no neighbor, no problem :)
            if (!neighbor)
                continue;

            //print("This type: " + el.type + " Neighbor type: " + neighbor.type);
            //print("Neighbor exists x: " + (coords.x + mapOffset.Item1) + " y: " + (coords.y) + " z: " + (coords.z + mapOffset.Item2) + " rot: " + rot);
            int thisBonus = element.sidesData[i].likedTypes.Find(x => x.neighborType == neighbor.type).bonus;
            print("Points froms this neighbor: " + thisBonus);
            points += thisBonus;
        }

        // TODO: add bonus for height


        print("Total Points: " + points);
        map[(int)coords.x, (int)coords.y, (int)coords.z].element = el;
        scoreManager.score += points;
    }

    // TODO: add map bounds checks

    public int GetTileHeight(int x, int z)
    {
        if (x < 0 || x >= MAP_SIZE || z < 0 || z >= MAP_SIZE)
        {
            Debug.LogError("Indices passed are out of bounds! x: " + x + " z: " + z);
            return 0;
        }
        int height = 0;
        var elem = map[x, 0, z]; // TODO: indexing from 0?
        while (elem.element != null && height < MAP_SIZE / 2)
        {
            ++height;
            elem = map[x, height, z];
        }
        return height; // TODO: should be removed
    }

    public bool IsPositionValid(Vector3 pos)
    {
        if (pos.x < 0 || pos.x >= MAP_SIZE ||
            pos.y < 0 || pos.y >= MAP_SIZE / 2 ||
            pos.z < 0 || pos.z >= MAP_SIZE)
            return false;

        return true;
    }
}
