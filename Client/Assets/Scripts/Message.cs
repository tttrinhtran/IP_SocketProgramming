using System;

[Serializable]
public enum MessageType
{
    Hello,
    Start,
    Lobby,
    Play,
    Wait,
    End
}

[Serializable]
public class StartMessage
{
    public MessageType Type;
    public string Text;

    // Constructor to initialize the properties
    public StartMessage(MessageType type, string text)
    {
        Type = type;
        Text = text;
    }
}

[Serializable]
public class PlayMessage
{
    public MessageType type;
    public int point;
    public Data data;
}

[Serializable]
public class Data
{
    public string hint;
    public string currentAnswer;
}

