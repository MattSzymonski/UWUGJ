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
    public Vector3 cursorPosition = new(5, 0, 5); // TODO: if out of bounds, check here
    public Color cursorInvalid;
    public Color cursorValid;

    [ReadOnly] public float cursorAngle;
    [ReadOnly] public float cursorCameraAngle;
    [ReadOnly] public Vector3 neighbourDirection;

    [ReadOnly] public Vector3 neighbourDirectionPointer;
    [ReadOnly] public float neighbourDirectionPointerAngle;
    [ReadOnly] public Vector3 targetCursorPosition;

    [ReadOnly]  public float targetGhostRotation;
    public float ghostRotationSpeed;

    [Header("Element")]
    public Element ghostElement;
    public Element elementToSpawn;
    public Material ghostMaterial;

    bool dpadUp;
    bool triggersActivated;
    public Vector3 cursorDirection;
    [ReadOnly] public float cursorMagnitude;
    [ReadOnly] public bool cursorMoved;
    [ReadOnly] public bool cursorStartedMoving;
    private MightyGamePack.MightyTimer cursorDelayTimer;

    public ScoreManager scoreManager;
    public Database database;
    int chosenElementIdx;
    int lastChosenElementIdx;


    void Start()
    {
        var manager = mainGameManager.timersManager;
        cursorDelayTimer = manager.CreateTimer("CursorDelayTimer", 0.005f, 2f, false, true); // Create new timer (Not looping, stopped on start)
        //ghostElement = database.elements[0]; // TODO: change into what is chosen as first element in ScoreManager! (0th element as well?)
        cursor.transform.position = cursorPosition;
        targetCursorPosition = cursorPosition;
        //elementToSpawn = Instantiate(ghostElement, cursor.transform);
        // TODO: add some kind of tint or glow or alpha to signal it is not fully placed
        chosenElementIdx = -1;
        lastChosenElementIdx = -1;
    }

    public void ResetIndex()
    {
        lastChosenElementIdx = -1;
    }

    public void RestartGame()
    {
        var elements = map.gameObject.GetComponentsInChildren<Element>();
        foreach (var item in elements)
        {
            Destroy(item.gameObject);
        }



    }

    void Update()
    {
        if (mainGameManager.gameState != MightyGamePack.GameState.Playing)
        {
            return;
        }

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


        if (!cursorMoved)
        {
            if (cursorMagnitude > 0.01f)
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

                    if (map.IsPositionValid(cursorPosition + neighbourDirection))
                    {
                        cursorPosition += neighbourDirection;
                        cursorPosition = new Vector3(cursorPosition.x, map.GetTileHeight((int)cursorPosition.x, (int)cursorPosition.z), cursorPosition.z);
                        targetCursorPosition = cursorPosition;

                        // Ghost color
                        Color color = cursorInvalid;
                        if (elementToSpawn != null)
                        {
                            color = map.CanPlaceElement(elementToSpawn, cursorPosition) ? cursorValid : cursorInvalid;
                            var renderers = elementToSpawn.transform.GetComponentsInChildren<MeshRenderer>();
                            foreach (var item in renderers)
                            {
                                foreach (var material in item.materials)
                                {
                                    material.SetColor("_BaseColor", color);
                                }
                            }
                        }

                        cursorMoved = true;
                    }
                    else
                    {
                        Debug.Log("Invalid cursor position");
                        // TODO Juice
                    }
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


        lastChosenElementIdx = chosenElementIdx;
        // Choosing element
        if (Input.GetButtonDown("ControllerAny Y"))
        {
            chosenElementIdx = 0; 
        }
        else if (Input.GetButtonDown("ControllerAny X"))
        {
            chosenElementIdx = 1; 
        }
        else if (Input.GetButtonDown("ControllerAny A"))
        {
            chosenElementIdx = 2; 
        }
        else if (Input.GetButtonDown("ControllerAny B"))
        {
            chosenElementIdx = 3; 
        }

        // Choosing element
        if (lastChosenElementIdx != chosenElementIdx) // TODO: when a refresh of level happened we can enter here and fail to select FIXME:
        {
            if (!scoreManager.IsElementDisabled(chosenElementIdx))
            {
                targetGhostRotation = 0;

                if (elementToSpawn)
                {
                    Destroy(elementToSpawn.gameObject);
                }

                scoreManager.HighlightChosenSprite(chosenElementIdx);

                ghostElement = database.thisLevelChoice[chosenElementIdx]; // TODO: this is wrong, alter the database choices

                // TODO: juice it up, and replace current element with a newly chosen one, spawn new one and remove this one
                elementToSpawn = Instantiate(ghostElement, cursor.transform);
                // TODO: add some kind of tint or glow or alpha to signal it is not fully placed

                // Ghost color
                Color color = cursorInvalid;
                if (elementToSpawn != null)
                {
                    color = map.CanPlaceElement(elementToSpawn, cursorPosition) ? cursorValid : cursorInvalid;
                    var renderers = elementToSpawn.transform.GetComponentsInChildren<MeshRenderer>();
                    foreach (var item in renderers)
                    {
                        Material[] materials = new Material[item.materials.Length]; // <-- CREATING THE TEMPORARY ARRAY
                        for (int j = 0; j < materials.Length; ++j)
                        {
                            materials[j] = ghostMaterial;
                            materials[j].SetColor("_BaseColor", color);
                        }

                        item.materials = materials; // <-- ASSIGNING THE WHOLE ARRAY

                        //for (int i = 0; i < item.materials.Length; i++)
                        //{
                        //    Debug.Log("old" + item.materials[i].name);
                        //    item.materials[i] = ghostMaterial;
                        //    Debug.Log("new" +  item.materials[i].name);
                        //    item.materials[i].SetColor("_BaseColor", color);
                        //}
                    }
                }
            }
        }

        // Placing element
        if (Input.GetButtonDown("ControllerAny Right Bumper") && elementToSpawn)
        {
            Debug.Log("Placing element: " + elementToSpawn.type);
            if (!scoreManager.IsElementDisabled(chosenElementIdx) && map.CanPlaceElement(elementToSpawn, cursorPosition))
            {
                Debug.Log("Can place object: " + elementToSpawn.type + " at pos: " + cursorPosition);
                //map.PlaceElement(elementToSpawn, cursorPosition);

                // Finally remove ghost and spawn real element
                Destroy(elementToSpawn.gameObject);
                elementToSpawn = null;

                // Spawn real
                var finalElementToSpawn = Instantiate(ghostElement, cursor.transform);
                finalElementToSpawn.transform.parent = GameObject.Find("Map").transform;
                finalElementToSpawn.transform.position = cursorPosition;
                finalElementToSpawn.transform.eulerAngles = new Vector3(0, targetGhostRotation, 0);
                finalElementToSpawn.rotation = (int)Clamp0360(targetGhostRotation);
                finalElementToSpawn.GetComponent<MightyGamePack.TransformJuicer>().StartJuicing();
                mainGameManager.audioManager.PlayRandomSound("pop1", "pop2", "pop3", "pop4", "pop5");
                map.PlaceElement(finalElementToSpawn, cursorPosition);

                // Some juice
                Camera.main.transform.parent.GetComponent<MightyGamePack.CameraShaker>().ShakeOnce(2.2f, 1.5f, 0.3f, 0.65f);
                mainGameManager.UIManager.TriggerHitBlinkEffect(new Color(1, 1f, 1f, 0.05f));
                
                // TODO: should be placed in a container somewhere?
                scoreManager.DisableElementSprite(chosenElementIdx);
                cursorPosition = new Vector3(cursorPosition.x, map.GetTileHeight((int)cursorPosition.x, (int)cursorPosition.z), cursorPosition.z);
                targetCursorPosition = cursorPosition;
            }
        }

        // Rotating element clockwise
        if (Input.GetButtonDown("ControllerAny Left Bumper") && elementToSpawn)
        {
            targetGhostRotation = targetGhostRotation + 90.0f;
            if (targetGhostRotation >= 360 )
            {
                targetGhostRotation = 0.0f;
            }

            Debug.Log("rotated element, rotation: " + elementToSpawn.rotation);
        }

        if (elementToSpawn != null)
        {
            float rotationAnimated = Mathf.Lerp(elementToSpawn.transform.eulerAngles.y, targetGhostRotation, Time.deltaTime * ghostRotationSpeed);
            elementToSpawn.transform.eulerAngles = new Vector3(0, Clamp0360(rotationAnimated), 0);
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
