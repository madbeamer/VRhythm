using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// combo?
// adding indicators (bad, good, perfect) to the drum
// cannot detach major radius from minor radius
public class closingCircle : MonoBehaviour
{
    public GameObject torusPrefab;
    public GameObject particlesPrefab;
    public float[] rythm;

    private GameObject changedTorus;
    private Queue<GameObject> queue = new Queue<GameObject>();
    private float radiusDrum;
    private TextMeshProUGUI pointsText;
    private TextMeshProUGUI comboText;
    private bool missed = true;
    private int points = 0;
    private double combo = 1.0;
    private int ptsNextCombo = 0;

    private const float shrinkingTime = 1.0f;
    private const int maxPoints = 100;

    private IEnumerator ShrinkTorus(GameObject torus)
    {
        float startScale = torus.transform.localScale.x;
        while (torus.transform.localScale.x > 0)
        {
            float scaleChange = startScale / 1 * Time.deltaTime * shrinkingTime; // 1 is the shrinking time
            torus.transform.localScale -= new Vector3(scaleChange, 0, scaleChange);
            yield return null;
        }
        //destroy the torus
        Destroy(queue.Dequeue());
        //multiplier and points change if you missed the timing
        if (missed)
        {
            points -= 1;
            combo = 1.0;
            ptsNextCombo = 0;
            pointsText.text = "0";
            comboText.text = "x1,0";
        }
        else
        {
            missed = true;
        }
    }

    private IEnumerator SpawnTorus()
    {
        // adding rythm by making an array and then iterating over it by waiting for the time of the beat
        foreach (float beat in rythm)
        {
            GameObject newTorus = Instantiate(changedTorus);
            newTorus.SetActive(true);
            StartCoroutine(ShrinkTorus(newTorus));
            queue.Enqueue(newTorus);
            yield return new WaitForSeconds(beat);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // the queue is only used here
        int newPoints;
        if (queue.Count > 0)
        {
            GameObject smallTorus = queue.Peek();
            float radiusTorus = smallTorus.transform.localScale.x / 2;

            //particle system, change color and multiplier depending on the radius
            GameObject particles = Instantiate(particlesPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
            particles.transform.localScale = new Vector3(radiusTorus, radiusTorus, 1);

            //add points
            newPoints = (int)(radiusDrum / radiusTorus);
            //current points
            points += (int)(newPoints * combo);
            //check if the combo is increased
            ptsNextCombo += newPoints;
            if (ptsNextCombo >= maxPoints)
            {
                combo += 0.1;
                ptsNextCombo -= maxPoints;
                comboText.text = $"x{combo.ToString("F1")}";
            }
            //update the text
            pointsText.text = $"{points}";

            //destroy the torus
            missed = false;
            smallTorus.transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            //points lost if struck without a torus
            points -= 1;
            combo = 1.0;
            ptsNextCombo = 0;
            //update the text
            pointsText.text = "0";
            comboText.text = "x1,0";
        }

    }

    void Start()
    {
        // points text
        pointsText = GameObject.Find("PointsText").GetComponent<TextMeshProUGUI>();
        comboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();

        //diameter of the drums and position
        radiusDrum = transform.localScale.x / 2;

        //create modified clone of torus
        changedTorus = Instantiate(torusPrefab, transform.position + new Vector3(0, transform.localScale.y / 2, 0), transform.rotation);
        changedTorus.transform.localScale = new Vector3(radiusDrum, torusPrefab.transform.localScale.y, radiusDrum);

        StartCoroutine(SpawnTorus());
    }
}
