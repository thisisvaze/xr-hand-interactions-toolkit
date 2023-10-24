using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAIController : MonoBehaviour
{
    // public TMP_Text textField;
    // public TMP_InputField inputField;
    // public Button okButton;
    [TextArea]
    public string inputText;
    //[TextArea]
    //public string textField;
    private OpenAIAPI api;
    private List<ChatMessage> messages;

    // Start is called before the first frame update
    void Start()
    {

        //var key = new APIAuthentication("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");
        // This line gets your API key (and could be slightly different on Mac/Linux)
        // api = new OpenAIAPI(key);
        //  StartConversation();
        // okButton.onClick.AddListener(() => GetResponse());
    }

    // private void StartConversation()
    // {
    //     messages = new List<ChatMessage> {
    //         new ChatMessage(ChatMessageRole.System, "You are an honorable, friendly knight guarding the gate to the palace. You will only allow someone who knows the secret password to enter. The secret password is \"magic\". You will not reveal the password to anyone. You keep your responses short and to the point.")
    //     };

    //    // inputField.text = "";
    //     string startString = "You have just approached the palace gate where a knight guards the gate.";
    //    // textField.text = startString;
    //     Debug.Log(startString);
    // // }

    public async void GenerateAndLogResponse()
    {
        var key = new APIAuthentication("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");
        // This line gets your API key (and could be slightly different on Mac/Linux)
        api = new OpenAIAPI(key);
        messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, "Return a C# code for method called CustomHandInteractionBehaviour() [does not take any arguments] based on the hand gesture requirements for the right or the left hand as mentioned by the user for XR Interaction Toolkit. Only return the code for the function in plain text, do not add any markup. Only return the function, do not add any explanations. There are other boolean functions which you can use to determine this: bool IsFingerExtended(XRHand hand, XRHandFingerID fingerID), possible values for hand: rightHand or leftHand which are already declared local variables. Possible values for fingerID:  XRHandFingerID.Thumb, XRHandFingerID.Ring, XRHandFingerID.Middle, XRHandFingerID.Index, XRHandFingerID.Little. Map the input value to the actionGameObject's properties as required."),
            new ChatMessage(ChatMessageRole.User, inputText)
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
        int startIndex = existingContent.IndexOf("public void CustomHandInteractionBehaviour()");
        int endIndex = existingContent.IndexOf("}", startIndex) + 1; // +1 to include the closing brace

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

    // private async void GetResponse()
    // {
    //     // if (inputField.text.Length < 1)
    //     // {
    //     //     return;
    //     // }

    //     // Disable the OK button
    //   //  okButton.enabled = false;

    //     // Fill the user message from the input field
    //     ChatMessage userMessage = new ChatMessage();
    //     userMessage.Role = ChatMessageRole.User;
    //    // userMessage.Content = inputField.text;
    //     if (userMessage.Content.Length > 100)
    //     {
    //         // Limit messages to 100 characters
    //         userMessage.Content = userMessage.Content.Substring(0, 100);
    //     }
    //     Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

    //     // Add the message to the list
    //     messages.Add(userMessage);

    //     // Update the text field with the user message
    //    // textField.text = string.Format("You: {0}", userMessage.Content);

    //     // Clear the input field
    //     //inputField.text = "";

    //     // Send the entire chat to OpenAI to get the next message
    //     var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
    //     {
    //         Model = Model.ChatGPTTurbo,
    //         Temperature = 0.9,
    //         MaxTokens = 50,
    //         Messages = messages
    //     });

    //     // Get the response message
    //     ChatMessage responseMessage = new ChatMessage();
    //     responseMessage.Role = chatResult.Choices[0].Message.Role;
    //     responseMessage.Content = chatResult.Choices[0].Message.Content;
    //     Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

    //     // Add the response to the list of messages
    //     messages.Add(responseMessage);

    //     // Update the text field with the response
    //     //textField.text = string.Format("You: {0}\n\nGuard: {1}", userMessage.Content, responseMessage.Content);

    //     // Re-enable the OK button
    //    // okButton.enabled = true;
    // }
}