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
    private List<float> timeTable = new List<float>();
    private List<int> drumLines = new List<int>();
    private HashSet<string> DrumsNeeded = new HashSet<string>();
    private const float shrinkingTime = 1.0f;
    private Dictionary<int, string> Drums = new Dictionary<int, string>
    {
        //for more info: https://github.com/TheNathannator/GuitarGame_ChartFormats/blob/main/doc/FileFormats/.chart/Drums.md
        {0, "Kick"},
        {1, "Red"},
        {2, "Yellow"},
        {3, "Blue"},
        {4, "5-lane Orange, 4-lane Green"},
        {5, "5-lane Green"},
        {32, "Expert + kick / 2x kick"},
        {34, "Red accent modifier"},
        {35, "Yellow accent modifier"},
        {36, "Blue accent modifier"},
        {37, "5-lane Orange, 4-lane Green accent modifier"},
        {38, "5-lane Green accent modifier"},
        {40, "Red ghost modifier"},
        {41, "Yellow ghost modifier"},
        {42, "Blue ghost modifier"},
        {43, "5-lane Orange, 4-lane Green ghost modifier"},
        {44, "5-lane Green ghost modifier"},
        {66, "Yellow cymbal modifier"},
        {67, "Blue cymbal modifier"},
        {68, "Green cymbal modifier"}
    };
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
                timeTable.Add(float.Parse(parts[0]));
                drumLines.Add(int.Parse(parts[1]));
            }
            timeTable[0] -= shrinkingTime; // account for the shrinking time
            HashSet<int> keysDrums = new HashSet<int>(drumLines);
            foreach (int key in keysDrums)
            {
                DrumsNeeded.Add(Drums[key]);
            }
        }
    }
    //create the time teable that will be used to wait the time between the notes

    public (List<float>, List<int>) GetTables()
    {
        return (timeTable, drumLines);
    }
}