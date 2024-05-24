using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Globalization;

public class Songs : MonoBehaviour
{

    private string dir = Path.Combine("Assets", "Audio", "Songs");
    private AudioSource audioSource;
    //for songs that have multiple difficulties i duplicate them for each difficulty
    private List<float> timeTable = new List<float>();
    private List<int> drumLines = new List<int>();
    private HashSet<int> keysDrums;
    private const float shrinkingTime = 1.0f;
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

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        //returning the .chart and the .wav paths
        var (audioClipPath, CsvPath) = OpenNthFolder();

        StartCoroutine(LoadClip(audioSource, audioClipPath));
        ReadCsv(CsvPath);
    }

    // Call this method to open the n-th folder in the base directory
    private (string, string) OpenNthFolder()
    {
        string difficulty_folder = Path.Combine(dir, difficultyIndex.ToString());
        //get the folder of the song
        string folder_song = Directory.GetDirectories(difficulty_folder)[songIndex];
        // Open the folder
        return (Path.Combine(folder_song, "song.wav"), Path.Combine(folder_song, "time_notes.txt"));
    }

    private IEnumerator LoadClip(AudioSource audioSource, string audioClipPath)
    {
        string filePath = "file:///" + Path.GetFullPath(audioClipPath);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Get the downloaded clip
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                // Assign the downloaded clip to the AudioSource
                audioSource.clip = clip;
            }
        }
    }
    private void ReadCsv(string filePath)
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            //read csv to get the time table and the drum lines
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split('/');
                timeTable.Add(float.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat));
                drumLines.Add(int.Parse(parts[1]));
            }
            timeTable[0] -= shrinkingTime; // account for the shrinking time
            keysDrums = new HashSet<int>(drumLines);
        }
    }
    //create the time teable that will be used to wait the time between the notes

    public (List<float>, List<int>) GetTables()
    {
        return (timeTable, drumLines);
    }
    public HashSet<int> GetKeysDrums()
    {
        return keysDrums;
    }
}