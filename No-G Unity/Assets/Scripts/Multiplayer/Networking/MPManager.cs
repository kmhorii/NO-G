using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MPManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
		PhotonNetwork.ConnectUsingSettings();
		//PhotonNetwork.ConnectToRegion("usw");
    }

	public override void OnConnectedToMaster()
	{
		Debug.Log("We are now connected to photon");
	}

	public void JoinGame()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.JoinRandomRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		CreateGame();
	}
	public void CreateGame()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
		RoomOptions ro = new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true};
		PhotonNetwork.CreateRoom("defaultGame", ro, TypedLobby.Default);

		SceneManager.LoadScene("MergeTest");
	}
}
