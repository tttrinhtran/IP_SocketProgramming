using UnityEditor.VersionControl;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StartSceneController : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public Button startButton;
    public ClientController clientController;

    private void Start()
    {
        // Ensure client controller is properly set up
        if (clientController == null)
        {
            Debug.LogError("ClientController reference not set in StartSceneController.");
            return;
        }

        // Add listener to the start button to send a message to the server
        startButton.onClick.AddListener(SendStartMessage);

    }

    private void SendStartMessage()
    {
        // Get the username from the input field
        string username = usernameInput.text;
        Debug.Log("Username: " + username);
        // Create a message indicating start action with the username
        StartMessage startMessage = new StartMessage(MessageType.Start, username);

        string jsonString=JsonUtility.ToJson(startMessage);
        // Send the message to the server
      
        clientController.SendMessageToServer(jsonString);
    }
    public void Update()
    {
        
    }

}
