using System;

[Serializable]
public class ChatMessage_BF
{
    public ChatMessage_BF() { }

    public MessageChatType mesType;

    public string displayName;
    public string id;

    public string channel;
    public string targetId;

    public string msg;

}

public enum MessageChatType
{
    GlobalChat,
    PrivateChat,
    RoomChat
}
