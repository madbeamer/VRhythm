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
    private Manager manager;
    private GameObject particlesPrefab;
    private GameObject changedTorus;
    private Queue<GameObject> queue = new Queue<GameObject>();
    private float radiusDrum;
    private bool missed = true;
    private const float shrinkingTime = 1.0f;

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
            manager.AddPoints(-1);
            manager.ResetCombo();
        }
        else
        {
            missed = true;
        }
    }

    public void SpawnTorus()
    {
        GameObject newTorus = Instantiate(changedTorus);
        newTorus.SetActive(true);
        StartCoroutine(ShrinkTorus(newTorus));
        queue.Enqueue(newTorus);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (manager.IsPlaying())
        {
            // the queue is only used here
            if (queue.Count > 0)
            {
                GameObject smallTorus = queue.Peek();
                float radiusTorus = smallTorus.transform.localScale.x / 2;

                //particle system, change color and multiplier depending on the radius
                GameObject particles = Instantiate(particlesPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
                particles.transform.localScale = new Vector3(radiusTorus, radiusTorus, 1);

                //add points
                int newPointsCombo = (int)(radiusDrum / radiusTorus);
                manager.AddptsNextCombo(newPointsCombo);
                //current points
                int newPoints = (int)(newPointsCombo * manager.GetCombo());
                manager.AddPoints(newPoints);
                //destroy the torus
                missed = false;
                smallTorus.transform.localScale = new Vector3(0, 0, 0);
            }
            else
            {
                //points lost if struck without a torus
                manager.AddPoints(-1);
                manager.ResetCombo();
            }
        }

    }

    void Start()
    {
        // points text
        manager = GameObject.Find("Drums").GetComponent<Manager>();
        particlesPrefab = Resources.Load<GameObject>("particlesPrefab");
        GameObject torusPrefab = Resources.Load<GameObject>("torusPrefab");
        //diameter of the drums and position
        radiusDrum = transform.localScale.x / 2;
        //create modified clone of torus
        changedTorus = Instantiate(torusPrefab, transform.position + new Vector3(0, transform.localScale.y / 2, 0), transform.rotation);
        changedTorus.transform.localScale = new Vector3(radiusDrum, torusPrefab.transform.localScale.y, radiusDrum);
    }
}
