using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;

public class HandInteractionsToolkitBase: MonoBehaviour
{


    [SerializeField]
    private UnityEvent<bool> _whenHandsJoint;

    [SerializeField]
    private XRHand rightHand;
    [SerializeField]
    private XRHand leftHand;

    [SerializeField]
    XRHandSubsystem xRHandSubsystem;


    public UnityEvent<bool> WhenHandsJoint => _whenHandsJoint;
    private void CheckIfHandsJoint()

    {
        bool areHandsJoint = false;
        if(DistanceBetweenHands() < 0.1f)
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

    private void Update()
    {
        CheckIfHandsJoint();
    }


}
