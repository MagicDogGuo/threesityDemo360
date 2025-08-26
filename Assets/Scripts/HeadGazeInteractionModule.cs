using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

[AddComponentMenu("XR/HeadGazeInteraction")]
public class HeadGazeInteractionModule : BaseInputModule
{
   

    /// Time in seconds between the pointer down and up events sent by a Cardboard trigger.
    /// Allows time for the UI elements to make their state transitions.
    [HideInInspector]
    public float clickTime = 0.1f;  // Based on default time for a button to animate to Pressed.

    /// The pixel through which to cast rays, in viewport coordinates.  Generally, the center
    /// pixel is best, assuming a monoscopic camera is selected as the `Canvas`' event camera.
    [HideInInspector]
    public Vector2 hotspot = new Vector2(0.5f, 0.5f);

    [Tooltip("assign the XR camera")]
    [SerializeField]
    private Transform XRcam;

    private PointerEventData pointerData;
    private Vector2 lastHeadPose;

    /// The ICardboardGazePointer which will be responding to gaze events.
    public static IHeadGazePointer headGazePointer;
    // Active state
    private bool isActive = false;
    
    /// @cond
    public override bool ShouldActivateModule()
    {
        bool activeState = base.ShouldActivateModule();


        if (activeState != isActive)
        {
            isActive = activeState;

            // Activate gaze pointer
            if (headGazePointer != null)
            {
                if (isActive)
                {
                    headGazePointer.OnGazeEnabled();
                    Debug.Log("Gaze pointer enabled");
                }
            }
        }

        return activeState;
    }

    public override void DeactivateModule()
    {
        DisableGazePointer();
        base.DeactivateModule();
        if (pointerData != null)
        {
            HandlePendingClick();
            HandlePointerExitAndEnter(pointerData, null);
            pointerData = null;
        }
        eventSystem.SetSelectedGameObject(null, GetBaseEventData());
    }

    public override bool IsPointerOverGameObject(int pointerId)
    {
        return pointerData != null && pointerData.pointerEnter != null;
    }

    public override void Process()
    {
        // Save the previous Game Object
        GameObject gazeObjectPrevious = GetCurrentGameObject();

        CastRayFromGaze();
        UpdateCurrentObject();
        UpdateReticle(gazeObjectPrevious);

        // Handle input
        if (!Input.GetMouseButtonDown(0) && Input.GetMouseButton(0))
        {
            HandleDrag();
        }
        else if (Time.unscaledTime - pointerData.clickTime < clickTime)
        {
            // Delay new events until clickTime has passed.
        }
        else if (!pointerData.eligibleForClick &&
                  Input.GetMouseButtonDown(0))
        {
            // New trigger action.
            HandleTrigger();
        }
        else if (!Input.GetMouseButton(0))
        {
            // Check if there is a pending click to handle.
            HandlePendingClick();
        }
    }
    /// @endcond

