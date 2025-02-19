using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CameraPositionCondition
{
    public enum CameraConditionEvents
    {
        OpenDialogue,
        EnableElement,
        DisbleElement,
        MoveTo,
        SetKeyTo
    }
    public GameVariableKeys key;
    public Vector2 interval;
    public string auxString;
    public float auxFloat;
    public Transform element;
    public UnityEvent eventOntrue;
    public CameraConditionEvents eventType;
}

[System.Serializable]
public class CameraPosition
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public CameraPositionCondition[] conditions;
}
[ExecuteInEditMode]
public class ScreenBahaviour : MonoBehaviour
{
    public static ScreenBahaviour Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Camera mainCamera;

    [Space(20)]
    public string curentCameraPosition;
    public bool setCurentCameraPositon;
    public bool moveToCameraPosition;
    public Transform worldButtons;
    public List<CameraPosition> cameras;

    [Space(20)]
    public float movementSpeed = 10;
    private Vector3 targetCameraLocation;
    private Quaternion targetCameraRotation;

    private void Start()
    {
        SetCameraLocation("Desk", true);
    }

    public void SetCameraLocation(string location)
    {
        SetCameraLocation(location, false);
    }
    IEnumerator DelayAction(float delayTime, Transform element, bool active)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        //Do the action after the delay time has finished.
        element.gameObject.SetActive(active);
    }

    public void SetCameraLocation(string location, bool instant)
    {
        if(location == "Desk")
            worldButtons.gameObject.SetActive(true);
        else worldButtons.gameObject.SetActive(false);

        foreach (var cam in cameras)
            if(cam.name == location)
            {
                foreach(var cond in cam.conditions)
                {
                    bool happen = true;
                    if(cond.key != GameVariableKeys.None)
                    {
                        var val = GameBehaviour.GetGlobalValue(cond.key.ToString());
                        happen = cond.interval.x < val && val < cond.interval.y;
                        //Debug.Log("key " + cond.key.ToString() + " " + val + " " + cond.interval + " " + happen);
                    }
                    if(happen)
                    {
                        cond.eventOntrue.Invoke();
                        switch(cond.eventType)
                        {
                            case CameraPositionCondition.CameraConditionEvents.DisbleElement:
                                if (cond.auxFloat <= 0)
                                    cond.element.gameObject.SetActive(false);
                                else StartCoroutine(DelayAction(cond.auxFloat, cond.element, false));
                                break;
                            case CameraPositionCondition.CameraConditionEvents.EnableElement:
                                if (cond.auxFloat <= 0)
                                    cond.element.gameObject.SetActive(true);
                                else StartCoroutine(DelayAction(cond.auxFloat, cond.element, true));
                                break;
                            case CameraPositionCondition.CameraConditionEvents.MoveTo:
                                break;
                            case CameraPositionCondition.CameraConditionEvents.SetKeyTo:
                                GameBehaviour.SetGlobalValue(cond.auxString, cond.auxFloat);
                                break;
                        }
                        //break;
                    }
                }
                curentCameraPosition = location;

                targetCameraLocation = cam.position;
                targetCameraRotation = cam.rotation;

                if(instant)
                {
                    mainCamera.transform.position = cam.position;
                    mainCamera.transform.rotation = cam.rotation;
                }

                break;
            }
    }

    private void Update()
    {
        if(Application.isPlaying)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraLocation, Time.deltaTime * movementSpeed);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetCameraRotation, Time.deltaTime * movementSpeed);
        }

        if(setCurentCameraPositon)
        {
            setCurentCameraPositon = false;
            bool found = false;
            foreach(CameraPosition cam in cameras)
                if(cam.name == curentCameraPosition)
                {
                    cam.position = mainCamera.transform.position;
                    cam.rotation = mainCamera.transform.rotation;
                    found = true;
                }
            if(!found)
            {
                cameras.Add(new CameraPosition()
                {
                    name = curentCameraPosition,
                    position = mainCamera.transform.position,
                    rotation = mainCamera.transform.rotation
                });
            }
        }
        if(moveToCameraPosition)
        {
            moveToCameraPosition = false;
            SetCameraLocation(curentCameraPosition, !Application.isPlaying);
        }
    }
}
