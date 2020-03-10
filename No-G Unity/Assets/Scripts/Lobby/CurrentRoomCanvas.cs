using Photon.Pun;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{
    public void OnClickStartSync()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

		PhotonNetwork.LoadLevel("Photon");
    }
}
