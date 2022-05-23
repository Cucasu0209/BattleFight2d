using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class ConnectToServer_t : MonoBehaviourPunCallbacks
{
    enum PanelTypes {
        Connect,
        Lobby,
        Room
    }
    [Header("Connect To Master")]
    [SerializeField] private GameObject PanelConnectToMaster;
    [SerializeField] private TextMeshProUGUI ConnectText;
    [SerializeField] private TMP_InputField NameInput;
    [SerializeField] private Button ConnectButton;

    [Header("Lobby")]
    [SerializeField] private GameObject PanelLobby;
    [SerializeField] private TMP_InputField RoomNameInput;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button DisConnectButton;
    [SerializeField] private Transform RoomListContent;


    [Header("Room")]
    [SerializeField] private GameObject PanelRoom;

    [Header("Prefab")]
    public RoomEntry_t RoomEntryPrefab;



    private List<RoomEntry_t> ListRoomEntryUI;


    void Start()
    {
        RegisterButtonEvent();
        ListRoomEntryUI = new List<RoomEntry_t>();
    }

    private void RegisterButtonEvent()
    {
        ConnectButton.onClick.AddListener(OnConnectBtnClick);
        CreateRoomButton.onClick.AddListener(OnCreateRoomBtnClick);
        DisConnectButton.onClick.AddListener(OnDisConnectClick);
    }

    #region Button CallBacks
    private void OnConnectBtnClick()
    {
        PhotonNetwork.NickName = NameInput.text;
        PhotonNetwork.ConnectUsingSettings();
        ConnectText.text = "Connecting.....";
    }

    private void OnCreateRoomBtnClick()
    {
        Debug.Log("Create click");
        PhotonNetwork.CreateRoom(RoomNameInput.text);
    }

    private void OnDisConnectClick()
    {
        PhotonNetwork.Disconnect();
    }

    #endregion

    #region Other Functions
    private void ChangeLocalScene(PanelTypes _type)
    {
        PanelConnectToMaster.SetActive(_type == PanelTypes.Connect);
        PanelLobby.SetActive(_type == PanelTypes.Lobby);
        PanelRoom.SetActive(_type == PanelTypes.Room);
    }
    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach(RoomEntry_t _entry in ListRoomEntryUI)
        {
            Destroy(_entry.gameObject);
        }
        ListRoomEntryUI.Clear();

        foreach(RoomInfo _room in roomList)
        {
            RoomEntry_t newRoomEntry = Instantiate(RoomEntryPrefab, RoomListContent);
            newRoomEntry.SetInfo(_room.Name);
            ListRoomEntryUI.Add(newRoomEntry);
        }
    }
    #endregion

    #region PUN Callbacks
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        ChangeLocalScene(PanelTypes.Lobby);
        PhotonNetwork.JoinLobby();
        Debug.Log("On Connect To Master");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        UpdateRoomList(roomList);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        ChangeLocalScene(PanelTypes.Room);
    }
    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        ChangeLocalScene(PanelTypes.Connect);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ChangeLocalScene(PanelTypes.Connect);
    }
    #endregion

}
