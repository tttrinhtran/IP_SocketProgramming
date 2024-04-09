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
    public ServerController ServerController;
    public GameplayData GameplayData;
    public TMP_Text clientMessageText;

    private Dictionary<ClientHandler, List<string>> clientMessages = new Dictionary<ClientHandler, List<string>>(); // Store messages per client



    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        stopButton.onClick.AddListener(StopServer);
        ServerController.ClientMessageReceived += AddMessageToList;
        ServerController.ClientDisconnected += HandleClientDisconnect;
    }

    void StartGame()
    {
        ServerController.StopAcceptingClients();
        GameplayData.GameplayQuestion question = GameplayData.GetRandomWord();
        if (question != null)
        {
            Debug.Log("Random Question: " + question.Keyword);
          
        }
        else
        {
            Debug.LogError("No questions available.");
        }

    }

    void StopServer()
    {
       ServerController.StopServer();
    }

    void AddMessageToList(ClientHandler client, string message)
    {
        if (!clientMessages.ContainsKey(client))
            clientMessages.Add(client, new List<string>());

        clientMessages[client].Add(message);
        Debug.Log("Message received: " + message);
        UpdateUIWithClientMessages();
    }

    void HandleClientDisconnect(ClientHandler client)
    {
        if (clientMessages.ContainsKey(client))
        {
            clientMessages.Remove(client);
            UpdateUIWithClientMessages();
        }
    }
    void UpdateUIWithClientMessages()
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
