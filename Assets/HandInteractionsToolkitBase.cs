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
    private UnityEvent _whenHandsJoint;

    [SerializeField]
    private UnityEvent _whenRightThumbsUp;
    [SerializeField]
    private UnityEvent _whenLeftThumbsUp;
    [SerializeField]
    private UnityEvent _whenRightThumbsDown;
    [SerializeField]
    private UnityEvent _whenLeftThumbsDown;

    private XRHand rightHand;

    private XRHand leftHand;

    private XRHandSubsystem handSubsystem;

    private List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
 private float previousOrientation;

    //public UnityEvent<bool> WhenHandsJoint => _whenHandsJoint;
    public UnityEvent WhenHandsJoint => _whenHandsJoint;
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

    Debug.Log("Distance:"+DistanceBetweenHands());  
    if (DistanceBetweenHands() < 0.08f)
    {
        _whenHandsJoint.Invoke(); // Invoke the event only when hands are joint
    }
}
private void CheckIfRightThumbsUp()
{
    if (IsOnlyThumbOpen(rightHand) && IsThumbFacingUpwards(rightHand))
    {
        _whenRightThumbsUp.Invoke(); // Invoke the event only when right hand is in thumbs-up pose and thumb is facing upwards
    }
}

bool IsOnlyThumbOpen(XRHand hand)
{
    // Check if thumb is extended
    if (!IsFingerExtended(hand,XRHandFingerID.Thumb))
        {Debug.Log("Thumb closed");
        return false;}

    // Check if other fingers are not extended
    if (IsFingerExtended(hand,XRHandFingerID.Index) || IsFingerExtended(hand,XRHandFingerID.Ring) || IsFingerExtended(hand,XRHandFingerID.Middle)  ||IsFingerExtended(hand,XRHandFingerID.Little) )
       { Debug.Log("Fingers open");
        return false;}

    return true;
}


bool IsFingerExtended(XRHand hand, XRHandFingerID fingerID) {
        Vector3 fingerDirection = new Vector3(0,0,0);
        hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var basePose);
        switch (fingerID)
        {
            case XRHandFingerID.Index:
                {
                    if (!(hand.GetJoint(XRHandJointID.IndexMetacarpal).TryGetPose(out  basePose) &&
                        hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var tipPose)))
                    {
                        return false;
                    }
                    fingerDirection = (tipPose.position - basePose.position).normalized;
                }
            break;

            case XRHandFingerID.Thumb:
                {
                    if (!(hand.GetJoint(XRHandJointID.ThumbMetacarpal).TryGetPose(out  basePose) &&
                        hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var tipPose)))
                    {
                        return false;
                    }
                    fingerDirection = (tipPose.position - basePose.position).normalized;

                }
                break;

             case XRHandFingerID.Little:
                {
                    if (!(hand.GetJoint(XRHandJointID.LittleMetacarpal).TryGetPose(out  basePose) &&
                        hand.GetJoint(XRHandJointID.LittleTip).TryGetPose(out var tipPose)))
                    {
                        return false;
                    }
                    fingerDirection = (tipPose.position - basePose.position).normalized;

                }
                break;
             case XRHandFingerID.Ring:
                {
                    if (!(hand.GetJoint(XRHandJointID.RingMetacarpal).TryGetPose(out  basePose) &&
                        hand.GetJoint(XRHandJointID.RingTip).TryGetPose(out var tipPose)))
                    {
                        return false;
                    }
                    fingerDirection = (tipPose.position - basePose.position).normalized;

                }
                break;
             case XRHandFingerID.Middle:
                {
                    if (!(hand.GetJoint(XRHandJointID.MiddleMetacarpal).TryGetPose(out  basePose) &&
                        hand.GetJoint(XRHandJointID.MiddleTip).TryGetPose(out var tipPose)))
                    {
                        return false;
                    }
                    fingerDirection = (tipPose.position - basePose.position).normalized;

                }
                break;
        }

       // ... existing code to calculate fingerDirection ...

        // Get the direction from the wrist to the base of the finger
        if (!hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose))
        {
            return false;
        }
        Vector3 baseDirection = (basePose.position - wristPose.position).normalized;

        // Calculate the angle between the baseDirection and fingerDirection
        float angle = Vector3.Angle(baseDirection, fingerDirection);

        // If the angle is small, the finger is extended
        return angle < 30;  // Adjust the threshold based on your requirements

    }

bool IsThumbFacingUpwards(XRHand hand)
{
    if (!(hand.GetJoint(XRHandJointID.ThumbMetacarpal).TryGetPose(out var basePose) &&
          hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var tipPose)))
    {
        return false;
    }

    Vector3 thumbDirection = (tipPose.position - basePose.position).normalized;
    float dotProduct = Vector3.Dot(thumbDirection, Vector3.up);

    // Adjust the threshold based on your requirements
    float threshold = 0.7f;
    return dotProduct > threshold;
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
         rightHand = handSubsystem.rightHand;
         leftHand = handSubsystem.leftHand;
         if(rightHand.isTracked && leftHand.isTracked){
            CheckIfHandsJoint();
            CheckIfRightThumbsUp();
         } 

          /*  // Get the orientation of the right hand with respect to the z-axis
            float currentOrientation = GetRightHandZAxisOrientation(rightHand);

            // If the orientation has changed since the last frame, trigger the event
            if (currentOrientation != previousOrientation)
            {
            HandGestureEvents.RightHandZAxisOrientationChanged.Invoke(currentOrientation);
                previousOrientation = currentOrientation;
            }*/

    
    }
/*
      private float GetRightHandZAxisOrientation(XRHand rightHand)
    {
        // This is a placeholder implementation. You would need to replace this with your own implementation that calculates the orientation of the right hand with respect to the z-axis.
        return rightHand.rootPose.rotation.eulerAngles.z;
    }*/


}
