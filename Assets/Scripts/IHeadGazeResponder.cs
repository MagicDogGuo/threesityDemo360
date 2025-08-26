using UnityEngine;
using System.Collections;
public interface IHeadGazeResponder
{
    /// Called when the user is looking on a GameObject with this script,
    /// as long as it is set to an appropriate layer (see CardboardGaze).
    void OnGazeEnter();

    /// Called when the user stops looking on the GameObject, after OnGazeEnter
    /// was already called.
    void OnGazeExit();

    // Called when the Cardboard trigger is used, between OnGazeEnter
    /// and OnGazeExit.
    void OnGazeTrigger();
}
