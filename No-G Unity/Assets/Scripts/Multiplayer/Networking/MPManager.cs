﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MPManager : MonoBehaviourPunCallbacks
{

	public GameObject[] EnableObjectsOnConnect;
	public GameObject[] DisableObjectsOnConnect;

	public GameObject CharacterText;
	// Start is called before the first frame update
	void Start()
    {
        //PhotonNetwork.ConnectToRegion(CloudRe);
        //PhotonNetwork.ConnectUsingSettings();
		//PhotonNetwork.NickName = PlayerInfo.Name;
		//PhotonNetwork.ConnectToRegion("usw");

		CharacterText.GetComponent<Text>().text = PlayerInfo.Name;
    }

    public void OpenStats()
    {
        SceneManager.LoadScene("Stats", LoadSceneMode.Additive);
    }

    public void JoinGame()
    {
        SceneManager.LoadScene("MainLobby", LoadSceneMode.Single);

        /*
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("HallwayMap");
        }
        else
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.JoinRandomRoom();
        }
        */
        //Debug.Log("Joining room");
    }
    /*
    private void Update()
    {
        //if (Input.GetButtonDown("Submit"))
        //{
        //    JoinGame();
        //}
        //Debug.Log(PhotonNetwork.NetworkingClient.Server);

        //jesus christ;
        //Debug.Log(PhotonNetwork.CloudRegion)
        //Debug.Log("App ID: " + PhotonNetwork.NetworkingClient.AppId);
        //Debug.Log("App Version: " + PhotonNetwork.NetworkingClient.AppVersion);
        //Debug.Log("Game server address: " + PhotonNetwork.NetworkingClient.GameServerAddress);
        //Debug.Log("Region: + " + PhotonNetwork.NetworkingClient.RegionHandler.BestRegion);
        //Debug.Log("Region+: " + PhotonNetwork.NetworkingClient.RegionHandler);
        //Debug.Log("User ID: " + PhotonNetwork.NetworkingClient.UserId);
    }

    public override void OnConnectedToMaster()
	{
		foreach(GameObject obj in EnableObjectsOnConnect) obj.SetActive(true);
		foreach(GameObject obj in DisableObjectsOnConnect) obj.SetActive(false);

		
		//Debug.Log("We are now connected to photon");
	}	

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		CreateGame();
	}

	public void CreateGame()
	{
		PhotonNetwork.AutomaticallySyncScene = true;

		int rnd = Random.Range(0, 10000);
		RoomOptions ro = new RoomOptions { MaxPlayers = 4, IsOpen = true, IsVisible = true};
		PhotonNetwork.CreateRoom("default#" + rnd, ro, TypedLobby.Default);
		Debug.Log("Creating room");

	}

	public override void OnJoinedRoom()
	{
		SceneManager.LoadScene("HallwayMap");
	}
    */
}
