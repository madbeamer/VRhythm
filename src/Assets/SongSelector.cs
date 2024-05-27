using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import the necessary namespace for UI components
using TMPro;

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

    // Step 2: Initialize a list with song objects
    List<Song> songList = new List<Song>
    {
        new Song("2 Tales of The Working Class", "easy"),
        new Song("Vril Society", "medium"),
        new Song("Swan Song", "hard"),
        new Song("Living In a Dream", "expert")
    };

    public int SongIndex = 0;


    public GameObject EasyButton;
    public GameObject MediumButton;
    public GameObject HardButton;
    public GameObject ExpertButton;
    public GameObject SongTitle;


    // Start is called before the first frame update
    void Start()
    {

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
