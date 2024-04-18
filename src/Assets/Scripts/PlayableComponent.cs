using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayableComponent : MonoBehaviour
{
    private readonly string BASE_AUDIO_DIRECTORY = Path.Combine("Assets", "Audio", "Samples");
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int altIndex;

    public int AltIndex
    {
        get { return altIndex; }
        set 
        { 
            altIndex = value;
            UpdateAudioClip();
        }
    }

    [SerializeField] private ComponentType componentType;

    public ComponentType ComponentType
    {
        get { return componentType; }
        set 
        { 
            componentType = value;
            UpdateAudioClip();
        }
    }


    /// <summary>
    /// Is called before any and all calls to Update()
    /// </summary>
    private void Start()
    {
        InstantiateAudioSource();
        UpdateAudioClip();
    }

    /// <summary>
    ///  Is called when this collider/rigidbody has begun touching another rigidbody/collider
    /// </summary>
    /// <param name="collision">Collision information, e.g. contact points and impact velocity</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (audioSource != null/* &&
            collision.relativeVelocity.magnitude > 2*/)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Checks whether <c cref="AudioSource">AudioSource</c> component exists, in which case we get it, otherwise we instantiate.
    /// </summary>
    private void InstantiateAudioSource()
    {
        if (audioSource == null)
        {
            if (gameObject.GetComponent<AudioSource>() == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            } else
            {
                audioSource = gameObject.GetComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Updates property <c cref="audioSource.clip">audioClip</c> of this script's 
    /// <c cref="audioSource">audioSource</c> field with the clip at index 
    /// <c cref="altIndex">altIndex</c> from the folder with the same name as this 
    /// script's assigned <c cref="componentType">componentType</c>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the value of field <c cref="altIndex">Is less than 0 or greater than the number of available clips</c></exception>
    private void UpdateAudioClip()
    {
        if (altIndex < 0)
        {
            throw new ArgumentOutOfRangeException($"Invalid soundfont index {altIndex}. Should be greater than 0 [{componentType}]");
        }
        // Get whole path, i.e. base dir + component-specific folder
        string componentDir = Path.Combine(BASE_AUDIO_DIRECTORY, componentType.ToString());
        // Read all files from that folder
        string[] clipFiles = Directory.GetFiles(componentDir, "*.wav");
        if (altIndex >= clipFiles.Length)
        {
            throw new ArgumentOutOfRangeException($"Invalid soundfont index {altIndex}. Should NOT be greater than or equal to {clipFiles.Length} [{componentType}]");
        }
        // Get audio clip's filename by index of file in folder
        string newClipFile = clipFiles[altIndex];
        Debug.Log($"Getting clip at directory {newClipFile}");
        // If the file doesn't exist, I crye (T~T)
        if (!File.Exists(newClipFile))
        {
            throw new Exception($"Nani desuka? File {newClipFile} don't exist, y'all");
        }

        // Load the clip's file asynchronously and set the AudioClip to the AudioSource
        StartCoroutine(LoadClip(newClipFile, clip =>
        {
            InstantiateAudioSource();
            audioSource.clip = clip;
            Debug.Log(clip);
        }));
    }

    /// <summary>
    /// Loads a file from its full path relative to the project's directory, and returns the corresponding AudioClip object.
    /// </summary>
    /// <param name="filePath">Full path of the file to convert to an AudioClip object, relative to the project's directory.</param>
    /// <returns></returns>
    private IEnumerator LoadClip(string filePath, UnityAction<AudioClip> onClipLoaded)
    {
        // Convert file's path to absolute
        filePath = Path.GetFullPath(filePath);

        // Convert local path to file URL
        string fileUrl = "file:///" + filePath;
        Debug.Log(fileUrl);

        // Create a UnityWebRequest to get the AudioClip
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileUrl, AudioType.WAV);

        // Send the request and wait for it to finish
        yield return www.SendWebRequest();

        // If there was an error, log it and return null
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load AudioClip: " + www.error);
            onClipLoaded?.Invoke(null);
        }

        // Otherwise, extract the AudioClip from the DownloadHandlerAudioClip and return it
        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
        onClipLoaded?.Invoke(clip);
    }
}
