using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DrumstickToggler : MonoBehaviour
{
    #region Public fields
    [SerializeField]
    private bool leftController = true;
    [SerializeField]
    private bool rightController = false;
    [SerializeField]
    private InputHelpers.Button toggleButton; // The button to toggle the GameObject
    #endregion

    #region Private fields
    private bool active = false;
    private InputDevice controller; // Reference to the XR controller
    private InputFeatureUsage<bool> toggleButtonUsage; // InputFeatureUsage for the toggle button
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        toggleButtonUsage = InputHelpers.(toggleButton);
        gameObject.SetActive(active);
        if (leftController)
        {
            controller = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        } else if (rightController)
        {
            controller = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controller != null)
        {
            if (controller.isValid && controller.TryReadSingleValue(toggleButton, out float buttonState) && (buttonState < 1e-05))
            {
                active = !active;
                gameObject.SetActive(active);
            }
        }
    }
}
