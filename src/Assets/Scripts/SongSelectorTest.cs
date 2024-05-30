using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Globalization;
using TMPro;

public class SongSelectorTest : MonoBehaviour
{
    // Start is called before the first frame update
    private string Difficulty = "Easy";
    private int Index = 0;
    public AudioSource audioSource;
    private List<float> timeTable = new List<float>();
    private List<int> drumLines = new List<int>();
    private string dir = Path.Combine("Assets", "Audio", "Songs");
    private TextMeshProUGUI SongTitle;
    public GameObject Manager;

    public GameObject EasyButton;
    public GameObject MediumButton;
    public GameObject HardButton;
    public GameObject ExpertButton;

    void Start()
    {
        SongTitle = GameObject.Find("SongTitle").GetComponent<TextMeshProUGUI>();
        DisplaySong();
    }
    //attach to difficulty buttons
    public void SetDifficulty(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        Difficulty = buttonText.text;
        Debug.Log($"Changed difficulty to {Difficulty}");
        Index = 0;
        DisplaySong();
    }
    //attach to right arrow button
    public void SetIndexRightArrow()
    {
        Index++;
        DisplaySong();
    }
    //attach to left arrow button
    public void SetIndexLeftArrow()
    {
        Index--;
        DisplaySong();
    }
    //not efficent but who cares
    private void DisplaySong()
    {
        string difficulty_folder = Path.Combine(dir, Difficulty);
        string[] difficulty_folder_songs = Directory.GetDirectories(difficulty_folder);
        int len = difficulty_folder_songs.Length;
        Index = ((Index % len) + len) % len;
        string song = difficulty_folder_songs[Index];
        SongTitle.text = Path.GetFileName(song);
        Color32 normalColor = new Color32(4, 80, 132, 255);
        Color32 selectedColor = new Color32(26, 131, 203, 255);

        if (Difficulty == "Easy")
        {
            EasyButton.GetComponent<Image>().color = selectedColor;
            MediumButton.GetComponent<Image>().color = normalColor;
            HardButton.GetComponent<Image>().color =normalColor;
            ExpertButton.GetComponent<Image>().color = normalColor;
        }
        else if (Difficulty == "Medium")
        {
            EasyButton.GetComponent<Image>().color = normalColor;
            MediumButton.GetComponent<Image>().color = selectedColor;
            HardButton.GetComponent<Image>().color =normalColor;
            ExpertButton.GetComponent<Image>().color = normalColor;
        }
        else if (Difficulty == "Hard")
        {
            EasyButton.GetComponent<Image>().color = normalColor;
            MediumButton.GetComponent<Image>().color =normalColor;
            HardButton.GetComponent<Image>().color = selectedColor;
            ExpertButton.GetComponent<Image>().color =normalColor;
        }
        else if (Difficulty == "Expert")
        {
            EasyButton.GetComponent<Image>().color = normalColor;
            MediumButton.GetComponent<Image>().color = normalColor;
            HardButton.GetComponent<Image>().color =normalColor;
            ExpertButton.GetComponent<Image>().color = selectedColor;
        }
        SetupSong();
    }

    //attach to start song button
    public void SetupSong(bool play = false)
    {
        drumLines.Clear();
        timeTable.Clear();
        //returning the .chart and the .wav paths
        string difficulty_folder = Path.Combine(dir, Difficulty);
        string folder_song = Directory.GetDirectories(difficulty_folder)[Index];

        Debug.Log($"In dir {difficulty_folder}; Reading lines for {folder_song}");

        StartCoroutine(LoadClip(Path.Combine(folder_song, "song.wav")));
        ReadCsv(Path.Combine(folder_song, "time_notes.txt"));
        GameObject drums = GameObject.Find("Drums");
        HashSet<int> DrumsNeeded = new HashSet<int>(drumLines);
        for (int i = 0; i < drums.transform.childCount; i++)
        {
            if (DrumsNeeded.Contains(i))
            {
                drums.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                drums.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        //start the song and remove the buttons
        if (play) {
            Manager.GetComponent<Manager>().PlaySong();
        }
    }
    private void ReadCsv(string filePath)
    {
        Debug.Log($"Getting CSV from {filePath}");
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
        }
    }
    private IEnumerator LoadClip(string audioClipPath)
    {
        //Assets/Audio/Songs/Easy/2 Tales of the Working Class/song.wav
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
                // transform.gameObject.SetActive(false);
            }
        }
    }
    public (List<float>, List<int>) GetTables()
    {
        // Returning same timetable and drumlines for all songs????
        return (timeTable, drumLines);
    }
}
