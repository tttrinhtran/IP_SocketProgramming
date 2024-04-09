using System;

[Serializable]
public enum MessageType
{
    Hello,
    Start,
    Play,
    End
}

[Serializable]
public class Message
{
    public MessageType Type;
    public string Text;
    public byte[] Data;

    // Constructor to initialize the properties
    public Message(MessageType type, string text, byte[] data)
    {
        Type = type;
        Text = text;
        Data = data;
    }
}

