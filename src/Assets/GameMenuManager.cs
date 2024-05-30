using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GameMenuManager : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2.0f;
    public GameObject menu;

    private InputFeatureUsage<bool> toggleButton;

    public UnityEngine.XR.InputDevice? device = null;

    private bool previousStatePressed = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set variable togglebutton to CommonUsages secondary button.
        toggleButton = UnityEngine.XR.CommonUsages.secondaryButton;
    }

    // Update is called once per frame
    void Update()
    {
        List<UnityEngine.XR.InputDevice> leftHandControllers = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandControllers);
        if (leftHandControllers.Count > 0) {
            device = leftHandControllers[0];
            bool buttonPressed = false;
            if (!previousStatePressed && device.HasValue && device.Value.TryGetFeatureValue(toggleButton, out buttonPressed) && buttonPressed)
            {
                previousStatePressed = true;
                Debug.Log("Show button pressed");
                menu.SetActive(!menu.activeSelf);
                menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;
            }
            else if (previousStatePressed && device.HasValue && device.Value.TryGetFeatureValue(toggleButton, out buttonPressed) && !buttonPressed)
            {
                previousStatePressed = false;
            }

            menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
            menu.transform.forward *= -1;
        }
    }
}
