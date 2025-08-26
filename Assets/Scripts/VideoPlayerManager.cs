using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class VideoPlayerManager : MonoBehaviour
{
    public MediaPlayer mediaPlayer; // Reference to the AVPro Video MediaPlayer
    public string videosPath = "360Videos"; // Configurable path to the folder containing the videos
    private string[] videoFileNames; // Array to hold the video file paths
    private int currentVideoIndex = 0; // Track the current video index
    private bool isLoaded = false; // Flag to check if videos are loaded

    // Add a list of your video file names here (without extension)
    // This is necessary because we can't enumerate files in StreamingAssets on Android
    [SerializeField]
    private string[] knownVideoNames = { "video1", "video2", "video3" }; // Add your actual video names here

    // Start is called before the first frame update
    void Start()
    {
        // Initialize media player (make sure you have a MediaPlayer component in the scene)
        if (mediaPlayer == null)
        {
            mediaPlayer = GetComponent<MediaPlayer>();
        }

        // Load video paths
        StartCoroutine(LoadVideoPathsCoroutine());
    }

    IEnumerator LoadVideoPathsCoroutine()
    {
        List<string> validVideos = new List<string>();

        // Check if we're on a platform that supports direct file access
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.LinuxEditor)
        {
            // Editor: Use the old method
            LoadVideoPathsEditor();
            yield break;
        }

        // For Android and other platforms, check each known video file
        foreach (string videoName in knownVideoNames)
        {
            string videoPath = Path.Combine(videosPath, videoName + ".mp4");
            string fullPath = Path.Combine(Application.streamingAssetsPath, videoPath);

            Debug.Log($"Checking video: {fullPath}");

            // Use UnityWebRequest to check if file exists
            using (UnityWebRequest www = UnityWebRequest.Get(fullPath))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    validVideos.Add(fullPath);
                    Debug.Log("Video file found: " + fullPath);
                }
                else
                {
                    Debug.LogWarning("Video file not found: " + fullPath);
                }
            }
        }

        videoFileNames = validVideos.ToArray();

        if (videoFileNames.Length == 0)
        {
            Debug.LogError("No video files found in StreamingAssets/" + videosPath);
        }

        isLoaded = true;
    }

    void LoadVideoPathsEditor()
    {
        // Construct the absolute path to the videos folder
        string path = Path.Combine(Application.streamingAssetsPath, videosPath);

        Debug.Log($"Editor path: {path}");

        if (Directory.Exists(path))
        {
            // Get all video files (e.g., .mp4, .mov) in the folder
            string[] videoFiles = Directory.GetFiles(path, "*.mp4", SearchOption.TopDirectoryOnly);

            videoFileNames = new string[videoFiles.Length];

            // Save the paths of the video files
            for (int i = 0; i < videoFiles.Length; i++)
            {
                videoFileNames[i] = videoFiles[i];
                Debug.Log("Video file found: " + videoFileNames[i]);
            }
        }
        else
        {
            videoFileNames = new string[0];
            Debug.LogError("The folder does not exist: " + path);
        }

        isLoaded = true;
    }

    // Load the video by index
    void LoadVideo(int index)
    {
        // Ensure the index is within bounds
        if (index >= 0 && index < videoFileNames.Length)
        {
            // Set the media path
            //mediaPlayer.SetMediaSource(MediaSource.Path);

            // On Android, use the StreamingAssets path directly
            string videoPath = videoFileNames[index];

            //mediaPlayer.SetMediaPath(new MediaPath(videoPath, MediaPathType.AbsolutePathOrURL));

            //mediaPlayer.OpenMedia();
            Debug.Log("Playing video-1: " + videoPath);
            mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, videoPath, false);
            mediaPlayer.Play();
            Debug.Log("Playing video: " + videoPath);
        }
        else
        {
            Debug.LogError("Invalid video index: " + index);
        }
    }

    void UnloadVideo()
    {
        if (mediaPlayer != null && mediaPlayer.VideoOpened)
        {
            mediaPlayer.Stop();
            mediaPlayer.CloseVideo();
            Debug.Log("Video unloaded.");
        }
    }

    public void PlayVideo()
    {
        if (!isLoaded)
        {
            Debug.LogWarning("Videos are still loading...");
            return;
        }

        if (mediaPlayer.VideoOpened)
        {
            ResumeVideo();
            return;
        }

        if (videoFileNames.Length > 0)
        {
            LoadVideo(currentVideoIndex);
        }
        else
        {
            Debug.LogError("No video files found in the specified folder: " + videosPath);
        }
    }

    // Switch to the next video in the list
    public void PlayNextVideo()
    {
        if (!isLoaded || videoFileNames.Length == 0) return;

        UnloadVideo();
        // Increment the index and loop around if necessary
        currentVideoIndex = (currentVideoIndex + 1) % videoFileNames.Length;
        LoadVideo(currentVideoIndex);
    }

    // Switch to the previous video in the list
    public void PlayPreviousVideo()
    {
        if (!isLoaded || videoFileNames.Length == 0) return;

        UnloadVideo();
        // Decrement the index and loop around if necessary
        currentVideoIndex = (currentVideoIndex - 1 + videoFileNames.Length) % videoFileNames.Length;
        LoadVideo(currentVideoIndex);
    }

    // Pause the current video
    public void PauseVideo()
    {
        if (mediaPlayer.Control.IsPlaying())
        {
            mediaPlayer.Pause();
        }
    }

    // Resume the current video
    public void ResumeVideo()
    {
        if (!mediaPlayer.Control.IsPlaying())
        {
            mediaPlayer.Play();
        }
    }

    // Stop the current video
    public void StopVideo()
    {
        if(mediaPlayer.VideoOpened) UnloadVideo();
    }
}