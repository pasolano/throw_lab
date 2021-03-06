using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class HapticFeedback : MonoBehaviour
{
    // inspector vars
    [SerializeField]
    private bool vibrationOnReleasePub;
    [SerializeField]
    private bool pullTriggerForReleasePub;
    [SerializeField]
    private float triggerReleasePercentagePub;
    [SerializeField]
    private bool showHitIndicatorsPub;

    // logic controlling vars
    private bool vibrationOnRelease;
    private bool pullTriggerForRelease;
    private float triggerReleasePercentage;
    [HideInInspector]
    public bool showHitIndicators;

    public Direction userDominantHand;
    InputDevice leftDevice;
    InputDevice rightDevice;
    HapticCapabilities capabilities;
    XRGrabInteractable xrGrabInteractable;
    XRRayInteractor interactor;
    XRInteractionManager interMan;
    bool hitZeroState = true;
    float currTriggerPercentage;

    void Start()
    {
        // set logic controllers to inspector var values
        vibrationOnRelease = vibrationOnReleasePub;
        pullTriggerForRelease = pullTriggerForReleasePub;
        triggerReleasePercentage = triggerReleasePercentagePub;
        showHitIndicators = showHitIndicatorsPub;

        xrGrabInteractable = GetComponent<XRGrabInteractable>();
        interactor = GetDominantInteractor(userDominantHand);
        interMan = GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>();
        triggerReleasePercentage /= 100;

        // set to command-line values
        getIntent();
    }

    public enum Direction
    {
        Left,
        Right
    }

    // ensures trigger release percentage is set to a positive number
    private void OnValidate()
    {
        triggerReleasePercentage = Mathf.Clamp(triggerReleasePercentage, 0, 100f); // or int.MaxValue, if you need to use an int but can't use uint.
    }

    public void BuzzControllers()
    {
        if (vibrationOnRelease)
        {
            leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            
            HapticImpulse(leftDevice);
            HapticImpulse(rightDevice);
        }
    }

    public void HapticImpulse(InputDevice device)
    {
        if (device.TryGetHapticCapabilities(out capabilities))
                if (capabilities.supportsImpulse)
                    device.SendHapticImpulse(0, 1.0f, 0.25f);
    }

    public void DropObject()
    {
        interMan.SelectExit(interactor, xrGrabInteractable);
        if (vibrationOnRelease)
            BuzzControllers();
    }

    public void GrabObject()
    {
        interMan.SelectEnter(interactor, xrGrabInteractable);
    }

    public bool HoldingObject(Direction hand)
    {
        return interactor.selectTarget;
    }

    public static XRRayInteractor GetDominantInteractor(Direction hand)
    {
        GameObject controller = null;
        if (hand == Direction.Left)
            controller = GameObject.Find("LeftHand Controller");
        else if (hand == Direction.Right)
            controller = GameObject.Find("RightHand Controller");
        return controller.GetComponent<XRRayInteractor>();
    }

    void Update()
    {
        currTriggerPercentage = GetTriggerValue(userDominantHand);

        // if controller isn't holding anything
        if (!HoldingObject(userDominantHand) && hitZeroState)
        {
            // if trigger threshold passed
            // NOTE this is based on public trigger percentage
            if (triggerReleasePercentage <= currTriggerPercentage)
            {
                GrabObject();
                hitZeroState = false;
            }
        }

        // if the controller is holding something
        else
        {
            // if pull trigger for release object
            // pull waits for the trigger to go below threshhold, then listens for the trigger to go above the sensitivity
            if (pullTriggerForRelease)
            {
                    // listen for trigger pull
                    if ((triggerReleasePercentage <= currTriggerPercentage) && hitZeroState)
                    {
                        DropObject();
                        hitZeroState = false;
                    }
            }
            // if release trigger for release of object
            else
            {
                // if trigger percentage has dropped below the threshhold, drop object
                if (currTriggerPercentage < triggerReleasePercentage)
                    DropObject();
            }
        }

        // update zero state
        // NOTE this is based on public trigger percentage
        if (currTriggerPercentage < triggerReleasePercentage)
            hitZeroState = true;
    }

    private float GetTriggerValue(Direction hand)
    {
        List<UnityEngine.XR.InputDevice> handDevices = new List<UnityEngine.XR.InputDevice>();

        XRNode handDeviceReference;

        if (hand == Direction.Left)
            handDeviceReference = UnityEngine.XR.XRNode.LeftHand;
        else if (hand == Direction.Right)
            handDeviceReference =  UnityEngine.XR.XRNode.RightHand;
        else
        {
            handDeviceReference = UnityEngine.XR.XRNode.LeftHand; // to silence compile errors
            Debug.LogError("Function \"getTriggerValue\" was not passed parameter \"hand\"");
            throw new Exception("Function \"getTriggerValue\" was not passed parameter \"hand\"");
        }

        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(handDeviceReference, handDevices);

        UnityEngine.XR.InputDevice device = handDevices[0];

        var inputFeatures = new List<UnityEngine.XR.InputFeatureUsage>();

        if (device.TryGetFeatureUsages(inputFeatures))
        {
            foreach (var feature in inputFeatures)
            {
                if (feature.name == "Trigger")
                {
                    float featureValue;

                    if (device.TryGetFeatureValue(feature.As<float>(), out featureValue))
                    {
                        return featureValue;
                    } 
                }
            }
        }

        Debug.LogError("Function \"getTriggerValue\" is not returning a featureValue");
        throw new Exception("Function \"getTriggerValue\" is not returning a featureValue");
    }

    private void getIntent()
    { 
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        AndroidJavaObject extras = intent.Call<AndroidJavaObject> ("getExtras");
        
        bool hasPull = intent.Call<bool> ("hasExtra", "pull");
        if (hasPull)
            pullTriggerForRelease = extras.Call<bool> ("getBoolean", "pull");

        bool hasVibro = intent.Call<bool> ("hasExtra", "vibro");
        if (hasVibro)
            vibrationOnRelease = extras.Call<bool> ("getBoolean", "vibro");

        bool hasIndic = intent.Call<bool> ("hasExtra", "indic");
        if (hasIndic)
            showHitIndicators = extras.Call<bool> ("getBoolean", "indic");

        bool hasTiming = intent.Call<bool> ("hasExtra", "timing");
        if (hasTiming) {
            triggerReleasePercentage = extras.Call<float> ("getFloat", "timing");
            triggerReleasePercentage /= 100;
        }
    }
}
