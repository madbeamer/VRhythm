using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class KickPlayer : MonoBehaviour
{
#region Serialized fields
    [SerializeField]
    private AudioSource audioSource;

#endregion

#region Private fields
    private bool active;
    private bool previousStatePressed = false;
    private InputFeatureUsage<bool> playButton;
    private UnityEngine.XR.InputDevice? device;
#endregion

    // Start is called before the first frame update
    void Start()
    {
        playButton = UnityEngine.XR.CommonUsages.secondaryButton;
        if (audioSource != null) {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        List<UnityEngine.XR.InputDevice> rightHandControllers = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandControllers);
        device = rightHandControllers[0];
    }

    // Update is called once per frame
    void Update()
    {
        bool buttonPressed = false;
        if (!previousStatePressed && device.HasValue && device.Value.TryGetFeatureValue(playButton, out buttonPressed) && buttonPressed)
        {
            previousStatePressed = true;
            PlayKick();
        } else if (previousStatePressed && device.HasValue && device.Value.TryGetFeatureValue(playButton, out buttonPressed) && !buttonPressed)
        {
            previousStatePressed = false;
        }
    }

    private void PlayKick()
    {
        audioSource.Play();
    }

}
