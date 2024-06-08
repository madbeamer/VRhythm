using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
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


    #region Private fields
    private bool active;
    private bool previousStatePressed = false;
    private InputFeatureUsage<bool> playButton;
    private UnityEngine.XR.InputDevice? device;
    private GameObject torusPrefab;
    #endregion

    void Start()
    {
        // Set playButton
        playButton = UnityEngine.XR.CommonUsages.secondaryButton;

        // points text
        manager = GameObject.Find("Drums").GetComponent<Manager>();
        particlesPrefab = Resources.Load<GameObject>("particlesPrefab");
        torusPrefab = Resources.Load<GameObject>("torusPrefab");
        //diameter of the drums and position
        radiusDrum = transform.lossyScale.x / 2; //maybe using the collider size
        //create modified clone of torus
    }

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
            manager.ResetCombo();
        }
        else
        {
            missed = true;
        }
    }

    public void SpawnTorus()
    {
        GameObject newTorus = Instantiate(torusPrefab, transform.position + new Vector3(0, transform.localScale.y / 2, 0), transform.rotation);
        newTorus.transform.localScale = new Vector3(radiusDrum, 0.1f, radiusDrum);
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
                GameObject particles = Instantiate(particlesPrefab, transform.position, Quaternion.LookRotation(transform.up));//change rotation of the particles
                particles.transform.localScale = new Vector3(radiusTorus, radiusTorus, 1);

                //add points
                int newPointsCombo = (int)(radiusDrum / radiusTorus); //multiply by int maybe
                //current points
                int newPoints = (int)(newPointsCombo * manager.GetCombo()); //could round to same number
                manager.AddPoints(newPoints, newPointsCombo);
                //destroy the torus
                missed = false;
                smallTorus.transform.localScale = new Vector3(0, 0, 0);
            }
            else
            {
                //points lost if struck without a torus
                manager.ResetCombo();
            }
        }

    }

    private void CheckPress()
    {
        List<UnityEngine.XR.InputDevice> rightHandControllers = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandControllers);
        if (rightHandControllers.Count > 0)
        {
            device = rightHandControllers[0];
            bool buttonPressed = false;
            if (!previousStatePressed && device.HasValue && device.Value.TryGetFeatureValue(playButton, out buttonPressed) && buttonPressed)
            {
                previousStatePressed = true;
                OnCollisionEnter(null);
            }
            else if (previousStatePressed && device.HasValue && device.Value.TryGetFeatureValue(playButton, out buttonPressed) && !buttonPressed)
            {
                previousStatePressed = false;
            }
        }
    }
    void Update()
    {
        if (manager.IsPlaying() && transform.tag == "KickCollider")
        {
            CheckPress();
        }
    }
}
