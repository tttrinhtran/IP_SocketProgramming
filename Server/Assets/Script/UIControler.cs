using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class UIController : MonoBehaviour
{
    public Button startButton;
    public Button stopButton;
    public GameplayControler gameplayControler;
 
    public TMP_Text clientMessageText;

    private Dictionary<ClientHandler, List<string>> clientMessages = new Dictionary<ClientHandler, List<string>>(); // Store messages per client



    void Start()
    {
        startButton.onClick.AddListener(gameplayControler.innitGame);
        stopButton.onClick.AddListener(gameplayControler.StopGame);
       
    }

  
    void UpdateLobby()
    {
        // Check if clientMessageText is not null before accessing it
        if (clientMessageText != null)
        {
            // Combine messages for all clients into one string
            List<string> allMessages = new List<string>();
            foreach (var messages in clientMessages.Values)
            {
                allMessages.AddRange(messages);
            }

            string combinedMessages = string.Join("\n", allMessages);
            Debug.Log(combinedMessages);
            UnityMainThreadDispatcher.Dispatcher.Enqueue(()=>
            {
                clientMessageText.text = combinedMessages;
            });
           
        }
        else
        {
            Debug.LogError("clientMessageText is not assigned! Attempting to find and assign...");
            // Attempt to find clientMessageText in the scene and assign it
            clientMessageText = GetComponent<TMP_Text>();
            if (clientMessageText != null)
            {
                Debug.Log("clientMessageText found and assigned successfully!");
                // After successfully assigning, update the UI
                UpdateUIWithClientMessages();
            }
            else
            {
                Debug.LogError("clientMessageText not found in the scene. Manual assignment required.");
            }
        }
    }

}
