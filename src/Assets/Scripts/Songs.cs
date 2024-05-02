using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Songs : MonoBehaviour
{

    private string dir = Path.Combine("Assets", "Audio", "Songs");
    private AudioSource audioSource;
    //for songs that have multiple difficulties i duplicate them for each difficulty
    public enum DifficultyIndex
    {
        Easy,
        Medium,
        Hard,
        Expert,
    }
    public DifficultyIndex difficultyIndex;
    // Set the index of the folder to open
    public int songIndex;

    private void ReadChartFile(string filePath)
    {
        List<string> syncLines = new List<string>();
        List<string> drumLines = new List<string>();
        string diffDrum = $"[{difficultyIndex}Drums]";

        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while (sr.ReadLine() != "[SyncTrack]") { }
            sr.ReadLine();


            while ((line = sr.ReadLine()) != "}") { syncLines.Add(line); }

            while (sr.ReadLine() != diffDrum) { }
            sr.ReadLine();
            while ((line = sr.ReadLine()) != "}") { drumLines.Add(line); }
        }
    }

    private IEnumerator LoadClip(AudioSource audioSource, string audioClipPath)
    {
        string filePath = "file:///" + Path.GetFullPath(audioClipPath);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }

    // Call this method to open the n-th folder in the base directory
    private (string, string) OpenNthFolder()
    {
        string difficulty_folder = Path.Combine(dir, difficultyIndex.ToString());
        //get the folder of the song
        string folder_song = Directory.GetDirectories(difficulty_folder)[songIndex];
        // Open the folder
        return (Path.Combine(folder_song, "song.wav"), Path.Combine(folder_song, "notes.chart"));
    }

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        var (audioClipPath, notesPath) = OpenNthFolder();

        StartCoroutine(LoadClip(audioSource, audioClipPath));
        ReadChartFile(notesPath);
    }
}