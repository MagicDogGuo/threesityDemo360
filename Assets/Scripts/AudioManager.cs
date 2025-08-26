using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private List<AudioSource> audioSources = new List<AudioSource>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get all AudioSource under this GameObject children
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
    }

    public void PlayByIndex(int index)
    {
        if (index < 0 || index >= audioSources.Count)
        {
            Debug.LogError("Index out of range");
            return;
        }
        audioSources[index].Play();
    }

}