    private void CastRayFromGaze()
    {
        Vector2 headPose = NormalizedCartesianToSpherical(XRcam.transform.forward);

        if (pointerData == null)
        {
            pointerData = new PointerEventData(eventSystem);
            lastHeadPose = headPose;
        }

        // Cast a ray into the scene
        pointerData.Reset();
        pointerData.position = new Vector2(hotspot.x * Screen.width, hotspot.y * Screen.height);
        eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
        
        // Debug显示所有射线碰到的UI物件
        if (m_RaycastResultCache.Count > 0)
        {
            Debug.Log("=== UI射线检测结果 (共" + m_RaycastResultCache.Count + "个) ===");
            for (int i = 0; i < m_RaycastResultCache.Count; i++)
            {
                var result = m_RaycastResultCache[i];
                Debug.Log($"UI物件[{i}]: {result.gameObject.name} (距离: {result.distance:F2})");
            }
        }
        
        // 同时进行Physics射线检测来检测3D物件
        Camera cam = null;
        if (XRcam != null)
        {
            cam = XRcam.GetComponent<Camera>();
        }
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        if (cam != null)
        {
            Ray ray = cam.ScreenPointToRay(new Vector3(hotspot.x * Screen.width, hotspot.y * Screen.height, 0));
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
            
            if (hits.Length > 0)
            {
                Debug.Log("=== 3D物件射线检测结果 (共" + hits.Length + "个) ===");
                System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance)); // 按距离排序
                
                for (int i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];
                    Debug.Log($"3D物件[{i}]: {hit.collider.gameObject.name} (距离: {hit.distance:F2})");
                }
            }
        }
        
        pointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_RaycastResultCache.Clear();
        pointerData.delta = headPose - lastHeadPose;
        lastHeadPose = headPose;
    }

    private void UpdateCurrentObject()
    {
        // Send enter events and update the highlight.
        var go = pointerData.pointerCurrentRaycast.gameObject;
        HandlePointerExitAndEnter(pointerData, go);
        // Update the current selection, or clear if it is no longer the current object.
        var selected = ExecuteEvents.GetEventHandler<ISelectHandler>(go);
        if (selected == eventSystem.currentSelectedGameObject)
        {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(),
                                  ExecuteEvents.updateSelectedHandler);
        }
        else
        {
            eventSystem.SetSelectedGameObject(null, pointerData);
        }
    }

    void UpdateReticle(GameObject previousGazedObject)
    {
        if (headGazePointer == null)
        {
            return;
        }

        Camera camera = pointerData.enterEventCamera; // Get the camera
        GameObject gazeObject = GetCurrentGameObject(); // Get the gaze target
        Vector3 intersectionPosition = GetIntersectionPosition();
        bool isInteractive = pointerData.pointerPress != null ||
            ExecuteEvents.GetEventHandler<IPointerClickHandler>(gazeObject) != null;

        if (gazeObject == previousGazedObject)
        {
            if (gazeObject != null)
            {
                headGazePointer.OnGazeStay(camera, gazeObject, intersectionPosition, isInteractive);
            }
        }
        else
        {
            if (previousGazedObject != null)
            {
                headGazePointer.OnGazeExit(camera, previousGazedObject);
            }

            if (gazeObject != null)
            {
                headGazePointer.OnGazeStart(camera, gazeObject, intersectionPosition, isInteractive);
            }
        }
    }

    private void HandleDrag()
    {
        bool moving = pointerData.IsPointerMoving();

        if (moving && pointerData.pointerDrag != null && !pointerData.dragging)
        {
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData,
                ExecuteEvents.beginDragHandler);
            pointerData.dragging = true;
        }

        // Drag notification
        if (pointerData.dragging && moving && pointerData.pointerDrag != null)
        {
            // Before doing drag we should cancel any pointer down state
            // And clear selection!
            if (pointerData.pointerPress != pointerData.pointerDrag)
            {
                ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);

                pointerData.eligibleForClick = false;
                pointerData.pointerPress = null;
                pointerData.rawPointerPress = null;
            }
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.dragHandler);
        }
    }

    private void HandlePendingClick()
    {
        if (!pointerData.eligibleForClick && !pointerData.dragging)
        {
            return;
        }

        if (headGazePointer != null)
        {
            Camera camera = pointerData.enterEventCamera;
            headGazePointer.OnGazeTriggerEnd(camera);
        }

        var go = pointerData.pointerCurrentRaycast.gameObject;

        // Send pointer up and click events.
        ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);
        if (pointerData.eligibleForClick)
        {
            ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);
        }
        else if (pointerData.dragging)
        {
            ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.dropHandler);
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.endDragHandler);
        }

        // Clear the click state.
        pointerData.pointerPress = null;
        pointerData.rawPointerPress = null;
        pointerData.eligibleForClick = false;
        pointerData.clickCount = 0;
        pointerData.clickTime = 0;
        pointerData.pointerDrag = null;
        pointerData.dragging = false;
    }

    private void HandleTrigger()
    {
        var go = pointerData.pointerCurrentRaycast.gameObject;

        // Send pointer down event.
        pointerData.pressPosition = pointerData.position;
        pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
        pointerData.pointerPress =
          ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.pointerDownHandler)
            ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(go);

        // Save the drag handler as well
        pointerData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(go);
        if (pointerData.pointerDrag != null)
        {
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.initializePotentialDrag);
        }

        // Save the pending click state.
        pointerData.rawPointerPress = go;
        pointerData.eligibleForClick = true;
        pointerData.delta = Vector2.zero;
        pointerData.dragging = false;
        pointerData.useDragThreshold = true;
        pointerData.clickCount = 1;
        pointerData.clickTime = Time.unscaledTime;

        if (headGazePointer != null)
        {
            headGazePointer.OnGazeTriggerStart(pointerData.enterEventCamera);
        }
    }

    private Vector2 NormalizedCartesianToSpherical(Vector3 cartCoords)
    {
        cartCoords.Normalize();
        if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;
        float outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
        if (cartCoords.x < 0)
            outPolar += Mathf.PI;
        float outElevation = Mathf.Asin(cartCoords.y);
        return new Vector2(outPolar, outElevation);
    }

    GameObject GetCurrentGameObject()
    {
        if (pointerData != null && pointerData.enterEventCamera != null)
        {
            return pointerData.pointerCurrentRaycast.gameObject;
        }

        return null;
    }

    Vector3 GetIntersectionPosition()
    {
        // Check for camera
        Camera cam = pointerData.enterEventCamera;
        if (cam == null)
        {
            return Vector3.zero;
        }

        float intersectionDistance = pointerData.pointerCurrentRaycast.distance + cam.nearClipPlane;
        Vector3 intersectionPosition = cam.transform.position + cam.transform.forward * intersectionDistance;

        return intersectionPosition;
    }

    void DisableGazePointer()
    {
        if (headGazePointer == null)
        {
            return;
        }

        GameObject currentGameObject = GetCurrentGameObject();
        if (currentGameObject)
        {
            Camera camera = pointerData.enterEventCamera;
            headGazePointer.OnGazeExit(camera, currentGameObject);
        }

        headGazePointer.OnGazeDisabled();
        Debug.Log("Gaze pointer disabled");
    }
}