using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DrumstickToggler : MonoBehaviour
{
    #region Serialized fields
    [SerializeField]
    private bool leftController = true;
    [SerializeField]
    private bool rightController = false;
    [SerializeField]
    private bool activeOnWake = false;
    [SerializeField]
    private Collider collider;
    #endregion

    #region Private fields
    private bool active;
    private bool previousStatePressed = false;
    InputFeatureUsage<bool> toggleButton;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        active = activeOnWake;
        toggleButton = UnityEngine.XR.CommonUsages.primaryButton;

        if (collider != null) {
            collider = gameObject.GetComponent<Collider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleButton != null)
        {
            UnityEngine.XR.InputDevice? device = null;
            if (leftController)
            {
                List<UnityEngine.XR.InputDevice> leftHandControllers = new List<UnityEngine.XR.InputDevice>();
                InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandControllers);
                device = leftHandControllers[0];
            } else if (rightController)
            {
                List<UnityEngine.XR.InputDevice> rightHandControllers = new List<UnityEngine.XR.InputDevice>();
                InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandControllers);
                device = rightHandControllers[0];
            }

            bool buttonPressed = false;
            if (!previousStatePressed && device.HasValue && device.Value.TryGetFeatureValue(toggleButton, out buttonPressed) && buttonPressed)
            {
                previousStatePressed = true;
                ToggleGameObject();
            } else if (previousStatePressed && device.HasValue && device.Value.TryGetFeatureValue(toggleButton, out buttonPressed) && !buttonPressed)
            {
                previousStatePressed = false;
            }
        }
    }

    private void ToggleGameObject()
    {
        active = !active;
        if (active)
        {
            gameObject.transform.localScale = new(1.0f, 1.0f, 1.0f);
            collider.enabled = true;
        } else
        {
            gameObject.transform.localScale = new(0.0f, 0.0f, 0.0f);
            collider.enabled = false;
        }
    }
}
