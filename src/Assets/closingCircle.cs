using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// combo?
// adding indicators (bad, good, perfect) to the drum
// cannot detach major radius from minor radius
public class closingCircle : MonoBehaviour
{
    public GameObject torusPrefab;
    public float shrinkingtime;
    public float[] rythm;

    private GameObject changedTorus;
    private Queue<GameObject> queue = new Queue<GameObject>();
    private float diameterDrum;
    private Text pointsText;

    private IEnumerator ShrinkTorus(GameObject torus)
    {
        float startScale = torus.transform.localScale.x;
        while (torus.transform.localScale.x > 0)
        {
            float scaleChange = startScale / shrinkingtime * Time.deltaTime;
            torus.transform.localScale -= new Vector3(scaleChange, scaleChange, 0);
            yield return null;
        }
        Destroy(queue.Dequeue());
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
        int newPoints;
        // the queue is only used here
        if (queue.Count > 0)
        {
            GameObject smallTorus = queue.Dequeue();
            newPoints = (int)(diameterDrum / smallTorus.transform.localScale.x); // * multiplier
            Destroy(smallTorus);
        }
        else
        {
            newPoints = -1; // cheange the amount of points lost
        }
        pointsText.text = (int.Parse(pointsText.text) + newPoints).ToString();
    }

    void Start()
    {
        // points text
        pointsText = GameObject.Find("PointsText").GetComponent<Text>();

        //diameter of the drums and position
        diameterDrum = transform.localScale.x;

        //create modified clone of torus
        changedTorus = Instantiate(torusPrefab);
        changedTorus.transform.localScale = new Vector3(diameterDrum, diameterDrum, torusPrefab.transform.localScale.z);
        changedTorus.transform.position = transform.position;

        StartCoroutine(SpawnTorus());
    }
}
