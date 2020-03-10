using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLayoutGroup : MonoBehaviour
{
    public Text status;
    private bool isLoaded = false;

    [SerializeField]
    private GameObject _playerListingPrefab;
    private GameObject PlayerListingPrefab
    {
        get { return _playerListingPrefab; }
    }

    private List<PlayerListing> _playerListings = new List<PlayerListing>();
    private List<PlayerListing> PlayerListings
    {
        get { return _playerListings; }
    }

    //Called by photon whenever the master client is swithced.
    private void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
    }

    private void Update()
    {
        if (isLoaded)
        {
            string open = PhotonNetwork.CurrentRoom.IsOpen ? "Open" : "Closed";
            string visible = PhotonNetwork.CurrentRoom.IsOpen ? "True" : "False";
            status.text = "Status: " + open + '\n' + "Visible: " + visible;
        }
    }

    //Called by photon whenever you join a room.
    public void JoinPhotonRoom()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();

        Player[] photonPlayers = PhotonNetwork.PlayerList;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }

        isLoaded = true;
    }

    //Called by photon when a player joins the room.
    private void OnPhotonPlayerConnected(Player photonPlayer)
    {
        PlayerJoinedRoom(photonPlayer);
    }

    //Called by photon when a player leaves the room.
    private void OnPhotonPlayerDisconnected(Player photonPlayer)
    {
        PlayerLeftRoom(photonPlayer);
    }


    private void PlayerJoinedRoom(Player photonPlayer)
    {
        if (photonPlayer == null)
            return;

        PlayerLeftRoom(photonPlayer);

        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);

        PlayerListings.Add(playerListing);
    }

    private void PlayerLeftRoom(Player photonPlayer)
    {
        int index = PlayerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if (index != -1)
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }

    public void OnClickRoomState()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.CurrentRoom.IsOpen = !PhotonNetwork.CurrentRoom.IsOpen;
        PhotonNetwork.CurrentRoom.IsVisible = !PhotonNetwork.CurrentRoom.IsVisible;
    }

    public void OnClickLeaveRoom()
    {
        isLoaded = false;
        PhotonNetwork.LeaveRoom();
    }
}
