using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class HeheController : MonoBehaviour
{
    PhotonView PV;
    TextMeshProUGUI content;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        content = GetComponentInChildren<TextMeshProUGUI>();


    }

    [PunRPC]
    void Hello(string nickname)
    {

        Debug.Log("Hahahahah");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (PV.IsMine)
                PV.RPC(nameof(Hello), RpcTarget.All, PhotonNetwork.NickName );
        }
    }




}
