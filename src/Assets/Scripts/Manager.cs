using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using System;
using UnityEngine.XR.Interaction.Toolkit;
//adding here the start song??
public class Manager : MonoBehaviour
{
    private List<GameObject> children = new List<GameObject>();
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI comboText;
    private int points = 0;
    private double combo = 1.0;
    private int ptsNextCombo = 0;
    private const int maxPoints = 100;
    private bool isPlaying = false;
    private string[] keytoDrum = new string[] {
        "Tom1",
        "Tom2",
        "FloorTom",
        "Snare",
        "Kick",
        "Crash",
        "Ride",
        "Hihat",
    };
    public AudioSource audioSource;
    private const float shrinkingTime = 1.0f;

    private closingCircle[] drums = new closingCircle[8];
    public GameObject PlayingMenu;
    public GameObject SongSelector;
    // Start is called before the first frame update
    void Start()
    {


        foreach (Transform drum in transform)
        {
            int index = Array.IndexOf(keytoDrum, drum.name);
            GameObject collider = drum.Find("Collider").gameObject;
            collider.AddComponent<closingCircle>();
            drums[index] = collider.GetComponent<closingCircle>();
        }
    }
    // Public method to set points
    public void AddPoints(int pts, int ptsCombo)
    {
        points += pts;
        pointsText.text = points.ToString();

        ptsNextCombo += ptsCombo;
        if (ptsNextCombo >= maxPoints)
        {
            combo += 0.1;
            comboText.text = "x" + combo.ToString("F1", CultureInfo.InvariantCulture);
            ptsNextCombo -= maxPoints;
        }
    }

    public double GetCombo()
    {
        return combo;
    }

    public void ResetCombo()
    {
        points -= 1;
        pointsText.text = points.ToString();

        combo = 1.0;
        ptsNextCombo = 0;
        comboText.text = "x1.0";
    }

    // Public method to set points
    public bool IsPlaying()
    {
        return isPlaying;
    }

    private IEnumerator GetRhythm(List<float> timeTable, List<int> drumLines)
    {
        int len = timeTable.Count;
        for (int i = 0; i < len; i++)
        {
            if (timeTable[i] != 0)
            {
                yield return new WaitForSeconds(timeTable[i]);
            }
            drums[drumLines[i]].SpawnTorus();
            //what drum to play
        }
        isPlaying = false;
    }

    //called by a button
    public void PlaySong()
    {

        isPlaying = true;
        (List<float> timeTable, List<int> drumLines) = SongSelector.GetComponent<SongSelectorTest>().GetTables();
        timeTable[0] -= shrinkingTime;
        //wait for the song to load
        audioSource.Play();
        StartCoroutine(GetRhythm(timeTable, drumLines));
        foreach (Transform drum in transform)
        {
            GameObject collider = drum.Find("Collider").gameObject;
            drum.GetComponent<XRGrabInteractable>().enabled = false;
            GetComponent<Collider>().GetComponent<AudioSource>().enabled = false;
        }
        PlayingMenu.SetActive(true);
        SongSelector.SetActive(false);
    }
}
