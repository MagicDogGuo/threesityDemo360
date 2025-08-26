using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public GameObject pictureButton;
    public GameObject videoButton;
    public GameObject exitButton;
    

    public GameObject leftArrowButton;
    public GameObject rightArrowButton;

    public GameObject playButton;
    public GameObject nextButton;
    public GameObject previousButton;
    public GameObject pauseButton;
    public GameObject stopButton;
    public GameObject returnButton;
    
    public Texture2D[] panoramicTextures;
    private int textureIndex;

    private bool isSelectPicture;

    public GameObject spherePicture;
    public GameObject sphereVideo;
    public VRCameraFade camFade;

    [SerializeField]
    private VideoPlayerManager videoPlayerManger;
	// Use this for initialization
	void Start ()
    {
        textureIndex = 0;
        HideSphere();
        ShowDefaultButton();
        HideArrowButton();
        HideVideoButton();
	}
    #region common functions
    void SetSphereTexture(Texture2D texture)
    {
        spherePicture.GetComponent<MeshRenderer>().material.mainTexture = texture;
    }
    
    void HideSphere()
    {
        spherePicture.SetActive(false);
        sphereVideo.SetActive(false);
    }
    
    void ShowDefaultButton()
    {
        pictureButton.SetActive(true);
        videoButton.SetActive(true);
        exitButton.SetActive(true);
    }
    void HideDefaultButton()
    {
        pictureButton.SetActive(false);
        videoButton.SetActive(false);
        exitButton.SetActive(false);
    }

    void ShowArrowButton()
    {
        leftArrowButton.SetActive(true);
        rightArrowButton.SetActive(true);
        returnButton.SetActive(true);
    }
    void HideArrowButton()
    {
        leftArrowButton.SetActive(false);
        rightArrowButton.SetActive(false);
        returnButton.SetActive(false);
    }

    void ShowVideoButton()
    {
        //playButton.SetActive(true);
        pauseButton.SetActive(true);
        stopButton.SetActive(true);
        returnButton.SetActive(true);
        nextButton.SetActive(true);
        previousButton.SetActive(true);
    }
    void HideVideoButton()
    {
        playButton.SetActive(false);
        pauseButton.SetActive(false);
        stopButton.SetActive(false);
        returnButton.SetActive(false);
        nextButton.SetActive(false);
        previousButton.SetActive(false);
    }
    void Return()
    {
        textureIndex = 0;
        spherePicture.SetActive(false);
        videoPlayerManger.StopVideo();
        sphereVideo.SetActive(false);
        ShowDefaultButton();
        HideArrowButton();
        HideVideoButton();
    }
    public void OnReturnButtonClick()
    {
        Debug.Log("OnReturnButtonClick");
        Return();
    }
    #endregion

    #region MainMenu buttons
    public void OnPictureButtonClick()
    {
        Debug.Log("OnPictureButtonClick");
        spherePicture.SetActive(true);
        HideDefaultButton();
        ShowArrowButton();
        isSelectPicture = true;
    }
    public void OnVideoButtonClick()
    {
        Debug.Log("OnVideoButtonClick");
        sphereVideo.SetActive(true);
        spherePicture.SetActive(false);
        HideDefaultButton();
        HideArrowButton();
        ShowVideoButton();

        isSelectPicture = false;
        videoPlayerManger.PlayVideo();
    }
    public void OnExitButtonClick()
    {
        Debug.Log("OnExitButtonClick");
        // check if the application is running in the editor
        #if UNITY_EDITOR
            // if so, stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    #endregion

    #region image control buttons
    public void OnLeftArrowButtonClick()
    {
        Debug.Log("OnLeftArrowButtonClick");
        if(isSelectPicture)
        {
            camFade.FadeIn(1f);
            textureIndex = (textureIndex - 1 + panoramicTextures.Length) % panoramicTextures.Length;
            SetSphereTexture(panoramicTextures[textureIndex]);
        }
    }
    public void OnRightArrowButtonClick()
    {
        Debug.Log("OnRightArrowButtonClick");
        if (isSelectPicture)
        {
            camFade.FadeIn(1f);
            textureIndex = (textureIndex + 1) % panoramicTextures.Length;
            SetSphereTexture(panoramicTextures[textureIndex]);
        }
    }
    #endregion

    #region video control buttons
    public void OnNextVideoButtonClick()
    {
        Debug.Log("OnNextVideoButtonClick");
        videoPlayerManger.PlayNextVideo();
        playButton.SetActive(false);
        pauseButton.SetActive(true);
    }
    public void OnPreviousVideoButtonClick()
    {
        Debug.Log("OnPreviousVideoButtonClick");
        videoPlayerManger.PlayPreviousVideo();
        playButton.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void OnPlayButtonClick()
    {
        Debug.Log("OnPlayButtonClick");
        videoPlayerManger.PlayVideo();
        playButton.SetActive(false);
        pauseButton.SetActive(true);
    }
    public void OnPauseButtonClick()
    {
        Debug.Log("OnPauseButtonClick");
        videoPlayerManger.PauseVideo();
        playButton.SetActive(true);
        pauseButton.SetActive(false);
    }
    public void OnStopButtonClick()
    {
        Debug.Log("OnStopButtonClick");
        videoPlayerManger.StopVideo();
        playButton.SetActive(true);
        pauseButton.SetActive(false);
    }
    #endregion
    
}
