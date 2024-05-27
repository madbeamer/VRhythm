using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import the necessary namespace for UI components
using TMPro;


public class SongSelector : MonoBehaviour
{
    List<string> easySongs = new List<string> {
        "2 Tales of The Working Class",
        "Vril Society",
        "Swan Song",
        "Living In a Dream",
    };
    public GameObject EasyButton;

    public GameObject MediumButton;

    public GameObject HardButton;

    public GameObject ExpertButton;

    public GameObject SongTitle;

    public int SongIndex;


    // Start is called before the first frame update
    void Start()
    {
        setSongIndexTo(SongIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void increaseSongIndex()
    {
        setSongIndexTo((SongIndex + 1)%4);
    }

    public void decreaseSongIndex()
    {
        setSongIndexTo((SongIndex - 1)%4);
    }

    public void setSongIndexTo(int newSongIndex)
    {
        updateTitle(newSongIndex);
        if (newSongIndex == SongIndex)
        {
            return;
        }
        if (newSongIndex == 0)
        {
            EasyButton.GetComponent<Image>().color = new Color32(255, 0, 226, 255);
            MediumButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            HardButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            ExpertButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            SongIndex = newSongIndex;
            return;
        }
        if (newSongIndex == 1)
        {
            EasyButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            MediumButton.GetComponent<Image>().color = new Color32(255, 0, 226, 255);
            HardButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            ExpertButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            SongIndex = newSongIndex;
            return;
        }
        if (newSongIndex == 2)
        {
            EasyButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            MediumButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            HardButton.GetComponent<Image>().color = new Color32(255, 0, 226, 255);
            ExpertButton.GetComponent<Image>().color = new Color32(166, 0, 148, 255);
            SongIndex = newSongIndex;
            return;
        }
        if (newSongIndex == 3)
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
        SongTitle.GetComponent<TextMeshProUGUI>().text = easySongs[newSongIndex];
        
        return;
    }
    
}
