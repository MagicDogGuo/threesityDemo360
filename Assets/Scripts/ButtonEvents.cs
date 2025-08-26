using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour 
{
    private GameObject gaze;
    void Start()
    {
        gaze = GameObject.FindGameObjectWithTag("Gaze");
    }

    /// <summary>
    /// PictureButton
    /// </summary>
    public void OnPointEnterPictureButton()
    {
        Debug.Log("Enter Picture Button" );
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("PictureButton");
    }
    public void OnPointExitPictureButton()
    {
        Debug.Log("Exit Picture Button" );
        gaze.GetComponent<GazeObject>().OnGazeEventExit("PictureButton");
    }

    /// <summary>
    /// Previous video Button
    /// </summary>
    public void OnPointEnterPreviousButton()
    {
        Debug.Log("Enter Previous Button");
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("PreviousButton");
    }
    public void OnPointExitPreviousButton()
    {
        Debug.Log("Exit Previous Button");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("PreviousButton");
    }


    /// <summary>
    /// Next video Button
    /// </summary>
    public void OnPointEnterNextButton()
    {
        Debug.Log("Enter Next Button" );
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("NextButton");
    }
    public void OnPointExitNextButton()
    {
        Debug.Log("Exit Next Button" );
        gaze.GetComponent<GazeObject>().OnGazeEventExit("NextButton");
    }


    /// <summary>
    /// VideoButton
    /// </summary>
    public void OnPointEnterVideoButton()
    {
        Debug.Log("Enter Video Button" );
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("VideoButton");
    }
    public void OnPointExitVideoButton()
    {
        Debug.Log("Exit Video Button");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("VideoButton");
    }


    /// <summary>
    /// ExitButton
    /// </summary>
    public void OnPointEnterExitButton()
    {
        Debug.Log("Enter Exit Button" );
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("ExitButton");
    }
    public void OnPointExitExitButton()
    {

        Debug.Log("Exit Exit Button");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("ExitButton");
    }

    /// <summary>
    /// LeftArrowButton
    /// </summary>
    public void OnPointEnterLeftArrowButton()
    {
        Debug.Log("EnterLeftArrowButton");
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("LeftArrowButton");
    }
    public void OnPointExitLeftArrowButton()
    {
        Debug.Log("ExitLeftArrowButton");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("LeftArrowButton");
    }

    /// <summary>
    /// RightArrowButton
    /// </summary>
    public void OnPointEnterRightArrowButton()
    {
        Debug.Log("EnterRightArrowButton");
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("RightArrowButton");
    }
    public void OnPointExitRightArrowButton()
    {
        Debug.Log("ExitRightArrowButton");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("RightArrowButton");
    }
    /// <summary>
    /// PlayButton
    /// </summary>
    public void OnPointEnterPlayButton()
    {
        Debug.Log("OnPointEnterPlayButton");
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("PlayButton");

    }
    public void OnPointExitPlayButton()
    {
        Debug.Log("OnPointExitPlayButton");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("PlayButton");
    }
    /// <summary>
    /// PauseButton
    /// </summary>
    public void OnPointEnterPauseButton()
    {
        Debug.Log("OnPointEnterPauseButton");
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("PauseButton");
    }
    public void OnPointExitPauseButton()
    {
        Debug.Log("OnPointExitPauseButton");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("PauseButton");
    }
    /// <summary>
    /// StopButton
    /// </summary>
    public void OnPointEnterStopButton()
    {
        Debug.Log("OnPointEnterStopButton");
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("StopButton");
    }
    public void OnPointExitStopButton()
    {
        Debug.Log("OnPointExitStopButton");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("StopButton");
    }
    /// <summary>
    /// ReturnButton
    /// </summary>
    public void OnPointEnterReturnButton()
    {
        Debug.Log("OnPointEnterReturnButton");
        gaze.GetComponent<GazeObject>().OnGazeEventEnter("ReturnButton");
    }
    public void OnPointExitReturnButton()
    {
        Debug.Log("OnPointExitReturnButton");
        gaze.GetComponent<GazeObject>().OnGazeEventExit("ReturnButton");
    }

    
}
