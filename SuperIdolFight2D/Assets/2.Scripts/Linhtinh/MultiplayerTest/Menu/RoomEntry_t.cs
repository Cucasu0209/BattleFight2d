using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class RoomEntry_t : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI RoomNameTxt;
    [SerializeField] private TextMeshProUGUI RoomSizeTxt;

    public void SetInfo(string _roomName)
    {
        RoomNameTxt.text = _roomName;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnJoinRoomClick);
    }

    private void OnJoinRoomClick()
    {
        PhotonNetwork.JoinRoom(RoomNameTxt.text);
    }
}
