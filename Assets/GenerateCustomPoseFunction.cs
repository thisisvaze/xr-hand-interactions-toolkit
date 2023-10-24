using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.IO;
public class GenerateCustomPoseFunction : MonoBehaviour
{
      [TextArea(5,10)][Tooltip("Custom Hand Interaction Behaviour description")]
    public string behaviorDescription;

    
    //[TextArea]
    //public string textField;
    private OpenAIAPI api;
    private List<ChatMessage> messages;
    [SerializeField]
    private OVRHand rightHand;
    [SerializeField] 
    private OVRHand leftHand;
    [SerializeField]
    private OVRSkeleton rightSkeleton;
    [SerializeField]
    private OVRSkeleton leftSkeleton;

    [SerializeField]
    private GameObject actionGameObject;
    //[Header("Pose Event")]
    /*[SerializeField]
    private UnityEvent _whenCustomHandPoseActivated;
    [SerializeField]
    private UnityEvent _whenCustomHandPoseDeactivated;
*/
/*
    public UnityEvent WhenCustomHandPoseActivated => _whenCustomHandPoseActivated;
    public UnityEvent WhenCustomHandPoseDeactivated => _whenCustomHandPoseDeactivated;
*/

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
        if (rightHand.IsTracked && leftHand.IsTracked)
        {
           // CheckIfCustomPoseActivated();
            CustomHandInteractionBehaviour();
        }
    }

/*    private void CheckIfCustomPoseActivated()
    {
        if(WhenCustomPoseActivated()){
        Debug.Log("Custom hand pose detected");
         _whenCustomHandPoseActivated.Invoke(); 
        }
        else{
             Debug.Log("Custom hand pose not detected");
            _whenCustomHandPoseDeactivated.Invoke();
        }
        

    
    }*/
    
    public async void GenerateAndLogResponse()
    {
        var key = new APIAuthentication("");
        // This line gets your API key (and could be slightly different on Mac/Linux)
        api = new OpenAIAPI(key);
        messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, "Return a C# code for method called void CustomHandInteractionBehaviour() [does not take any arguments] based on the hand gesture requirements for the right or the left hand as mentioned by the user for Oculus Interaction SDK. Only return the code for the function in plain text, do not add any markup. Only return the function, do not add any explanations. There are other boolean functions which you can use to determine this: bool IsFingerExtended(OVRSkeleton skeleton, OVRHand.HandFinger fingerID), possible values for hand: rightSkeleton or leftSkeleton which are already declared local variables. Possible values for fingerID:  OVRHand.HandFinger.Thumb, OVRHand.HandFinger.Ring, OVRHand.HandFinger.Middle, OVRHand.HandFinger.Index, OVRHand.HandFinger.Pinky. Map the input value to the actionGameObject's properties as required. You can access the position of hands using rightHand.transform.position and rotation using rightHand.transform.rotation"),
            new ChatMessage(ChatMessageRole.User, behaviorDescription)
        };

        //okButton.onClick.AddListener(() => GetResponse());
        // Send the entire chat to OpenAI to get the next message
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.GPT4,
            Temperature = 0.7,
            MaxTokens = 400,
            Messages = messages
        });

        // Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        // Add the response to the list of messages
        //messages.Add(responseMessage);

        // Update the text field with the response
        //textField = responseMessage.Content;

        string filePath = "Assets/GenerateCustomPoseFunction.cs";
        string existingContent = File.ReadAllText(filePath);

        // Find the start and end indices of the existing method
        int startIndex = existingContent.LastIndexOf("void CustomHandInteractionBehaviour()");
        int endIndex = existingContent.LastIndexOf("}");
            if (endIndex >= 0)
            {
                int secondLastIndex = existingContent.LastIndexOf("}", endIndex - 1);
                if (secondLastIndex >= 0)
                {   
                    endIndex = secondLastIndex;
                    // secondLastIndex is the index of the second last occurrence of '}'
                }
            }// +1 to include the closing brace

        if (startIndex < 0 || endIndex < 0)
        {
            Debug.Log("Could not find the CustomHandInteractionBehaviour method.");
            return;
        }

        // Extract the C# code from the markdown
        string code = responseMessage.Content;
        if (code.StartsWith("```csharp"))
        {
            code = code.Substring("```csharp".Length);
        }
        if (code.EndsWith("```"))
        {
            code = code.Substring(0, code.Length - "```".Length);
        }

        // Replace the existing method with the new content
        string newContent = existingContent.Remove(startIndex, endIndex - startIndex);
        newContent = newContent.Insert(startIndex, "\n" + code + "\n");

        File.WriteAllText(filePath, newContent);
    }


    float DistanceBetweenHands()
    {
        return Vector3.Distance(rightHand.transform.position, leftHand.transform.position);
        
    }

    Vector3 VectorBetweenHands()
    {
        return rightHand.transform.position - leftHand.transform.position;
    }
