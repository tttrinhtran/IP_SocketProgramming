using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playSceneController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Description;
    public TMPro.TextMeshProUGUI Score;
    public TMPro.TextMeshProUGUI TimeText; // Changed variable name to avoid conflict with Time class
    public TMPro.TextMeshProUGUI Keyword;
    public TMPro.TextMeshProUGUI Notification;
    public TMPro.TMP_InputField InputField;
    public Button SubmitButton;
    public ClientController clientController;
    private float time = 10f; // Changed to float for smooth countdown
    public MessageType type;

    // Start is called before the first frame update
    void Start()
    {
        if (clientController == null)
        {
            Debug.LogError("ClientController reference not set in StartSceneController.");
            return;
        }

        // Add listener to the start button to send a message to the server
        SubmitButton.onClick.AddListener(SendMessage);
        // set userID from userID.txt
    }

    void Update()
    {
        // Update time countdown
        if (time > 0)
        {
            time -= Time.deltaTime; // Decrement time by deltaTime each frame
            UpdateTimeText();
        }
        else
        {
            // Time's up, handle accordingly
            // For example:
            UpdateTimeText();
            TimeIsUp();
        }
    }

    private void UpdateTimeText()
    {
        // Update the time UI text
        TimeText.text = "Time: " + Mathf.RoundToInt(time).ToString(); // Convert time to integer for display
    }

    private void TimeIsUp()
    {
        // Handle time-up scenario here
        SendMessage(); // Send message to server
        Debug.Log("Time's up!");
    }

    private void SendMessage()
    {
        string answer = InputField.text;
        Debug.Log("Answer: " + answer);
        StartMessage answerMessage = new StartMessage(type, answer);
        string jsonString = JsonUtility.ToJson(answerMessage);
        clientController.SendMessageToServer(jsonString); 
        SubmitButton.interactable=false;

    }

    public void updateUI(PlayMessage playMessage)
    {
        Debug.Log("Updating UI with PlayMessage");
        Description.text=playMessage.data.hint;
        string text = playMessage.data.currentAnswer;
        char[] characters = text.ToCharArray();
        // Join the characters with spaces in between
        string spacedText = string.Join(" ", characters);
        Keyword.text=spacedText;
        Score.text="Score: "+playMessage.point.ToString();
        type=playMessage.Type;
        Debug.Log("Type: "+type + "MessageType: " + MessageType.Play);
        
        if ((MessageType)playMessage.Type==MessageType.Play)
        {
            Notification.text="Your Turn";
            SubmitButton.interactable=true;
            Debug.Log("play bitch");            time = 10;
        }
        else
        {
            Notification.text="Opponent's Turn";
            SubmitButton.interactable = false; time = 0;
            
        }
    }
}
