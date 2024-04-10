using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;
using System.IO;
using Unity.Burst.CompilerServices;
using UnityEditor.PackageManager;

public class ServerController : MonoBehaviour
{
    private TcpListener tcpListener;
    private List<ClientHandler> clients = new List<ClientHandler>();

    private bool acceptingClients = true; // New variable to track whether to accept clients
    private List<Thread> clientThreads = new List<Thread>();

    public event Action<ClientHandler> ClientDisconnected;
    public event Action<ClientHandler, string> ClientMessageReceived; // Event to notify UIController of received messages
    void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, 8888);
            tcpListener.Start();
            Debug.Log("Server started. Waiting for connections...");
            Debug.Log("Server started successfully.");

            Thread acceptThread = new Thread(new ThreadStart(AcceptClients));
            acceptThread.Start();
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
    }

    private void AcceptClients()
    {
        while (acceptingClients) // Only accept clients if acceptingClients is true
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            Debug.Log("Client connected");
            ClientHandler clientHandler = new ClientHandler(tcpClient, this);
            Thread clientThread = new Thread(new ThreadStart(clientHandler.HandleClient));
            clientThread.Start();
            clients.Add(clientHandler);
        }
    }
    private void setUpClient(ClientHandler client)
    {
    

    }

 
   
    //--------------------------------------------------------
    public void HandleClientMessage(ClientHandler client, string message)
    {
        client.ConvertMessageToJSON(message);
        ClientMessageReceived?.Invoke(client,message);
    }
    public void NotifyClientDisconnected(ClientHandler client)
    {
        ClientDisconnected?.Invoke(client);
    }


    public void RemoveClient(ClientHandler client)
    {
        clients.Remove(client);

    }

    // Method to be called by the button
    public void StopAcceptingClients()
    {
        acceptingClients = false; 
        Debug.Log("Stopped accepting new clients");
    }

       public List<string> GetConnectedClients()
    {
        List<string> connectedClients = new List<string>();
        foreach (ClientHandler client in clients)
        {
            connectedClients.Add(client.GetClientInfo());
            Debug.Log("Info"+client.GetClientInfo());
        }
        return connectedClients;
    }

   
public void SendMessageToClient(string clientIdentifier, Message message)
{
    foreach (ClientHandler client in clients)
    {
        if (client.GetClientInfo() == clientIdentifier)
        {
            client.SendJSON(message);
            return; 
        }
    }
    Debug.LogError("Client not found: " + clientIdentifier);
}



//---------------------------------Stop Server---------------------------------
    public void StopServer()
    {
        try
        {
            // Wait for all client threads to finish
            foreach (ClientHandler client in clients)
            {
                client.StopHandling(); // Add a method in ClientHandler to gracefully stop handling
            }
            foreach (Thread thread in clientThreads)
            {
                thread.Join();
            }
            // Close all client connections
            foreach (ClientHandler client in clients)
            {
             
                client.CloseConnection(); // Add a method in ClientHandler to close the connection
            }

            tcpListener.Stop(); // Stop the TCP listener
            Debug.Log("Server stopped.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error stopping server: " + ex.Message);
        }
    }
}

public class ClientHandler
{
    private TcpClient client;
    private ServerController serverController;
    private NetworkStream stream;
    private bool isRunning = true;

    public ClientHandler(TcpClient client, ServerController serverController)
    {
        this.client = client;
        this.serverController = serverController;
        
    }

    public void HandleClient()
{
    try
    {
        // Get client stream
        stream = client.GetStream();
        
        while (isRunning)
        {
            string message = ConvertMessageToJSON(ReadMessage());
            
            Debug.Log(message);
            serverController.HandleClientMessage(this, message);
           
      
        }
    }
    catch (SocketException ex)
    {
        Debug.LogError("Client disconnected: " + ex.Message);
        serverController.NotifyClientDisconnected(this); // Notify when a client is disconnected
    }
    finally
    {
        // Cleanup
        serverController.RemoveClient(this);
        stream.Close();
        client.Close();
    }
}

private string ReadMessage()
{
    byte[] buffer = new byte[1024];
    int bytesRead = stream.Read(buffer, 0, buffer.Length);
    return System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
}
    public void SendMessage(string message)
    {
        try
        {
            // Convert the message string to bytes
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(message);

            // Write the bytes to the network stream
            stream.Write(buffer, 0, buffer.Length);

            Debug.Log("Message sent to client: " + message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error sending message: " + ex.Message);
        }
    }
    public void StopHandling()
    {
        isRunning = false; // Set isRunning flag to false to terminate the client thread

        // Clean up resources
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();

        // Remove this client from the list of clients in the server
        serverController.RemoveClient(this);
    }

    public void CloseConnection()
    {
        try
        {
            // Close the client's network stream and connection
            stream.Close();
            client.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error closing client connection: " + ex.Message);
        }
    }

    public void SendJSON(Message data){
        try
        {
            // Serialize the JSON object to a string using JsonUtility
            string json = JsonUtility.ToJson(data);
        
            Debug.Log("JSON object: " + json);

            // Convert the JSON string to bytes
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(json);

            // Write the bytes to the network stream
            stream.Write(buffer, 0, buffer.Length);

            Debug.Log("JSON object sent to client: " + json);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error sending JSON object: " + ex.Message);
        }
    }
    public string ConvertMessageToJSON(string message)
{
    try
    {
      
        string[] messageParts = message.Split(',');
        Type type= (Type)Enum.Parse(typeof(Type), messageParts[0]);
        if(type==Type.Start)
        {
            StartMessage startMessage = new StartMessage(Type.Start, messageParts[1]);
            return JsonUtility.ToJson(startMessage);
        }
        else if(type==Type.Play)
        {

            // PlayMessage playMessage = new PlayMessage();
            // playMessage.type = Type.Play;
            // playMessage.point = int.Parse(messageParts[1]);
            // playMessage.data = new Data();
            // playMessage.data.hint= messageParts[2];
            // playMessage.data.currentAnswer = messageParts[3];
            // return JsonUtility.ToJson(playMessage);
            return null;
        }
        
        return null;

      
    }
    catch (Exception ex)
    {
        Debug.LogError("Error converting message to JSON: " + ex.Message);
        return null;
    }
}


    public string GetClientInfo()
{
    if (client != null && client.Client.RemoteEndPoint != null)
    {
        return client.Client.RemoteEndPoint.ToString();
    }
    return "Unknown";
}


}