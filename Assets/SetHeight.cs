using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class SetHeight : MonoBehaviour
{
    Camera mainCamera;
    Transform mainCameraTransform;
    XRRayInteractor domHand;
    HapticFeedback hapFeed;
    UnityEngine.XR.InputDevice device;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gSphere = GameObject.Find("GrabSphere");
        hapFeed = gSphere.GetComponent<HapticFeedback>();
        domHand = HapticFeedback.GetDominantInteractor(hapFeed.userDominantHand);

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        mainCameraTransform = mainCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (ButtonPushed())
        {
            // move bullseye to eye-level
            float viewHeight = mainCameraTransform.position.y;
            Transform bullseye = GameObject.Find("Bullseye").transform;
            Vector3 bsOrigLocalPos = bullseye.localPosition;
            Vector3 bullseyePos = bullseye.position;
            bullseyePos.y = viewHeight;
            bullseye.position = bullseyePos;
            
            // move balloon with bullseye offset
            var balloon = GameObject.Find("Balloon").transform;
            var balloonPos = balloon.position;
            balloonPos.y -= bsOrigLocalPos.y - bullseye.localPosition.y;
            balloon.position = balloonPos;
        }
    }

    // returns if the requested button is pressed
    bool ButtonPushed()
    {
        List<UnityEngine.XR.InputDevice> handDevices = new List<UnityEngine.XR.InputDevice>();

        XRNode handDeviceReference;

        if (hapFeed.userDominantHand == HapticFeedback.Direction.Left)
            handDeviceReference = UnityEngine.XR.XRNode.LeftHand;
        else if (hapFeed.userDominantHand == HapticFeedback.Direction.Right)
            handDeviceReference =  UnityEngine.XR.XRNode.RightHand;
        else
        {
            handDeviceReference = UnityEngine.XR.XRNode.LeftHand; // to silence compile errors
            Debug.LogError("Function \"ButtonPushed\" was not passed parameter \"hand\"");
            throw new Exception("Function \"ButtonPushed\" was not passed parameter \"hand\"");
        }

        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(handDeviceReference, handDevices);

        device = handDevices[0];

        var inputFeatures = new List<UnityEngine.XR.InputFeatureUsage>();

        if (device.TryGetFeatureUsages(inputFeatures))
        {
            Debug.Log("Devices:\n");
            foreach (var feature in inputFeatures)
            {
                Debug.Log(feature.name + "\n");
                if (feature.name == "PrimaryButton")
                {
                    bool featureValue;

                    device.TryGetFeatureValue(feature.As<bool>(), out featureValue);
                    return featureValue; 
                }
            }
        }

        Debug.LogError("Function \"ButtonPushed\" is not returning a featureValue");
        throw new Exception("Function \"ButtonPushed\" is not returning a featureValue");
    }

    Vector3 getExtents(GameObject obj)
    {
        var coll = obj.GetComponent<MeshCollider>();
        return coll.bounds.extents;
    }
}