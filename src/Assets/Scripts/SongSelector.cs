using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import the necessary namespace for UI components
using TMPro;
using System.IO;
using System;

public class SongSelector : MonoBehaviour
{
    // Step 1: Define a class to represent the song
    class Song
    {
        public string SongName { get; set; }
        public string Difficulty { get; set; }

        public Song(string songName, string difficulty)
        {
            SongName = songName;
            Difficulty = difficulty;
        }
    }
    private string dir = Path.Combine("Assets", "Audio", "Songs");

    // Step 2: Initialize a list with song objects
    List<Song> songList = new List<Song>{};

    public int SongIndex = 0;


    public GameObject EasyButton;
    public GameObject MediumButton;
    public GameObject HardButton;
    public GameObject ExpertButton;
    public GameObject SongTitle;


    // Start is called before the first frame update
    void Start()
    {
        string easy_folder = Path.Combine(dir, "easy");
        
        //get the folder of the song
        string[] easy_folder_songs = Directory.GetDirectories(easy_folder);
        for (int i = 0; i < easy_folder_songs.Length; i++)
        {
            songList.Add(new Song(Path.GetFileName(easy_folder_songs[i]), "easy"));
        }

        string medium_folder = Path.Combine(dir, "medium");
        string[] medium_folder_songs = Directory.GetDirectories(medium_folder);
        for (int i = 0; i < medium_folder_songs.Length; i++)
        {
            songList.Add(new Song(Path.GetFileName(medium_folder_songs[i]), "medium"));
        }

        string hard_folder = Path.Combine(dir, "hard");
        string[] hard_folder_songs = Directory.GetDirectories(hard_folder);
        for (int i = 0; i < hard_folder_songs.Length; i++)
        {
            songList.Add(new Song(Path.GetFileName(hard_folder_songs[i]), "hard"));
        }

        string expert_folder = Path.Combine(dir, "expert");
        string[] expert_folder_songs = Directory.GetDirectories(expert_folder);
        for (int i = 0; i < expert_folder_songs.Length; i++)
        {
            songList.Add(new Song(Path.GetFileName(expert_folder_songs[i]), "expert"));
        }

        setSongIndexTo(SongIndex);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void changeDifficultyTo(string difficulty)
    {
        for (int i = 0; i < songList.Count; i++)
        {
            if (songList[i].Difficulty == difficulty)
            {
                setSongIndexTo(i);
                return;
            }
        }
    }

    public void increaseSongIndex()
    {
        setSongIndexTo((SongIndex + 1) % songList.Count);
    }

    public void decreaseSongIndex()
    {
        setSongIndexTo((SongIndex - 1 + songList.Count) % songList.Count);
    }

    public void setSongIndexTo(int newSongIndex)
    {
        updateTitle(newSongIndex);

        if (songList[newSongIndex].Difficulty == "easy")
        {
            EasyButton.GetComponent<Image>().color = new Color32(255, 0, 226, 255);
            MediumButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            HardButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            ExpertButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            SongIndex = newSongIndex;
            return;
        }
        if (songList[newSongIndex].Difficulty == "medium")
        {
            EasyButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            MediumButton.GetComponent<Image>().color = new Color32(255, 0, 226, 255);
            HardButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            ExpertButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            SongIndex = newSongIndex;
            return;
        }
        if (songList[newSongIndex].Difficulty == "hard")
        {
            EasyButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            MediumButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            HardButton.GetComponent<Image>().color = new Color32(255, 0, 226, 255);
            ExpertButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            SongIndex = newSongIndex;
            return;
        }
        if (songList[newSongIndex].Difficulty == "expert")
        {
            EasyButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            MediumButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            HardButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            ExpertButton.GetComponent<Image>().color = new Color32(255, 0, 226, 255);
            SongIndex = newSongIndex;
            return;
        }
    }

    void updateTitle(int newSongIndex)
    {
        SongTitle.GetComponent<TextMeshProUGUI>().text = songList[newSongIndex].SongName;
    }
}
