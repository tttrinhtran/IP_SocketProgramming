using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientController : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private bool isConnected = false;

    void Start()
    {
        ConnectToServer();
        SendMessageToServer("Hello, server!");
    }

    void OnDestroy()
    {
        DisconnectFromServer();
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(IPAddress.Loopback, 8888); // Connect to localhost (127.0.0.1) on port 8888
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("Connected to server.");

            // Start a new thread to receive messages from the server
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessages));
            receiveThread.Start();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to connect to server: " + ex.Message);
        }
    }

    private void ReceiveMessages()
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (isConnected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received message from server: " + message);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to receive message from server: " + ex.Message);
        }
    }

    private void DisconnectFromServer()
    {
        if (client != null)
        {
            isConnected = false;
            stream.Close();
            client.Close();
            Debug.Log("Disconnected from server.");
        }
    }

    // Example method to send a message to the server
    public void SendMessageToServer(string message)
    {
        try
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("Sent message to server: " + message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to send message to server: " + ex.Message);
        }
    }
}
