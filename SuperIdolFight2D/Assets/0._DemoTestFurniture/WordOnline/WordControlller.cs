using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class WordControlller : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NickName = Random.Range(0, 10000).ToString();
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
    }
    public override void OnConnectedToMaster()
    {
        
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.CreateRoom("Room", new RoomOptions() { MaxPlayers = 4 });

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("??");
        base.OnJoinedRoom();
        PhotonNetwork.Instantiate("BattleEffect/Hehe", new Vector3(Random.Range(-6f, 6), Random.Range(-6, 6), 0), Quaternion.identity);
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        base.OnJoinRoomFailed(returnCode, message);
        PhotonNetwork.JoinRoom("Room");
    }
}
