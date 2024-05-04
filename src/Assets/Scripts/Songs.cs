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
        var (audioClipPath, notesPath) = OpenNthFolder();

        StartCoroutine(LoadClip(audioSource, audioClipPath));
        ReadChartFile(notesPath);
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
    private void ReadChartFile(string filePath)
    {
        string diffDrum = $"[{difficultyIndex}Drums]";
        string line;
        string[] parts;
        float noBPM;
        List<(int, int)> syncLines = new List<(int, int)>();
        List<int> drumTime = new List<int>();
        //in song.ini there is the preview start time, i don't know what it does

        using (StreamReader sr = new StreamReader(filePath))
        {
            //find the resolution
            while ((line = sr.ReadLine()) != null && !line.StartsWith("  Resolution")) { }
            // i divide by 60 (seconds/minute) and by 1000 (because the bpm is *1000)
            noBPM = 60000f / float.Parse(line.Split(' ')[4]);

            //find the sync track
            while (sr.ReadLine() != "[SyncTrack]") { }
            //skip '{'
            sr.ReadLine();
            //read until the end of the sync track
            while ((line = sr.ReadLine()) != "}")
            {
                parts = line.Split(' ');
                //parts[4] = letter, parts[2] = tick, parts[5] = bpm
                //there is also 'TS' but i don't know what it does
                if (parts[4] == "B")
                {
                    //different bpm at different ticks
                    syncLines.Add((int.Parse(parts[2]), int.Parse(parts[5])));
                }
            }
            //find the drum track
            while (sr.ReadLine() != diffDrum) { }
            //skip '{'
            sr.ReadLine();
            //read until the end of the drum track
            while ((line = sr.ReadLine()) != "}")
            {
                parts = line.Split(' ');
                //parts[2] = tick, parts[4] = letter, parts[5] = int, parts[6] = int
                //there is also 'S' but i don't know what it does
                //there is also part[6] but i don't know what it does and it is always 0
                if (parts[4] == "N")
                {
                    drumTime.Add(int.Parse(parts[2]));
                    //type of drum
                    drumLines.Add(int.Parse(parts[5]));
                }
            }
        }

        //not to efficent but i don't think it will be a problem
        createTimeTable(drumTime, syncLines, noBPM);
    }
    //create the time teable that will be used to wait the time between the notes
    private void createTimeTable(List<int> drumTime, List<(int, int)> syncLines, float noBPM)
    {
        //in all songs there is a time delay when the songs start but i dont know how to get it
        //timeTable.Add(syncLines[0].Item1 * (noBPM / syncLines[0].Item2));
        int len1 = drumTime.Count - 2;
        int len2 = syncLines.Count;

        for (int i = 0; i < len1; i++)
        {
            // difference between the time of the notes
            int diff = drumTime[i + 1] - drumTime[i];
            for (int x = 0; x < len2; x++)
            {
                if (syncLines[x].Item1 <= drumTime[i] && (x == len2 - 1 || syncLines[x + 1].Item1 > drumTime[i + 1]))
                {
                    //current Ticks/Seconds multiplied by the difference between the notes
                    timeTable.Add(diff * (noBPM / syncLines[x].Item2));
                    break;
                }
            }
        }
    }

    private IEnumerator GetRhythm()
    {
        //i can check wich time point is nearest and wait for it by doing nothing
        //the note come before the song starts?????
        int len = timeTable.Count;
        for (int i = 0; i < len; i++)
        {
            yield return new WaitForSeconds(timeTable[i]);
            int a = drumLines[i];
            //what drum to play
            Debug.Log(a);
        }
    }

    //called by a button
    public void PlaySong()
    {
        //wait for the song to load
        audioSource.Play();
        StartCoroutine(GetRhythm());

    }
}