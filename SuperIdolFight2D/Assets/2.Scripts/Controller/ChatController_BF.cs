using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ChatController_BF : MonoBehaviourPunCallbacks, IChatClientListener
{
    #region Singleton
    protected ChatController_BF() { }

    private static ChatController_BF f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static ChatController_BF Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<ChatController_BF>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    public static ChatController_BF AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<ChatController_BF>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
    public static T AddToScene<T>(string gameObjectName, bool isSingleton, bool selectGameObjectAfterCreation = false) where T : MonoBehaviour
    {
        var component = FindObjectOfType<T>();
        if (component != null && isSingleton)
        {
            Debug.Log("Cannot add another " + typeof(T).Name + " to this Scene because you don't need more than one.");
#if UNITY_EDITOR
            UnityEditor.Selection.activeObject = component;
#endif
            return component;
        }

        component = new GameObject(gameObjectName, typeof(T)).GetComponent<T>();

#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(component.gameObject, "Created " + gameObjectName);
        if (selectGameObjectAfterCreation) UnityEditor.Selection.activeObject = component.gameObject;
#endif
        return component;
    }
    public void Awake()
    {
        if (f_instance != null && f_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        f_instance = this;
        DontDestroyOnLoad(gameObject);


    }
    public void Init()
    {
        Debug.Log("Init Singleton " + this.name);
    }
    #endregion

    #region Variables
    private ChatClient chatClient;

    public readonly string GlobalChannel = "GlobalChannel";
    public readonly string RoomChannelKey = "RoomChannel";

    public string _tempGlobalChatContent = "";
    public string _tempRoomChatContent = "";
    public string currentRoom = "";
    public Dictionary<string, string> _tempFriendChatContent = new Dictionary<string, string>(); // (name#id, content)

    public static Action<string> OnChatContentChange;
    #endregion

    #region Monobehaviour
    private void Update()
    {
        chatClient.Service();
    }

    public override void OnEnable()
    {
        ConnectChatClient();
    }
    public override void OnDisable()
    {

    }
    #endregion

    #region Photon Chat
    public void ConnectChatClient()
    {
        chatClient = new ChatClient(this);
        // Set your favourite region. "EU", "US", and "ASIA" are currently supported.
        chatClient.ChatRegion = "EU";
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(PlayerData.LoadUniqueID()));
    }

    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("[ChatController]: DebugReturn " + message);
    }

    void IChatClientListener.OnDisconnected()
    {
        Debug.Log("[ChatController]: OnDisconnected");
    }

    void IChatClientListener.OnConnected()
    {
        chatClient.Subscribe(new string[] { GlobalChannel });
        Debug.Log("[ChatController]: OnConnected");
    }

    void IChatClientListener.OnChatStateChange(ChatState state)
    {
        Debug.Log("[ChatController]: OnChatStateChange");
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {

        for (int i = 0; i < senders.Length; i++)
        {
            string sender = senders[i];
            object message = messages[i];
            Debug.Log(sender);
            if (sender == PlayerData.LoadUniqueID()) continue;
            try
            {
                var _msg = JsonUtility.FromJson<ChatMessage_BF>((string)message);
                if (_msg is ChatMessage_BF)
                {
                    Debug.Log("haha");
                    ChatMessage_BF messData = (ChatMessage_BF)_msg;
                    string senderName = messData.displayName;
                    string senderId = messData.id;
                    string _message = messData.msg;
                    string _channel = messData.channel;
                    if (channelName == GlobalChannel)
                    {
                        AddGlobalChat(senderName, _message);
                    }
                    else if (channelName.Split('#').Length >= 2 && channelName.Split('#')[1] == RoomChannelKey)
                    {
                        AddRoomChat(senderName, _message);
                    }
                }
            }
            catch (Exception e) { }
        }

        Debug.Log("[ChatController]: OnGetMessages");
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        if (sender == PlayerData.LoadUniqueID()) return;

        var _msg = JsonUtility.FromJson<object>((string)message);
        if (_msg is ChatMessage_BF)
        {
            ChatMessage_BF messData = (ChatMessage_BF)_msg;
            string senderName = messData.displayName;
            string senderId = messData.id;
            string _message = messData.msg;
            if (_tempFriendChatContent.ContainsKey(senderName + "#" + senderId)) _tempFriendChatContent[senderName + "#" + senderId] = _message;
            else _tempFriendChatContent.Add(senderName + "#" + senderId, _message);
        }
        Debug.Log("[ChatController]: OnPrivateMessage");
    }

    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("[ChatController]: OnSubscribed");
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        Debug.Log("[ChatController]: OnUnsubscribed");
    }

    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("[ChatController]: OnStatusUpdate");
    }

    void IChatClientListener.OnUserSubscribed(string channel, string user)
    {
        Debug.Log("[ChatController]: OnUserSubscribed");
    }

    void IChatClientListener.OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log("[ChatController]: OnUserUnsubscribed");
    }
    #endregion

    #region Photon PUN
    public override void OnJoinedRoom()
    {
        currentRoom = PhotonNetwork.CurrentRoom + "#" + RoomChannelKey;
        chatClient.Subscribe(new string[] { currentRoom });
    }
    public override void OnLeftRoom()
    {
        chatClient.Unsubscribe(new string[] { currentRoom });
        _tempRoomChatContent = "";
        if (OnChatContentChange != null) OnChatContentChange(RoomChannelKey);
    }
    #endregion

    #region Send Chat Message
    public void SendPrivateChat(string _targetName, string _msg)
    {

        if (_targetName.Split('#').Length < 2) return;
        string _targetId = "";
        _targetId = _targetName.Split('#')[1];

        ChatMessage_BF newMsg = new ChatMessage_BF()
        {
            mesType = MessageChatType.PrivateChat,
            id = PlayerData.LoadUniqueID(),
            displayName = PlayerData.LoadDisplayName(),
            targetId = _targetId,
            msg = _msg,
        };
        string strMsg = JsonUtility.ToJson(newMsg);

        chatClient.SendPrivateMessage(_targetId, strMsg);
        AddPrivateChat(_targetName, PlayerData.LoadDisplayName(), _msg);
    }
    public void SendGlobalChat(string _msg)
    {
        ChatMessage_BF newMsg = new ChatMessage_BF()
        {
            mesType = MessageChatType.GlobalChat,
            id = PlayerData.LoadUniqueID(),
            displayName = PlayerData.LoadDisplayName(),
            channel = GlobalChannel,
            msg = _msg,
        };
        string strMsg = JsonUtility.ToJson(newMsg);

        chatClient.PublishMessage(GlobalChannel, strMsg);
        AddGlobalChat(PlayerData.LoadDisplayName(), _msg);
    }
    public void SendRoomChat(string _msg)
    {
        if (!PhotonNetwork.InRoom) return;
        string _roomChannel = PhotonNetwork.CurrentRoom.Name + "#" + RoomChannelKey;
        ChatMessage_BF newMsg = new ChatMessage_BF()
        {
            mesType = MessageChatType.RoomChat,
            id = PlayerData.LoadUniqueID(),
            displayName = PlayerData.LoadDisplayName(),
            channel = _roomChannel,
            msg = _msg,
        };
        string strMsg = JsonUtility.ToJson(newMsg);

        chatClient.PublishMessage(_roomChannel, strMsg);
        AddRoomChat(PlayerData.LoadDisplayName(), _msg);
    }
    #endregion

    #region Get Chat Content
    public string GetGlobalChatContent()
    {
        return _tempGlobalChatContent;
    }

    public string GetRoomChatContent()
    {
        return _tempRoomChatContent;
    }

    public string GetFriendChatContent(string _friend)
    {
        if (_tempFriendChatContent.ContainsKey(_friend) == false)
        {
            _tempFriendChatContent.Add(_friend, "");
        }
        return _tempFriendChatContent[_friend];
    }
    #endregion

    #region Add Chat Content
    public void AddPrivateChat(string _targetDName, string _name, string _msg)
    {
        if (_tempFriendChatContent.ContainsKey(_targetDName) == false) _tempFriendChatContent.Add(_targetDName, "");
        _tempFriendChatContent[_targetDName] += string.Format(" [{0}]: {1}\n", _name, _msg);
        if (OnChatContentChange != null) OnChatContentChange(_targetDName);
    }
    public void AddGlobalChat(string _name, string _msg)
    {
        _tempGlobalChatContent += string.Format(" [{0}]: {1}\n", _name, _msg);
        if (OnChatContentChange != null) OnChatContentChange(GlobalChannel);
    }
    public void AddRoomChat(string _name, string _msg)
    {
        _tempRoomChatContent += string.Format(" [{0}]: {1}\n", _name, _msg);
        if (OnChatContentChange != null) OnChatContentChange(RoomChannelKey);
    }
    #endregion
}
