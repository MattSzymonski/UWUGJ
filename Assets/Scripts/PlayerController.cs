using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    public GameObject camera;
    //public bool freeFloatMode;
    public float currentAngle;
    public float rotationSpeed;
    public float rotationSmooth;
    float targetAngle;

    [Header("Cursor")]
    public Map map;
    public GameObject cursor;
    public Vector3 cursorPosition;

    [Header("Element")]
    public Element currentElement;

    bool dpadUp;
    bool triggersActivated;

    void Start()
    {
        cursorPosition = new Vector3(4, 1, 3);
    }

    void Update()
    {
        // Change camera mode
        //if (Input.GetAxis("ControllerAny DPad Y") > 0)
        //{
        //    if (!dpadUp)
        //    {
        //        freeFloatMode = !freeFloatMode;

        //        // Snap to nearest 90 multiplier
        //        if (freeFloatMode)
        //        {
        //            targetAngle = Mathf.Round(targetAngle / 90) * 90;
        //        }
        //        dpadUp = true;
        //    }
        //}
        //else
        //{
        //    dpadUp = false;
        //}

        // Camera orbiting
        //if (freeFloatMode)
        {
            float rotationDirection = -Input.GetAxis("ControllerAny Triggers") * rotationSpeed;
            targetAngle += rotationDirection;
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSmooth);
            camera.transform.eulerAngles = new Vector3(0, currentAngle, 0);
        }





        //else
        //{
        //    float triggerAxis = Input.GetAxis("ControllerAny Triggers");
        //    if (triggerAxis != 0)
        //    {
        //        if (!triggersActivated)
        //        {
        //            if (triggerAxis > 0f)
        //            {
        //                Debug.Log("Click R " + triggerAxis);
        //                triggersActivated = true;
        //                return;
        //            }

        //            if (triggerAxis < 0f)
        //            {
        //                Debug.Log("Click L " + triggerAxis);
        //                triggersActivated = true;
        //                return;
        //            }

        //        }
        //    }
        //    else
        //    {
        //        triggersActivated = false;
        //    }
        //}

        // Moving cursor


        // Placing element
        if (Input.GetButtonDown("ControllerAny Right Bumper"))
        {
            Debug.Log("Placing element: " + currentElement.type);
            if (map.CanPlaceElement(currentElement, cursorPosition))
            {
                print("Can place object: " + currentElement.type + " at pos: " + cursorPosition);
                map.PlaceElement(currentElement, cursorPosition);
            }
        }
    }
}
