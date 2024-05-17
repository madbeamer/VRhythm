using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//adding here the start song??
public class Manager : MonoBehaviour
{
    private AudioSource audioSource;
    private List<GameObject> children = new List<GameObject>();
    private TextMeshProUGUI pointsText;
    private TextMeshProUGUI comboText;
    private int points = 0;
    private double combo = 1.0;
    private int ptsNextCombo = 0;
    private const int maxPoints = 100;
    private bool isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pointsText = GameObject.Find("PointsText").GetComponent<TextMeshProUGUI>();
        comboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
            child.gameObject.AddComponent<closingCircle>();
        }
    }
    // Public method to set points
    public void AddPoints(int value)
    {
        points += value;
        pointsText.text = $"{points}";
    }
    public double GetCombo()
    {
        return combo;
    }
    public void ResetCombo()
    {
        combo = 1.0;
        ptsNextCombo = 0;
        comboText.text = $"1,0";
    }

    // Public method to set points
    public void AddptsNextCombo(int value)
    {
        ptsNextCombo += value;
        if (ptsNextCombo >= maxPoints)
        {
            combo += 0.1;
            comboText.text = $"x{combo.ToString("F1")}";
            ptsNextCombo -= maxPoints;
        }
    }
    public bool IsPlaying()
    {
        return isPlaying;
    }
    private IEnumerator GetRhythm(List<float> timeTable, List<int> drumLines)
    {
        int len = timeTable.Count;
        for (int i = 0; i < len; i++)
        {
            yield return new WaitForSeconds(timeTable[i]);
            int a = drumLines[i];
            children[0].GetComponent<closingCircle>().SpawnTorus();
            //what drum to play
        }
        isPlaying = false;
    }

    //called by a button
    public void PlaySong()
    {
        isPlaying = true;
        (List<float> timeTable, List<int> drumLines) = transform.GetComponent<Songs>().GetTables();
        //wait for the song to load
        audioSource.Play();
        StartCoroutine(GetRhythm(timeTable, drumLines));

    }
}
