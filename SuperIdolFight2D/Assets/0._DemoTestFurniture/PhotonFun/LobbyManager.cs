using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public static LobbyManager instance;


    private void Awake()
    {
        instance = this;
    }

    public TMP_InputField RoomNameInput;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public Button ButtonCreateRoom;
    public Button LeaveRoom;
    public TextMeshProUGUI RoomName;

    public RoomItem RoomItemPrejfab;
    List<RoomItem> RoomItemList = new List<RoomItem>();
    public Transform ContentObject;

    float UpdateNextTime = 0;
    float timeBtwUpdate = 1.5f;

    private void Start()
    {
        ButtonCreateRoom.onClick.AddListener(OnClickCreate);
        LeaveRoom.onClick.AddListener(OnClickLeaveRoom);
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
    }
    public void OnClickCreate()
    {
        if (RoomNameInput.tag.Length >= 1)
        {
            PhotonNetwork.CreateRoom(RoomNameInput.text, new RoomOptions() { MaxPlayers = 3 });

        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(true);
        RoomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        if (Time.time > UpdateNextTime)
        {
            StartCoroutine(UpdateRoomList(roomList, 0));
            UpdateNextTime = Time.time + timeBtwUpdate;
        }
        else
        {
            StartCoroutine(UpdateRoomList(roomList, UpdateNextTime - Time.time));
        }


    }

    private IEnumerator UpdateRoomList(List<RoomInfo> list, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        foreach (RoomItem item in RoomItemList)
        {
            Destroy(item.gameObject);

        }
        RoomItemList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(RoomItemPrejfab, ContentObject);
            newRoom.Settext(room.Name);
            RoomItemList.Add(newRoom);
        }
    }

    public void JoinRoom(string _roomName)
    {
        PhotonNetwork.JoinRoom(_roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }
}
