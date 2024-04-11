using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
public class ClientController : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private bool isConnected = false;
    public Canvas StartScene, PlayScene, EndScene, LobbyScene;
    public bool isSentName = false;
    

    void Start()
    {
        PlayScene.gameObject.SetActive(false);
        EndScene.gameObject.SetActive(false);
        LobbyScene.gameObject.SetActive(false);
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
            StartMessage startMessage = new StartMessage(MessageType.Hello, "Client connected");
            ;
            string  jsonString = JsonUtility.ToJson(startMessage);
            // Send the start message to the server
            SendMessageToServer(jsonString);
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
            Debug.Log("Processing received message: " + messageJson);

            // Deserialize JSON message into a dictionary
            Dictionary<string, object> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(messageJson);
            // Log deserialized dictionary contents
            Debug.Log("Received dictionary with " + jsonDict.Count + " key-value pairs:");
            MessageType messageType = 0;
            foreach (var kvp in jsonDict)
            {
                Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value);
                if (kvp.Key == "Type")
                {
                    Debug.Log("Message type: " + kvp.Value);
                    messageType = (MessageType)Enum.Parse(typeof(MessageType), kvp.Value.ToString());

                }
            }
            switch (messageType)
            {
                case MessageType.Hello:
                    Debug.Log("Received Hello message");
                    break;
                case MessageType.Start:
                    Debug.Log("Received Start message");
                    StartMessage startMessage = JsonConvert.DeserializeObject<StartMessage>(messageJson);
                    HandleStartMessage(startMessage);
                    break;
                case MessageType.Lobby:
                    Debug.Log("isSentName: " + isSentName);
                    if (isSentName)
                    {
                        Debug.Log("Received Lobby message");
                        StartMessage lobbyMessage = JsonConvert.DeserializeObject<StartMessage>(messageJson);
                        HandleLobbyMessage(lobbyMessage);
                    }
                    break;
                case MessageType.Wait:
                case MessageType.Play:
                    Debug.Log("Received Play message");
                    PlayMessage playMessage = JsonConvert.DeserializeObject<PlayMessage>(messageJson);
                    HandlePlayMessage(playMessage);
                    break;
                // Add cases for other message types
                default:
                    Debug.LogWarning("Received unrecognized message type: " + messageType);
                    break;
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error deserializing JSON: " + ex.Message);
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
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);

            // Send the bytes to the server
            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("Sent message to server: " + message);
            StartMessage startMessage = JsonConvert.DeserializeObject<StartMessage>(message);
            if (startMessage.Type==MessageType.Start)
            {
                isSentName = true;
            }
            // Convert the JSON string to bytes
            

        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to send message to server: " + ex.Message);
        }
    }
    private void HandleStartMessage(StartMessage startMessage)
    {
        // Access and process the properties of the StartMessage
        Debug.Log("Received Start message. Text: " + startMessage.Text);
        if (startMessage.Text == "OK!")
        {
            StartScene.gameObject.SetActive(false);
            PlayScene.gameObject.SetActive(false);
            EndScene.gameObject.SetActive(false);
            LobbyScene.gameObject.SetActive(true);
        }
      
    }
    private void HandlePlayMessage(PlayMessage playMessage)
    {
       StartScene.gameObject.SetActive(false);
        PlayScene.gameObject.SetActive(true);
        EndScene.gameObject.SetActive(false);
        LobbyScene.gameObject.SetActive(false);
        Debug.Log("okay");
        PlayScene.GetComponent<playSceneController>().updateUI(playMessage);

    }
    private void HandleLobbyMessage(StartMessage startMessage)
    {
        StartScene.gameObject.SetActive(false);
        PlayScene.gameObject.SetActive(false);
        EndScene.gameObject.SetActive(false);
        LobbyScene.gameObject.SetActive(true);
        Debug.Log("Received Lobby message. Text: " + startMessage.Text);
        LobbyScene.GetComponent<lobbySceneController>().updateUI(startMessage.Text);
    }

}
