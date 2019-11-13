using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun, IPunObservable
{
	public float SpawnTime;
	float timer = 0;
	bool hasPlayerSpawned = false;
	int CurPlayers;

	public GameObject winGame;
	public GameObject loseGame;
	public Timer countdownTimer;
	public Timer gameTimer;

	public bool gameStarted = false;
	public GameObject[] spawnPoints;
	// Start is called before the first frame update
	void Start()
    {
		CurPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
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
		CurPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

		if (gameStarted)
		{
			foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
			{
				if (plyr.name.ToLower().Contains("player(clone)"))
                    plyr.name = plyr.GetComponent<PhotonView>().Owner.NickName;

				DeathCheck(plyr);
			}
		}
		else
		{
            if (countdownTimer.isFinished) StartGame();

            else if (CurPlayers >= 2) StartCountdown();
			else StopCountdown();

		}

	}

	public void DeathCheck(GameObject plyr)
	{
		if (plyr.GetComponent<PlayerHealth>().isDead)
		{
			if (GameObject.FindGameObjectsWithTag("Player").Length > 2)
			{
				if (plyr.GetPhotonView().IsMine)
				{
					Destroy(plyr);
					PhotonNetwork.Instantiate("Spectator", Vector3.zero, Quaternion.identity, 0);
				}
			}
			else
			{
				this.GetComponent<PlayerMovement>().enabled = false;
				this.GetComponent<PlayerHealth>().enabled = false;

				if (plyr.GetPhotonView().IsMine) LoseGame();
				else WinGame();

				Destroy(plyr);
			}

			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
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

	public void StopCountdown()
	{
		countdownTimer.ResetClock();
	}

    public void StartCountdown()
	{
		countdownTimer.StartClock();
	}

	public void StartGame()
	{
		gameStarted = true;
		int i = 0;
		foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
		{
			plyr.GetComponent<PlayerHealth>().takeDamage = true;

			if(plyr.GetPhotonView().IsMine)
			{
				plyr.transform.position = spawnPoints[i].transform.position;
			}
			i++;
		}

		gameTimer.StartClock();
		PhotonNetwork.CurrentRoom.IsOpen = false;
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
