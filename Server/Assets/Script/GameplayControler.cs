using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControler : MonoBehaviour
{
    GameplayData gameplayData;
    ServerController serverControler;
    UIController uiControler;
     Dictionary<ClientHandler, List<string>> clientMessages = new Dictionary<ClientHandler, List<string>>();
    
    void Start()
    {
        // Initialize serverControler, gameplayData, uiControler
        serverControler = FindObjectOfType<ServerController>();
        gameplayData = FindObjectOfType<GameplayData>();
        uiControler = FindObjectOfType<UIController>();

        if (serverControler != null)
        {
            serverControler.ClientMessageReceived += (client, message) => MessageHandle(client, message);
            serverControler.ClientDisconnected += HandleClientDisconnect;
        }
        else
        {
            Debug.LogError("ServerController not found.");
        }

        if (gameplayData == null)
        {
            Debug.LogError("GameplayData not found.");
        }

        if (uiControler == null)
        {
            Debug.LogError("UIController not found.");
        }
  
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
        serverControler.ClientMessageReceived += (client, message) => MessageHandle(client, message);
        serverControler.ClientDisconnected += HandleClientDisconnect;


   }

    void MessageHandle(ClientHandler client, string message)
    {
        if (!clientMessages.ContainsKey(client))
            clientMessages.Add(client, new List<string>());

        clientMessages[client].Add(message);
        Debug.Log("Message received: " + message);
        MessageClient messageClient=client.ConvertMessageToJSON(message);
       
        if (messageClient.Type == Type.Start)
        {
            setTheUser(client, messageClient);

        }
           
      

       
    }
    void setTheUser(ClientHandler client, MessageClient message)
    {
        client.setClientModel(message.Text);
        Debug.Log("Client set: " + message.Text);
    }
   

    void HandleClientDisconnect(ClientHandler client)
    {
        if (clientMessages.ContainsKey(client))
        {
            clientMessages.Remove(client);
           //uiControler.UpdateLobby();
        }
    }

    public void UpdateLobby()
    {
        // Check if clientMessageText is not null before accessing it
       
    }
   
   
}
