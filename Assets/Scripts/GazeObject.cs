using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GazeObject : MonoBehaviour, IHeadGazePointer 
{
    public delegate void OnWaitTrigger();
    public static OnWaitTrigger OnWaitEvent;

    public delegate void OnButtonWaitTrigger(string buttonName);
    public static OnButtonWaitTrigger OnButtonWaitEvent;

    public float waitTime = 2;

    private bool start = false;

    private float timeGo = 0;

    public Image gazePoint;
    public Image gazeLoading;

    private string buttonName;
	// Use this for initialization
	void Start () 
    {
        GazeReset();
        start = false;

        
        HeadGazeInteractionModule.headGazePointer = this;
	}
	
    public void OnGazeEventEnter(string buttonName)
    {
        start = true;
        this.buttonName = buttonName;
    }

    public void OnGazeEventExit(string buttonName )
    {
        start = false;
        this.buttonName = buttonName;
        GazeReset();
    }

    public void GazeReset()
    {
        gazePoint.enabled = true;
        gazeLoading.enabled = false;
        gazeLoading.fillAmount = 0;
        timeGo = 0;
    }

    public void OnGazeEnabled() { }
    public void OnGazeDisabled() { }
    public void OnGazeStart(Camera camera, GameObject targetObject, Vector3 intersectionPosition, bool isInteractive) 
    {
        Debug.Log("OnGazeStart");
    }
    public void OnGazeStay(Camera camera, GameObject targetObject, Vector3 intersectionPosition, bool isInteractive) 
    {
        Debug.Log(camera.name + " OnGazeStay " + targetObject.name + " isInteractive: " + isInteractive);
        gazePoint.enabled = false;
        gazeLoading.enabled = true;

        gazeLoading.fillAmount = timeGo / waitTime;
        timeGo += Time.deltaTime;
        if (timeGo > waitTime)
        {
            start = false;
            GazeReset();
            Debug.Log("Trigger button");
            //if (OnWaitEvent != null) OnWaitEvent();

            //if (OnButtonWaitEvent != null) OnButtonWaitEvent(buttonName);
            // try to get Button on targetObject or its parent
            // if get the button component, invoke the onClick event
            if (targetObject.GetComponentInParent<Button>() != null) targetObject.GetComponentInParent<Button>().onClick.Invoke();
            //targetObject.GetComponent<Button>()?.onClick.Invoke();

        }
    }
    public void OnGazeExit(Camera camera, GameObject targetObject) 
    {
        Debug.Log("OnGazeExit");
        GazeReset();
    }
    public void OnGazeTriggerStart(Camera camera) 
    {
        Debug.Log("OnGazeTriggerStart");
    }
    public void OnGazeTriggerEnd(Camera camera) 
    {
        Debug.Log("OnGazeTriggerEnd");
    }
    public void GetPointerRadius(out float innerRadius, out float outerRadius)
    {
        innerRadius = 0.1f;
        outerRadius = 0.2f;
    }

}
