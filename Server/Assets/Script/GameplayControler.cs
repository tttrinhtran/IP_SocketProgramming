using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.MPE;

public class GameplayControler : MonoBehaviour
{
    GameplayData gameplayData;
    ServerController serverControler;
    UIController uiControler;
    Dictionary<ClientHandler, string> clientMessages = new Dictionary<ClientHandler, string>();
    GameplayQuestion gameplayQuestion;
   int turn = 0; 
   ClientHandler curUser;
    List<ClientHandler> sortedUsers ;
        
    
        void Start()
        {
            
            // Initialize serverControler, gameplayData, uiControler
            serverControler = FindObjectOfType<ServerController>();
            gameplayData = FindObjectOfType<GameplayData>();
            uiControler = FindObjectOfType<UIController>();
            innitGame();

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
      gameplayQuestion= gameplayData.GetRandomWord();
        if (gameplayQuestion != null)
        {
            Debug.Log("Random Question: " + gameplayQuestion.Keyword);
          
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
            clientMessages.Add(client, message);

        clientMessages[client] = message;
        Debug.Log("Message received: " + message);
        MessageClient messageClient=client.ConvertMessageToJSON(message);
       
        if(messageClient.Type == Type.Hello)
        {
            setTheUser(client, messageClient); 
        }
        
        if (messageClient.Type == Type.Start)
        {
            Debug.Log("Yo");
            // setTheUser(client, messageClient);
            if(client.clientModel == null){
                Debug.Log("Client null"); 
            }
            UpdateLobby();
            Debug.Log("Update Lobby");
        }

        // if(messageClient.Type == Type.Lobby)
        // {
            
        // }

        if(messageClient.Type == Type.Play)
        {
            gameLogic(messageClient, client);
        }
      
      if(messageClient.Type==Type.Wait)
      {
        
        
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
            sendLobbyUsers();
           //uiControler.UpdateLobby();
        }
    }

   public void UpdateLobby()
    {
        // Generate the lobby users message
        string lobbyUsersMessage = sendLobbyUsers();
        
        // Broadcast the lobby users message to all clients
        serverControler.BroadcastToAllClients(lobbyUsersMessage);
        Debug.Log("Send multiple messages successfully"); 
    }

    public string sendLobbyUsers()
    {
        MessageClient messageClient = new MessageClient(); 
        messageClient.Type = Type.Lobby;
        messageClient.Text = string.Join("\n", serverControler.GetConnectedClients());

        return JsonUtility.ToJson(messageClient);
    }

    public void startGame()
    {
        var sortedClients = clientMessages.Keys.OrderBy(client => client.clientModel.UserId).ToList();

        // Retrieve the sorted list of users based on their UserIDs
        List<ClientHandler> sortedUsers = sortedClients.Select(client => client).ToList();
        
        
        
        uiControler.startButton.onClick.AddListener(innitGame);
    }

    Message handlePlayMessage(int point, Type type, Data data)
    {
        Message res = new Message();
        res.point = point; 
        res.type = type;
        res.data = data; 
        return res; 
    }
    
    int checkAnswer(string input, string answer)
    { 
        if(input.Length >1) 
        {
            if(turn < 2) return 0; 
            else{
                if(input == answer) return 5; 
                else return 0; 
            }
        }
        else
        {
            if(answer.Contains(input) && input.Length != answer.Length) return 1; 
            else return 0; 
        }
    }

    void gameLogic(MessageClient messageClient, ClientHandler client)
    {
        
        int point=checkAnswer(messageClient.Text, gameplayQuestion.Keyword);
        client.clientModel.point += point;
        if(point == 5)
        {
           endGame();
        }
        else if(point==1)
        {

            Message curRes = handlePlayMessage(point, Type.Play, new Data());
            serverControler.SendMessageToClient(client, curRes);
            
        }
        else
        {
            Message curRes = handlePlayMessage(point, Type.Wait, new Data());
            serverControler.SendMessageToClient(client, curRes);
        }



    }
    void endGame()
    {
        MessageClient messageClient = new MessageClient(); 
        messageClient.Type = Type.End;
        messageClient.Text = "Game Over";
        serverControler.BroadcastToAllClients(JsonUtility.ToJson(messageClient));
    }
}
