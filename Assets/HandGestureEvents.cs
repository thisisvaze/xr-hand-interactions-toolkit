using UnityEngine;
using UnityEngine.Events;

public class HandGestureEvents : MonoBehaviour
{
    // Define a UnityEvent that takes a single float parameter
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public FloatEvent RightHandZAxisOrientationChanged;
}