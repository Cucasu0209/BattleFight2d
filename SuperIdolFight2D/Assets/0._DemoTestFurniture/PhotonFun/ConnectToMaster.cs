using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ConnectToMaster : MonoBehaviourPunCallbacks
{
    public TMP_InputField UserNameInput;
    public TextMeshProUGUI buttonText;
    public Button ConnectButton;

    private void Start()
    {
        ConnectButton.onClick.AddListener(OnClickConnect);
    }
    public void OnClickConnect()
    {
        if (UserNameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = UserNameInput.text;
            buttonText.text = "Connecting......";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene("Lobby");

    }
}
