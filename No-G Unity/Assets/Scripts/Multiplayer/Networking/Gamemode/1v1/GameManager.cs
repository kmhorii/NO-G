using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class GameManager : MonoBehaviourPun, IPunObservable
{
	public float SpawnTime;
	float timer = 0;
	bool hasPlayerSpawned = false;
	int CurPlayers;

	public GameObject winGame;
	public GameObject loseGame;
	public Timer timerClock;

	// Start is called before the first frame update
	void Start()
    {
		GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        player.GetComponent<PlayerMovement>().PlayerName = PlayerInfo.Name;

		photonView.RPC("AddPlayerCount", RpcTarget.All);

		photonView.RPC("NewPlayerJoined", RpcTarget.All);

		foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
		{
			plyr.name = plyr.GetComponent<PhotonView>().Owner.NickName;
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (GameObject.FindGameObjectsWithTag("Player").Length == 2) StartGame();

		foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
		{
			if(plyr.name.ToLower().Contains("player(clone)")) plyr.name = plyr.GetComponent<PhotonView>().Owner.NickName;

			if (plyr.GetComponent<PlayerHealth>().isDead)
			{
				if (!plyr.GetComponent<PlayerHealth>().playerJustJoined)
				{
					if (plyr.GetPhotonView().IsMine) LoseGame();
					else WinGame();

					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
				}
				else plyr.GetComponent<PlayerHealth>().playerJustJoined = false;
			}
		}
	}

	[PunRPC]
	void NewPlayerJoined()
	{
		foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
		{
			plyr.name = plyr.GetComponent<PhotonView>().Owner.NickName;
		}
	}

    [PunRPC]
    void DebugLog(string debug)
    {
        Debug.Log(debug);
    }

	[PunRPC]
	void AddPlayerCount()
	{
		CurPlayers++;
	}

	[PunRPC]
	void RemovePlayerCount()
	{
		CurPlayers--;
	}

	public void WinGame()
	{
		winGame.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

	public void LoseGame()
	{
		loseGame.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

	public void StartGame()
	{
		timerClock.starting = true;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			stream.SendNext(CurPlayers);
		}else if(stream.IsReading)
		{
			CurPlayers = (int)stream.ReceiveNext();
		}
	}
}
