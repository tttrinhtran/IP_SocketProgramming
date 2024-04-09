using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public class ClientController : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private bool isConnected = false;
    public Canvas StartScene, PlayScene, EndScene;
    

    void Start()
    {
        PlayScene.gameObject.SetActive(false);
        EndScene.gameObject.SetActive(false);
        ConnectToServer();
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
            client.Connect("127.0.0.1", 8888); // Connect to localhost (127.0.0.1) on port 8888
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("Connected to server.");

            // Start a new thread to receive messages from the server
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessages));
            receiveThread.Start();
            Message startMessage = new Message(MessageType.Hello, "Client connected", new byte[0]);
            ;

            // Send the start message to the server
            SendMessageToServer(startMessage);
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
                if (bytesRead == 0)
                {
                    // Connection closed by the server
                    Debug.Log("Connection closed by server.");
                    isConnected = false;
                    break;
                }
                string messageJson = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received message from server: " + messageJson);

                // Process received message on the main thread
                MainThreadDispatcher.Enqueue(() =>
                {
                    ProcessReceivedMessage(messageJson);
                });
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to receive message from server: " + ex.Message);
        }
    }

    private void ProcessReceivedMessage(string messageJson)
    {
        try
        {
            // Parse JSON data
            Message receivedMessage = JsonUtility.FromJson<Message>(messageJson);

            // Now you can access the properties of the received message
            Debug.Log("Received message Type: " + receivedMessage.Type);
            Debug.Log("Received message Text: " + receivedMessage.Text);
            // Debug.Log("Received message Data: " + receivedMessage.Data); // Use this line if Data is not a byte array

            // Example: Handle the received message based on its type
            switch (receivedMessage.Type)
            {
                case MessageType.Start:
                    StartScene.gameObject.SetActive(false);
                    PlayScene.gameObject.SetActive(true);
                    EndScene.gameObject.SetActive(false);
                    break;
                case MessageType.Play:
                    // Handle play message
                    break;
                case MessageType.End:
                    // Handle end message
                    break;
                default:
                    // Handle unrecognized message type
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error processing received message: " + ex.Message);
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
    public void SendMessageToServer(Message message)
    {
        try
        {
            // Convert the message to a JSON string
            string jsonString = JsonUtility.ToJson(message);

            // Convert the JSON string to bytes
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonString);

            // Send the bytes to the server
            stream.Write(buffer, 0, buffer.Length);

            Debug.Log("Sent message to server: " + message.Text);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to send message to server: " + ex.Message);
        }
    }

}