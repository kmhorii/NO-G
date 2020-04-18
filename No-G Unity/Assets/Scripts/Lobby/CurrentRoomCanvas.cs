using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviour
{
    public Text LevelName;

    public void OnClickStartSync()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

		PhotonNetwork.LoadLevel((LevelName.text).ToString());
    }
}
