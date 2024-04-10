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
    End,
    Wait

}
[System.Serializable]
public class Message
{

    public int point; 
    public string hint;
    public Type type;
    public Data data; 
} 

[System.Serializable]
public class Data{
    public string hint; 
    public string currentAnswer; 
}

public class ClientModel
{
     string UserId;
     public int point; 
     ClientHandler clientHandler;

    public ClientModel(ClientHandler clientHandler)
    {
    
    
        this.clientHandler = clientHandler;
    }
    public void setUserID(string UserId){
        this.UserId = UserId;
    }
    public void setPoint(int point){
        this.point = point;
    }

    public int getPoint(){
        return this.point;
    }
    public string getUserId(){
        return this.UserId;
    }

  
}
[Serializable]
public class StartMessage
{
    public Type Type;
    public string Text;

    // Constructor to initialize the properties
    public StartMessage(Type type, string text)
    {
        Type = type;
        Text = text;
    }
}

//[Serializable]
// public class PlayMessage
// { 
//     public Type type;
//     public int point;
//     public DataClient data;
// }
