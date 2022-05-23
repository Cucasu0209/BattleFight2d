using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController_test : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.NickName = Random.Range(2, 123121).ToString();
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        Debug.Log("mester");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("hehe", new RoomOptions() { MaxPlayers = 3 }, new TypedLobby("d", LobbyType.Default));
        Debug.Log("lobby");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("JoinRoom");
        Debug.Log(PhotonNetwork.PlayerList.Length);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("+1Plyer");
        Debug.Log(PhotonNetwork.PlayerList.Length);
    }
}
