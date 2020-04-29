using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyCanvas : MonoBehaviour {
    public string RoomName;

    [SerializeField]
    private RoomLayoutGroup _roomLayoutGroup;
    private RoomLayoutGroup RoomLayoutGroup
    {
        get { return _roomLayoutGroup; }
    }

    public void OnClickJoinRoom()
    {
        if (RoomName != null)
        {
            if (PhotonNetwork.NickName.Length == 0)
            {
                string[] Names = new string[6] { "Zach", "Matt", "David", "Ori", "Derek", "Chris" };
                PhotonNetwork.NickName = Names[Random.Range(0, 6)];
            }

            if (PhotonNetwork.JoinRoom(RoomName)) ;
        }
        else
        {
            print("Join room failed.");
        }
    }

    public void OnClickBack()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CloseConnection(PhotonNetwork.LocalPlayer);
        }

        SceneManager.LoadScene("Photon");
    }

}
