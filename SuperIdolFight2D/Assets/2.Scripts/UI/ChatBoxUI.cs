using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ChatBoxUI : MonoBehaviour, IDragHandler
{
    #region Variables
    [SerializeField] private Button ChatTogger;

    [Header("Content Box")]
    [SerializeField] private Transform ChatBoxHolder;
    [SerializeField] private TextMeshProUGUI ChatContent;
    [SerializeField] private ScrollRect ChatContentScroll;

    [Header("Typer")]
    [SerializeField] private TMP_InputField ChatTyper;
    [SerializeField] private Button SendButton;

    [Header("Friend List")]
    [SerializeField] private ChatFriendListEntry friendEntryPrefab;
    [SerializeField] private Transform frientListContent;

    private RectTransform rectransform;
    private Dictionary<ChatFriendListEntry, string> channelChatList = new Dictionary<ChatFriendListEntry, string>();
    private ChatFriendListEntry activeFriendEntry;
    private bool isChatBoxOpening = true;
    #endregion

    #region Monobehaviour
    private void OnEnable()
    {
        rectransform = GetComponent<RectTransform>();
        ChatController_BF.Instance.Init();
        RegisterUIEvent();
        ChatController_BF.OnChatContentChange += RefreshChatBoxContent;
        FriendController_BF.OnFriendListUpdate += InstantiateFriendEntry;
        FriendController_BF.OnFriendListUpdate();
    }

    private void OnDisable()
    {
        ChatController_BF.OnChatContentChange -= RefreshChatBoxContent;
        FriendController_BF.OnFriendListUpdate -= InstantiateFriendEntry;
    }
    #endregion

    #region UI Event
    private void RegisterUIEvent()
    {
        SendButton.onClick.AddListener(OnSendButtonClick);
        ChatTogger.onClick.AddListener(ToggleChatBox);
    }

    private void OnSendButtonClick()
    {
        string _msg = ChatTyper.text;
        SendChatMessage(_msg);
        ChatTyper.text = "";
        Debug.Log("[ChatUIBox]: Send Message " + _msg);
    }
    private void ToggleChatBox()
    {
        if (isChatBoxOpening)
        {
            ChatBoxHolder.DOScale(Vector2.zero, 0.2f);
        }
        else
        {
            ChatBoxHolder.DOScale(Vector2.one, 0.2f);
        }
        isChatBoxOpening = !isChatBoxOpening;
    }
    #endregion

    #region Other Functions
    private void ClearFriendEntry()
    {
        foreach (ChatFriendListEntry _friend in channelChatList.Keys)
        {
            Destroy(_friend.gameObject);
        };
        channelChatList.Clear();
    }
    private void InstantiateFriendEntry()
    {
        ClearFriendEntry();

        // public channel
        ChatFriendListEntry _g_entry = Instantiate(friendEntryPrefab, frientListContent);
        _g_entry.SetInf("Public", OnFriendEntryClick);
        channelChatList.Add(_g_entry, ChatController_BF.Instance.GlobalChannel);

        // room channel
        ChatFriendListEntry _r_entry = Instantiate(friendEntryPrefab, frientListContent);
        _r_entry.SetInf("Room", OnFriendEntryClick);
        channelChatList.Add(_r_entry, ChatController_BF.Instance.RoomChannelKey);

        // private message
        foreach (string _friend in FriendController_BF.Instance._tempFriendList.Keys)
        {
            ChatFriendListEntry _f_entry = Instantiate(friendEntryPrefab, frientListContent);
            string _friendName = _friend.Split('#')[0];
            _f_entry.SetInf(_friendName, OnFriendEntryClick);
            channelChatList.Add(_f_entry, _friend);
        }

        OnFriendEntryClick(_g_entry);
    }
    public void OnFriendEntryClick(ChatFriendListEntry _entry)
    {
        foreach (ChatFriendListEntry _item in channelChatList.Keys)
        {
            _item.Active(_item == _entry);
        }
        activeFriendEntry = _entry;
        RefreshChatBoxContent(channelChatList[_entry]);
    }
    private void RefreshChatBoxContent(string _channelName)
    {
        if (_channelName == channelChatList[activeFriendEntry])
        {
            if (channelChatList[activeFriendEntry] == ChatController_BF.Instance.GlobalChannel)
            {
                ChatContent.SetText(ChatController_BF.Instance.GetGlobalChatContent());
            }
            else if (channelChatList[activeFriendEntry] == ChatController_BF.Instance.RoomChannelKey)
            {
                ChatContent.SetText(ChatController_BF.Instance.GetRoomChatContent());
            }
            else
            {
                ChatContent.SetText(ChatController_BF.Instance.GetFriendChatContent(channelChatList[activeFriendEntry]));
            }
            ChatContentScroll.DONormalizedPos(new Vector2(ChatContentScroll.normalizedPosition.x, 0), 0.5f);
        }

    }
    private void SendChatMessage(string _msg)
    {
        if (channelChatList[activeFriendEntry] == ChatController_BF.Instance.GlobalChannel)
        {
            ChatController_BF.Instance.SendGlobalChat(_msg);
        }
        else if (channelChatList[activeFriendEntry] == ChatController_BF.Instance.RoomChannelKey)
        {
            ChatController_BF.Instance.SendRoomChat(_msg);
        }
        else
        {
            ChatController_BF.Instance.SendPrivateChat(channelChatList[activeFriendEntry], _msg);
        }
    }


    #endregion

    #region Drag Drop
    public void OnDrag(PointerEventData eventData)
    {
        rectransform.anchoredPosition += eventData.delta;
    }
    #endregion
}
