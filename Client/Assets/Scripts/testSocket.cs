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
    public Canvas StartScene, PlayScene, EndScene, LobbyScene;
    

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
            // Deserialize JSON data into appropriate message class based on its type
            if (messageJson.Contains("Start"))
            {
                StartMessage startMessage = JsonUtility.FromJson<StartMessage>(messageJson);
                HandleStartMessage(startMessage);
            }
            else if (messageJson.Contains("Play") || messageJson.Contains("Wait"))
            {
                PlayMessage playMessage = JsonUtility.FromJson<PlayMessage>(messageJson);
                HandlePlayMessage(playMessage);
            }
            else if (messageJson.Contains("End"))
            {
               // EndMessage endMessage = JsonUtility.FromJson<EndMessage>(messageJson);
                // HandleEndMessage(endMessage);
            }
            else if (messageJson.Contains("Lobby"))
            {
                //LobbyMessage lobbyMessage = JsonUtility.FromJson<LobbyMessage>(messageJson);
                // HandleLobbyMessage(lobbyMessage);
            }
            else
            {
                Debug.LogWarning("Received unrecognized message: " + messageJson);
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
    public void SendMessageToServer(string message)
    {
        try
        {            

            // Convert the JSON string to bytes
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);

            // Send the bytes to the server
            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("Sent message to server: " + message);

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
            PlayScene.gameObject.SetActive(true);
            EndScene.gameObject.SetActive(false);
            LobbyScene.gameObject.SetActive(false);
        }
      
    }
    private void HandlePlayMessage(PlayMessage playMessage)
    {
       StartScene.gameObject.SetActive(false);
        PlayScene.gameObject.SetActive(true);
        EndScene.gameObject.SetActive(false);
        LobbyScene.gameObject.SetActive(false);
        
        playSceneController playSceneController = PlayScene.GetComponent<playSceneController>();
        playSceneController.updateUI(playMessage);

    }


}