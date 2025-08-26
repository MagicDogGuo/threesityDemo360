using UnityEngine;
using System.Collections;

public interface IHeadGazePointer
{
    void OnGazeEnabled();
    void OnGazeDisabled();

    void OnGazeStart(Camera camera, GameObject targetObject, Vector3 intersectionPosition, bool isInteractive);

    void OnGazeStay(Camera camera, GameObject targetObject, Vector3 intersectionPosition, bool isInteractive);

    void OnGazeExit(Camera camera, GameObject targetObject);

    void OnGazeTriggerStart(Camera camera);

    void OnGazeTriggerEnd(Camera camera);

    void GetPointerRadius(out float innerRadius, out float outerRadius);
}
