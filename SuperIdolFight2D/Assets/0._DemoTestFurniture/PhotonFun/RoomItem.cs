using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Button JoinRoom;

    private void Start()
    {
        JoinRoom.onClick.AddListener(ClickJoinRoom);
    }
    public void Settext(string txt)
    {
        text.text = txt;

    }
    public void ClickJoinRoom()
    {
        LobbyManager.instance.JoinRoom(text.text);
    }
}
