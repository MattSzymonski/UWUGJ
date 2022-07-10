using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    public MainGameManager mainGameManager;

    [Header("Camera")]
    public GameObject camera;
    //public bool freeFloatMode;
    [ReadOnly] public float currentCameraAngle;
    [ReadOnly] public float cameraAngleCounterClockwise;
    [ReadOnly] public float currentAngleClamped;
    [ReadOnly] public Vector3 cameraDirection;
    public float rotationSpeed;
    public float rotationSmooth;
    float targetAngle;

    [Header("Cursor")]
    public Map map;
    public GameObject cursor;
    public float cursorSpeed = 1;
    public Vector3 cursorPosition = new Vector3(50, 0, 50);
    public Color cursorInvalid;
    public Color cursorValid;

    [ReadOnly] public float cursorAngle;
    [ReadOnly] public float cursorCameraAngle;
    [ReadOnly] public Vector3 neighbourDirection;

    [ReadOnly] public Vector3 neighbourDirectionPointer;
    [ReadOnly] public float neighbourDirectionPointerAngle;
    [ReadOnly] public Vector3 targetCursorPosition;

    [Header("Element")]
    public Element currentElement;

    bool dpadUp;
    bool triggersActivated;
    public Vector3 cursorDirection;
    [ReadOnly] public float cursorMagnitude;
    [ReadOnly] public bool cursorMoved;
    [ReadOnly] public bool cursorStartedMoving;
    private MightyGamePack.MightyTimer cursorDelayTimer;


    public GameObject n0;
    public GameObject n1;
    public GameObject n2;
    public GameObject n3;

    public ScoreManager scoreManager;
    public Database database;
    int chosenElementIdx;



    void Start()
    {
        var manager = mainGameManager.timersManager;
        cursorDelayTimer = manager.CreateTimer("CursorDelayTimer", 0.01f, 1f, false, true); // Create new timer (Not looping, stopped on start)
        currentElement = database.elements[0]; // TODO: change into what is chosen as first element in ScoreManager!
    }

    void Update()
    {
        // Camera orbiting
        float rotationDirection = -Input.GetAxis("ControllerAny Triggers") * rotationSpeed;
        targetAngle += rotationDirection;
        currentCameraAngle = Mathf.LerpAngle(currentCameraAngle, targetAngle, Time.deltaTime * rotationSmooth);

        currentAngleClamped = Clamp0360(currentCameraAngle);
        camera.transform.eulerAngles = new Vector3(0, currentCameraAngle, 0);
        cursorDirection = new Vector3(Input.GetAxis("ControllerAny Left Stick Horizontal"), 0, -Input.GetAxis("ControllerAny Left Stick Vertical"));
        cursorMagnitude = cursorDirection.magnitude;

        neighbourDirectionPointer = Quaternion.Euler(0, Clamp0360(currentCameraAngle), 0) * cursorDirection.normalized;
        neighbourDirectionPointerAngle = Mathf.Abs(Angle360OneToAnother(neighbourDirectionPointer, -Vector3.forward, Vector3.right) - 360);
/*
        n0.transform.position = new Vector3(1, 0, 0);
        n1.transform.position = new Vector3(0, 0, 1);
        n2.transform.position = new Vector3(-1, 0, 0);
        n3.transform.position = new Vector3(0, 0, -1);

        // Start timer and wait 0.05 sec to move then block moving until stick is reseted to zero again
        if (cursorMagnitude > 0.02f)
        {
            neighbourDirection = GetNeighbourDirection();
            if (neighbourDirection ==  new Vector3(1, 0, 0))
            {
                n0.transform.position = n0.transform.position + new Vector3(0, 0.3f, 0);
            }
            if (neighbourDirection == new Vector3(0, 0, 1))
            {
                n1.transform.position = n1.transform.position + new Vector3(0, 0.3f, 0);
            }
            if (neighbourDirection == new Vector3(-1, 0, 0))
            {
                n2.transform.position = n2.transform.position + new Vector3(0, 0.3f, 0);
            }
            if (neighbourDirection == new Vector3(0, 0, -1))
            {
                n3.transform.position = n3.transform.position + new Vector3(0, 0.3f, 0);
            }
        }
*/

        if (!cursorMoved)
        {
            if (cursorMagnitude > 0.02f)
            {
                if (!cursorStartedMoving)
                {
                    cursorDelayTimer.RestartTimer();
                    cursorDelayTimer.PlayTimer();
                    cursorStartedMoving = true;
                }


                if (cursorStartedMoving && cursorDelayTimer.finished)
                {
                    Debug.Log("moving cursor");

                    neighbourDirection = GetNeighbourDirection();
                    cursorPosition += neighbourDirection;
                    cursorPosition = new Vector3(cursorPosition.x, map.GetTileHeight((int)cursorPosition.x, (int)cursorPosition.z), cursorPosition.z);
                    targetCursorPosition = cursorPosition;

                    /*Color color = map.CanPlaceElement(currentElement, cursorPosition) ? cursorValid: cursorInvalid;
                    var renderers = cursor.transform.GetComponentsInChildren<MeshRenderer>();
                    foreach (var item in renderers)
                    {
                        item.material.SetColor("_BaseColor", color);
                    }*/


                    // is cursor out of bounds
                    if (!map.IsPositionValid(targetCursorPosition))
                    {
                        // play an error sound and signal some juicy way that we are at the end (for example bounce back and forth once in the direction of movement)
                        cursorMoved = false;

                    }
                    else
                        cursorMoved = true;
                }
            }
            else
            {
                cursorMoved = false;
            }
        }

        if (cursorStartedMoving && cursorDirection.magnitude == 0)
        {
            cursorDelayTimer.StopTimer();
            cursorDelayTimer.RestartTimer();
            cursorStartedMoving = false;
            cursorMoved = false;
        }

        cursor.transform.position = Vector3.Lerp(cursor.transform.position, targetCursorPosition, Time.deltaTime * cursorSpeed);

        DebugExtension.DebugArrow(cursor.transform.position + Vector3.up * 0.1f, cursorDirection.normalized * 10f);
        DebugExtension.DebugArrow(cursor.transform.position + Vector3.up * 0.1f, cameraDirection);
        DebugExtension.DebugArrow(cursor.transform.position + Vector3.up * 0.1f, neighbourDirectionPointer, Color.red);


        // Choosing element
        if (Input.GetButtonDown("ControllerAny Y"))
        {
            chosenElementIdx = 0; 
        }
        else if (Input.GetButtonDown("ControllerAny X"))
        {
            chosenElementIdx = 1; 
        }
        else if (Input.GetButtonDown("ControllerAny B"))
        {
            chosenElementIdx = 2; 
        }
        else if (Input.GetButtonDown("ControllerAny A"))
        {
            chosenElementIdx = 3; 
        }

        scoreManager.HighlightChosenSprite(chosenElementIdx);
        currentElement = database.elements[chosenElementIdx];

        // Placing element
        if (Input.GetButtonDown("ControllerAny Right Bumper"))
        {
            Debug.Log("Placing element: " + currentElement.type);
            //if (map.CanPlaceElement(currentElement, cursorPosition) && !scoreManager.isElementDisabled(chosenElementIdx))
            {
                Camera.main.transform.parent.GetComponent<MightyGamePack.CameraShaker>().ShakeOnce(2.2f, 1.5f, 0.3f, 0.65f);
                mainGameManager.UIManager.TriggerHitBlinkEffect(new Color(1, 1f, 1f, 0.05f));
                //GetComponent<MightyGamePack.TransformJuicer>().StartJuicing();

                Debug.Log("Can place object: " + currentElement.type + " at pos: " + cursorPosition);
                //map.PlaceElement(currentElement, cursorPosition);
                scoreManager.DisableElementSprite(chosenElementIdx);
            }
        }

        // Rotating element
        if (Input.GetButtonDown("ControllerAny Left Bumper"))
        {

        }
    }

    float Angle360(Vector3 from, Vector3 to, Vector3 right)
    {
        float angle = Vector3.Angle(from, to);
        return (Vector3.Angle(right, to) > 90f) ? 360f - angle : angle;
    }

    float Angle360OneToAnother(Vector3 from, Vector3 to, Vector3 right)
    {
        float angle = Clamp0360(Vector3.SignedAngle(from, to, -Vector3.up));
        return angle;
    }

    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }

    Vector3 GetNeighbourDirection()
    {
        // 4 sides
        float angle = neighbourDirectionPointerAngle;

        // 45-135
        if (angle >= 45 && angle < 135)
        {
            return new Vector3(1, 0, 0);
        }

        // 135-225
        if (angle >= 135 && angle < 225)
        {
            return new Vector3(0, 0, 1);
        }

        // 225-315
        if (angle >= 225 && angle < 315)
        {
            return new Vector3(-1, 0, 0);
        }

        // 315-45
        if ((angle >= 315 && angle <= 360) || (angle >= 0 && angle < 45))
        {
            return new Vector3(0, 0, -1);
        }

        throw new System.Exception("Go back to school");
    }
}
