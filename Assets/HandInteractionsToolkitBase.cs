using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;

public class HandInteractionsToolkitBase : MonoBehaviour
{


    [SerializeField]
    private UnityEvent<bool> _whenHandsJoint;

    private XRHand rightHand;

    private XRHand leftHand;

    private XRHandSubsystem handSubsystem;

    private List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
 private float previousOrientation;

    //public UnityEvent<bool> WhenHandsJoint => _whenHandsJoint;
    public UnityEvent<bool> WhenHandsJoint => _whenHandsJoint;
    private void Start()
    {
        SubsystemManager.GetInstances(handSubsystems);
        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];


            // Now you can use handSubsystem
        }
    }
    private void CheckIfHandsJoint()

    {
        bool areHandsJoint = false;
        if (DistanceBetweenHands() < 0.1f)
        {
            areHandsJoint = true;
        }

        _whenHandsJoint.Invoke(areHandsJoint);
    }

    float DistanceBetweenHands()
    {
        return Vector3.Distance(rightHand.rootPose.position, leftHand.rootPose.position);

    }

    Vector3 VectorBetweenHands()
    {
        return rightHand.rootPose.position - leftHand.rootPose.position;
    }

    // private void GetUpdatedHandsData(){
    //         rightHand = xRHandSubsystem.rightHand;
    //         leftHand = xRHandSubsystem.leftHand;
    // }
    private void Update()
    {
      //  GetUpdatedHandsData();

         rightHand = handSubsystem.rightHand;
         leftHand = handSubsystem.leftHand;

            // Get the orientation of the right hand with respect to the z-axis
            float currentOrientation = GetRightHandZAxisOrientation(rightHand);

            // If the orientation has changed since the last frame, trigger the event
            if (currentOrientation != previousOrientation)
            {
            HandGestureEvents.RightHandZAxisOrientationChanged.Invoke(currentOrientation);
                previousOrientation = currentOrientation;
            }

        CheckIfHandsJoint();
    }

      private float GetRightHandZAxisOrientation(XRHand rightHand)
    {
        // This is a placeholder implementation. You would need to replace this with your own implementation that calculates the orientation of the right hand with respect to the z-axis.
        return rightHand.rootPose.rotation.eulerAngles.z;
    }


}
