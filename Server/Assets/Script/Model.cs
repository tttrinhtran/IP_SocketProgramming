using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;
using System.IO;
using Unity.Burst.CompilerServices;

public enum Type{
    Hello,
    Start,
    Lobby, 
    Play,
    Wait, 
    End


}
[System.Serializable]
public class Message
{
    public int point; 
    public Type Type;
    public Data data; 
} 

[System.Serializable]
public class Data{
    public string hint; 
    public string currentAnswer; 
}

public class ClientModel
{
     public string UserId;
     public int point; 
    public ClientModel()
    {
       
    }
    public string getUserId()
    {
        return UserId;
    }

  
}
[Serializable]
public class MessageClient
{
    public Type Type;
    public string Text;

    // Constructor to initialize the properties
    public MessageClient(Type type, string text)
    {
        Type = type;
        Text = text;
    }
    public MessageClient(){}
}

