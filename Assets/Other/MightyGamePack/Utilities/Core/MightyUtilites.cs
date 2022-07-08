/* 
NAME:
    Mighty Utilites

DESCRIPTION:
    A bunch of helpful utility functions to use in other scripts for faster and cleaner coding
    !Some samples of useful code are below!

USAGE:
    MightyUtilites.ClearY(myVector);
    
TODO:
   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MightyGamePack
{
    public class MightyUtilites
    {
        ///<summary>
        ///Clears Y component of the vector3 (Eg. (2,5,3) -> (2,0,3))
        ///</summary>
        public static Vector3 ClearY(Vector3 vector) 
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        ///<summary>
        ///Converts vector2 to vector3 moving Y to Z(Eg. (8,1) -> (8,0,1))
        ///</summary>
        public static Vector3 Vec2ToVec3(Vector2 vector) 
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        ///<summary>
        ///Converts vector3 to vector2 moving Z to Y(Eg. (8,3,1) -> (8,1))
        ///</summary>
        public static Vector2 Vec3ToVec2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        ///<summary>
        ///Creates unit vector perpendicular to the line defined by two points
        ///</summary>
        public static Vector3 PerpendicularToLine(Vector3 lineStartPoint, Vector3 lineEndPoint)
        {
            Vector3 lineDir = lineStartPoint - lineEndPoint;
            Vector3 perpendicularDir = Vector3.Cross(lineDir, Vector3.up).normalized;
            return perpendicularDir;
        }

        
    }

    public enum MightyColor
    {
        Clear,
        White,
        Black,
        Gray,
        DarkGray,
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Coral,
        MightyNeon,
    }

    public static class MightyColorsExtension
    {
        public static Color GetColor(this MightyColor color)
        {
            switch (color)
            {
                case MightyColor.Clear:
                    return new Color32(0, 0, 0, 0);
                case MightyColor.White:
                    return new Color32(255, 255, 255, 255);
                case MightyColor.Black:
                    return new Color32(0, 0, 0, 255);
                case MightyColor.Gray:
                    return new Color32(128, 128, 128, 255);
                case MightyColor.DarkGray:
                    return new Color32(64, 64, 64, 255);
                case MightyColor.Red:
                    return new Color32(255, 0, 63, 255);
                case MightyColor.Orange:
                    return new Color32(255, 128, 0, 255);
                case MightyColor.Yellow:
                    return new Color32(255, 211, 0, 255);
                case MightyColor.Green:
                    return new Color32(98, 200, 79, 255);
                case MightyColor.Blue:
                    return new Color32(0, 135, 189, 255);
                case MightyColor.Coral:
                    return new Color32(255, 111, 97, 255);
                case MightyColor.MightyNeon:
                    return new Color32(255, 88, 143, 255);
                default:
                    return new Color32(0, 0, 0, 255);
            }
        }
    }


}


/*
OTHER USEFUL SCRIPTS:

    // Code Regions
        #region Gizmos
        #endregion
   
    // Gizmos
        void OnDrawGizmos()
        {
            //Examples:
            //DebugExtension.DrawCircle(transform.position, Vector3.up, Color.yellow, 3);
            //Gizmos.DrawLine(transform.position, transform.position);
            //Gizmos.DrawSphere(transform.position, 1f);
        }

    // Instantiate prefabs
        public GameObject objectPrefab;
        public List<GameObject> spawnedObjects;
        GameObject newObject = Instantiate(objectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newObject.transform.parent = this.transform;
        spawnedObjects.Add(newObject);

    // Mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Position in word based on mouse position on screen


    // Reversed for
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            GameObject objToDestroy = enemies[i];
            enemies.RemoveAt(i);
            Destroy(objToDestroy);       
        }

    // Classic timer
        flota myTimer;
        float myTime;
        void Update() 
        {
            if(myTimer < myTime)
            {
                myTimer += 1 * Time.deltaTime;
            }
            else
            {
                //Do stuff
                myTimer = 0;
            }
        }

     // Add this before function so it will be clicable in inspector
        [Button] 

    // Score (Add it to MainGameManager)
        public float score;
        score += Time.unscaledDeltaTime; // Seconds
        UIManager.SetScore((int)Mathf.Floor(score));
        score = 0; // In Reset function
        UIManager.ResetScore(); // In Reset function
		
	// RotateTowards
		transform.LookAt(Camera.main.transform.position, Vector3.up);
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + 180, 0);
*/
