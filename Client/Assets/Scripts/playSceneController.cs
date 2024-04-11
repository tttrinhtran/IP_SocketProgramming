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
    private int userID;

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
        userID = int.Parse(System.IO.File.ReadAllText("userID.txt"));
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
        StartMessage answerMessage = new StartMessage(MessageType.Play, answer);
        string jsonString = JsonUtility.ToJson(answerMessage);
        clientController.SendMessageToServer(jsonString);
    }

    public void updateUI(PlayMessage playMessage)
    {
        Debug.Log("Updating UI with PlayMessage");
        Description.text=playMessage.data.hint;
        Keyword.text=playMessage.data.currentAnswer;
        Score.text="Score: "+playMessage.point.ToString();
        if (playMessage.Type==MessageType.Play)
        {
            Notification.text="Your Turn";
            SubmitButton.interactable=true;
        }
        else
        {
            Notification.text="Opponent's Turn";
            SubmitButton.interactable=false;
        }
    }
}
