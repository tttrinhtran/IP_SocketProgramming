using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets; // Add this line
using System.Collections.Generic;
using System.Threading;

public class ServerController : MonoBehaviour
{
    private TcpListener tcpListener;
    private List<ClientHandler> clients = new List<ClientHandler>();
    private bool canBroadcast = false;

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
        while (true)
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            Debug.Log("Client connected");

            ClientHandler clientHandler = new ClientHandler(tcpClient, this);
            Thread clientThread = new Thread(new ThreadStart(clientHandler.HandleClient));
            clientThread.Start();

            clients.Add(clientHandler);

            // Set canBroadcast to true if at least one client is connected
            if (!canBroadcast && clients.Count > 0)
            {
                canBroadcast = true;
            }
        }
    }

    public void BroadcastMessage(string message, ClientHandler sender)
    {
        // Only broadcast message if at least one client is connected
        if (canBroadcast)
        {
            lock (clients)
            {
                foreach (ClientHandler client in clients)
                {
                    if (client != sender)
                    {
                        client.SendMessage(message);
                    }
                }
            }
        }
    }

    public void RemoveClient(ClientHandler client)
    {
        clients.Remove(client);
        // If no clients are left, set canBroadcast back to false
        if (clients.Count == 0)
        {
            canBroadcast = false;
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

            // Send a "hello" message to the client
            SendMessage("Hello from the server!");

            // Handle client messages
            while (isRunning)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Broadcast message to all clients
                serverController.BroadcastMessage(message, this);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
        finally
        {
            // Cleanup
            serverController.RemoveClient(this);
            stream.Close();
            client.Close();
        }
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

}
