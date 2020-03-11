using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    public string Name;

    public string PlayerPick;
    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = true;

    public byte Version = 1;

    //public GameObject[] player;
    public Transform Lobby;

    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;

    //private LevelManager lm;
    private GameManager gm;

    public void Start()
    {      
        if (!PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
            PhotonNetwork.ConnectUsingSettings();
            //PhotonNetwork.GameVersion = "1";

            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    private void Update()
    {
        if (Lobby == null && GameObject.FindGameObjectWithTag("Lobby") != null)
        {
            Lobby = GameObject.FindGameObjectWithTag("Lobby").transform;
        }
    }

    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage
    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("");
    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed()  was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 10}, null);");
        PhotonNetwork.JoinOrCreateRoom("Room" + Random.Range(100, 130), new RoomOptions() { IsVisible = true, MaxPlayers = 4 }, TypedLobby.Default);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");

    }

}
