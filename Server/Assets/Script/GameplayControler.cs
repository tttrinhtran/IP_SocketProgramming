using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControler : MonoBehaviour
{
    GameplayData gameplayData;
    ServerController serverControler;
    UIController uiControler;

    
    void Start()
    {
        serverControler.ClientMessageReceived += AddMessageToList;
        serverControler.ClientDisconnected += HandleClientDisconnect;
    }

    // Update is called once per frame
   public void innitGame() //getGamedata+ Client
   {
    serverControler.StopAcceptingClients();
     GameplayData.GameplayQuestion question = gameplayData.GetRandomWord();
        if (question != null)
        {
            Debug.Log("Random Question: " + question.Keyword);
          
        }
        else
        {
            Debug.LogError("No questions available.");
        }


   }
    public void StopGame()
    {
       serverControler.StopServer();
    }
   void lobby()
   {


   }

     void AddMessageToList(ClientHandler client, string message)
    {
        if (!clientMessages.ContainsKey(client))
            clientMessages.Add(client, new List<string>());

        clientMessages[client].Add(message);
        Debug.Log("Message received: " + message;
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
   
}