bool IsFingerExtended(OVRSkeleton skeleton, OVRHand.HandFinger fingerID)
{
    Vector3 fingerDirection;
    OVRSkeleton.BoneId metacarpalID, tipID;

    switch (fingerID)
    {
        case OVRHand.HandFinger.Index:
            metacarpalID = OVRSkeleton.BoneId.Hand_Index1;
            tipID = OVRSkeleton.BoneId.Hand_IndexTip;
            break;
        case OVRHand.HandFinger.Thumb:
            metacarpalID = OVRSkeleton.BoneId.Hand_Thumb1;
            tipID = OVRSkeleton.BoneId.Hand_ThumbTip;
            break;
        case OVRHand.HandFinger.Pinky:
            metacarpalID = OVRSkeleton.BoneId.Hand_Pinky1;
            tipID = OVRSkeleton.BoneId.Hand_PinkyTip;
            break;
        case OVRHand.HandFinger.Ring:
            metacarpalID = OVRSkeleton.BoneId.Hand_Ring1;
            tipID = OVRSkeleton.BoneId.Hand_RingTip;
            break;
        case OVRHand.HandFinger.Middle:
            metacarpalID = OVRSkeleton.BoneId.Hand_Middle1;
            tipID = OVRSkeleton.BoneId.Hand_MiddleTip;
            break;
        default:
            Debug.LogError("Invalid finger ID");
            return false;
    }

        var basePose= skeleton.Bones[(int)metacarpalID].Transform;
        var tipPose = skeleton.Bones[(int)tipID].Transform;

    fingerDirection = (tipPose.position - basePose.position).normalized;

        var wristPose = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform;

    Vector3 baseDirection = (basePose.position - wristPose.position).normalized;
    float angle = Vector3.Angle(baseDirection, fingerDirection);
    //Debug.Log(angle);
    return angle < 30;  // Adjust the threshold based on your requirements
}

public bool WhenCustomPoseActivated()
{
    return false;
}








 
 
void CustomHandInteractionBehaviour()
{

    bool isThumbExtended = IsFingerExtended(rightSkeleton, OVRHand.HandFinger.Thumb);
    bool isIndexExtended = IsFingerExtended(rightSkeleton, OVRHand.HandFinger.Index);
    bool isMiddleExtended = IsFingerExtended(rightSkeleton, OVRHand.HandFinger.Middle);
    bool isRingExtended = IsFingerExtended(rightSkeleton, OVRHand.HandFinger.Ring);
    bool isPinkyExtended = IsFingerExtended(rightSkeleton, OVRHand.HandFinger.Pinky);

    if (isThumbExtended && isIndexExtended && isMiddleExtended && isRingExtended && isPinkyExtended)
    {
        // If all fingers are extended, the hand is open. Move the cube away from the hand.
        actionGameObject.transform.position = Vector3.MoveTowards(actionGameObject.transform.position, rightHand.transform.position, -1f);
    }
    else if (!isThumbExtended && !isIndexExtended && !isMiddleExtended && !isRingExtended && !isPinkyExtended)
    {
        // If no fingers are extended, the hand is a fist. Move the cube close to the hand.
        actionGameObject.transform.position = Vector3.MoveTowards(actionGameObject.transform.position, rightHand.transform.position, 1f);
    }
}



}
