using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameplayControler : MonoBehaviour
{
    GameplayData gameplayData;
    ServerController serverControler;
    UIController uiControler;
    Dictionary<ClientHandler, string> clientMessages = new Dictionary<ClientHandler, string>();
    GameplayQuestion gameplayQuestion;
   int turn = 0; 
   ClientHandler curUser;
   List<string> connectedClients = new List<string>();
    List<ClientHandler> sortedUsers = new List<ClientHandler>();
    string curString = "";
        
    
        void Start()
        {
            
            // Initialize serverControler, gameplayData, uiControler
            serverControler = FindObjectOfType<ServerController>();
            gameplayData = FindObjectOfType<GameplayData>();
            uiControler = FindObjectOfType<UIController>();
            // innitGame();    
            startGame();
            if (serverControler != null)
            {

                serverControler.ClientMessageReceived += (client, message) => MessageHandle(client, message);
                serverControler.ClientDisconnected += (client) => HandleClientDisconnect(client);
                
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
    var sortedClients = clientMessages.Keys.OrderBy(client => client.clientModel.UserId).ToList();

        // Retrieve the sorted list of users based on their UserIDs
        sortedUsers = sortedClients.Select(client => client).ToList();


        serverControler.StopAcceptingClients();
        gameplayData.LoadGameplayDataFromFile();
        gameplayQuestion= gameplayData.GetRandomWord();
        if (gameplayQuestion != null)
        {
            Debug.Log("Random Question: " + gameplayQuestion.Keyword);
          
        }
        else
        {
            Debug.LogError("No questions available.");
        }
        Debug.Log("Sort user size: " + sortedUsers.Count); 
        curUser = sortedUsers[0];
        foreach(ClientHandler client in sortedUsers)
        {
            Data data = new Data();
            data.hint = gameplayQuestion.Description;
            int tmp = gameplayQuestion.Keyword.Length; 
            for(int i = 0; i < tmp; i++)
            {
                data.currentAnswer += "_";
            }
            curString = data.currentAnswer.ToUpper(); 
            if(client == curUser)
            {
                handlePlayMessage(0, Type.Play, data, client);
            }
            else
            {
                handlePlayMessage(0, Type.Wait, data, client);
            }
        }
   }

    public void StopGame()
    {
       serverControler.StopServer();
       Application.Quit();
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
            
        //    GetConnectedClientsToLobby(client);
        //    serverControler.BroadcastToAllClients(sendLobbyUsers());
        }
        
        if (messageClient.Type == Type.Start)
        {
            Debug.Log("Yo");
            setTheUser(client, messageClient); 
            GetConnectedClientsToLobby(client);
            // setTheUser(client, messageClient);
            if(client.clientModel == null){
                Debug.Log("Client null"); 
            }
            UpdateLobby();
            Debug.Log("Update Lobby");
        }

        if(messageClient.Type == Type.Play)
        {
            gameLogic(messageClient, client);
        }
       

    }

    public List<string> GetConnectedClientsToLobby(ClientHandler client)
    {
        
    
        connectedClients.Add(client.GetClientInfo());
        Debug.Log("Info"+client.GetClientInfo());
        
        return connectedClients;
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
            Debug.Log("Client disconnected: " + client.GetClientInfo());
            connectedClients.Remove(client.GetClientInfo());
             clientMessages.Remove(client);
            serverControler.RemoveClient(client);
            // client.CloseConnection();
            
           
            
            UpdateLobby();
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
        Debug.Log("Shiba: "+ connectedClients[0]);
        messageClient.Text = string.Join("\n", connectedClients);

        return JsonUtility.ToJson(messageClient);
    }

    public void startGame()
    {
        uiControler.startButton.onClick.AddListener(innitGame);
    }

    Message createPlayMessage(int point, Type type, Data data)
    {
        Message res = new Message();
        res.point = point; 
        res.Type = type;
        res.data = data; 
        return res; 
    }
    
    Tuple<int, string> checkAnswer(string input, string answer)
{
    int point = 0;
    input = input.ToUpper();
    // string maskedString = "";

    if (input.Length > 1)
    {
        if (turn < 2) 
        {
            // If it's not the player's turn, return 0 points and the original answer
            return Tuple.Create(point, curString);
        }
        else
        {
            if (input == answer) 
            {
                // If the input matches the answer, return 5 points and the original answer
                point = 5;
                curString = answer;
            }
            else 
            {
                // If the input does not match the answer, return 0 points and the original answer
                return Tuple.Create(point, curString);
            }
        }
    }
    else
    {
        if (answer.Contains(input) && input.Length != answer.Length)
        {
            // If the input is contained in the answer but is not equal to the answer, return 1 point and the masked answer
            point = 1;
            curString = MaskAnswer(answer.ToUpper(), input.ToUpper());
        }
        else
        {
            // If the input is not contained in the answer or is equal to the answer, return 0 points and the original answer
            point = 0; 
            return Tuple.Create(point, curString);
        }
    }

    // Return the point value and the masked string
    return Tuple.Create(point, curString);
}

string MaskAnswer(string answer, string input)
{
    // Convert the answer and current string to char arrays for easier manipulation
    char[] answerChars = answer.ToUpper().ToCharArray();
    char[] curStringChars = curString.ToUpper().ToCharArray();

    // Iterate through each character in the answer
    for (int i = 0; i < answerChars.Length; i++)
    {
        // If the character matches the input or is already revealed in curString, keep it
        if (answerChars[i] == input[0] || curStringChars[i] != '_')
        {
            curStringChars[i] = answerChars[i];
        }
    }

    // Convert the char array back to a string and return
    return new string(curStringChars);
}



    void gameLogic(MessageClient messageClient, ClientHandler client)
{
    Tuple<int, string> pointTuple = checkAnswer(messageClient.Text, gameplayQuestion.Keyword);
    int point = pointTuple.Item1; // Extract the point value from the tuple
    string resString = pointTuple.Item2; // Extract the masked answer from the tuple

    // Update the client's points
    client.clientModel.point += point;
    curUser = client;
    Data newData = new Data(); 
    newData.hint = gameplayQuestion.Description;
    newData.currentAnswer = resString; 
     turn++; 
    // Check if the game should end
    if (point == 5 || turn == 5)
    {
        endGame();
        return; 
    }
    else if (point == 1)
    {
        // Handle the message for the current client
        handlePlayMessage(client.clientModel.point, Type.Play, newData, curUser);
    }
    else if(point == 0)
    {
        // Handle the message for the current client
        Debug.Log("Point: " + point);
       // handlePlayMessage(client.clientModel.point, Type.Wait, newData, client);

        // Find the index of the current client in the sorted users list
        int index = sortedUsers.FindIndex(c => c == curUser);

        // Determine the next user
      
        if (index == sortedUsers.Count - 1)
        {
            curUser = sortedUsers[0];
        }
        else
        {
            curUser = sortedUsers[index + 1];
        }

        // Handle the message for the next client
        handlePlayMessage(client.clientModel.point, Type.Play, newData, curUser);
    }

    // Increment the turn counter
   

    foreach(ClientHandler clientHandler in sortedUsers)
    {
        if(clientHandler != curUser)
        {
            handlePlayMessage(clientHandler.clientModel.point, Type.Wait, newData, clientHandler);
        }
    }
}

    // Updated handlePlayMessage function to include the client parameter
    void handlePlayMessage(int point, Type type, Data data, ClientHandler client)
    {
        // Create a message with the specified parameters
        Message res = new Message();
        res.point = point;
        res.Type = type;
        res.data = data;

        // Send the message to the client
        serverControler.SendMessageToClient(client, res);
    }

    void endGame()
    {
        sortedUsers = sortedUsers.OrderByDescending(client => client.clientModel.point).ToList(); 
        MessageClient messageClient = new MessageClient(); 
        messageClient.Type = Type.End;
        foreach(ClientHandler client in sortedUsers)
        {
            messageClient.Text += client.clientModel.UserId + ": " + client.clientModel.point + "\n";
        }
        serverControler.BroadcastToAllClients(JsonUtility.ToJson(messageClient));

        uiControler.lobbyButton.onClick.AddListener(UpdateLobby);
    }

    // public void backToLobby()
    // {
    //    MessageClient messageClient = new MessageClient();
    //    messageClient.Type = Type.Lobby;
    //    messageClient.Text = "Back to lobby";
    //    serverControler.BroadcastToAllClients(JsonUtility.ToJson(messageClient));
    // }

    

    void closeProgram()
    {
        // them nut stop game
        serverControler.StopServer();
    }
}
