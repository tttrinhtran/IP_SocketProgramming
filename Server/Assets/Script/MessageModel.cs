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
