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
	int CurPlayersPlaying;

	public GameObject winGame;
	public GameObject loseGame;
	public Timer countdownTimer;
	public Timer gameTimer;

	public bool gameStarted = false;
	public bool countdownStarted = false;
	public GameObject[] spawnPoints;

	public List<GameObject> alivePlayers;
	// Start is called before the first frame update
	void Start()
    {
		alivePlayers = new List<GameObject>();

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
		CurPlayersPlaying = GameObject.FindGameObjectsWithTag("Player").Length;

		foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (plyr.name.ToLower().Contains("player(clone)")) plyr.name = plyr.GetComponent<PhotonView>().Owner.NickName;
		}

		if (!gameStarted)
		{
			if (!countdownStarted)
			{
				if (CurPlayers >= 2) StartCountdown();
			}
			else
			{
				if (CurPlayers < 2) StopCountdown();
				else if (countdownTimer.isFinished)
				{
					StartGame();
				}
			}
		}else
		{
			Debug.Log("Checking Death");
			foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
			{
				DeathCheck(plyr);
			}
		}
	}

	public void DeathCheck(GameObject plyr)
	{
		Debug.Log(plyr.name + " is " + ((plyr.GetComponent<PlayerHealth>().isDead) ? "Dead" : "Not Dead"));
		if (plyr.GetComponent<PlayerHealth>().isDead)
		{
				plyr.GetComponent<PlayerHealth>().hasBeenDead = true;

				alivePlayers.Remove(plyr);

				Spectate(plyr);

				if (alivePlayers.Count >= 2)
				{
					if (plyr.GetPhotonView().IsMine)
					{
					}
				}
				else
				{
					//this.GetComponent<PlayerMovement>().enabled = false;
					//this.GetComponent<PlayerHealth>().enabled = false;

					if (plyr.GetPhotonView().IsMine) LoseGame();
					else WinGame();
				}

				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
		}
	}

	public void Spectate(GameObject plyr)
	{
		plyr.GetComponent<PlayerHealth>().enabled = false;
		plyr.GetComponent<MeshRenderer>().enabled = false;

		plyr.gameObject.layer = 14;
		plyr.gameObject.tag = "Spectator";

		CurPlayersPlaying = GameObject.FindGameObjectsWithTag("Player").Length;

		foreach (Transform child in plyr.transform)
		{
            if (child.gameObject.tag == "Gun")
            {
                child.gameObject.GetComponent<ShootingGun>().enabled = false;
                child.gameObject.GetComponent<MeshRenderer>().enabled = false;
				child.gameObject.GetComponent<MeshCollider>().enabled = false;

				foreach(Transform gunChild in child)
				{
					if(gunChild.GetComponent<MeshRenderer>() != null) gunChild.gameObject.GetComponent<MeshRenderer>().enabled = false;
				}
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

	public void StopCountdown()
	{
		countdownStarted = false;
		countdownTimer.ResetClock();
	}

    public void StartCountdown()
	{
		countdownStarted = true;
		countdownTimer.StartClock();
	}

	public void StartGame()
	{
		gameStarted = true;

		foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
		{
			alivePlayers.Add(plyr);

			if(plyr.GetPhotonView().IsMine)
			{
				plyr.GetComponent<PlayerHealth>().takeDamage = true;
				plyr.transform.position = spawnPoints[plyr.GetComponent<PhotonView>().Owner.ActorNumber - 1].transform.position;
			}
		}

		gameTimer.StartClock();
		PhotonNetwork.CurrentRoom.IsOpen = false;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			stream.SendNext(CurPlayers);
			stream.SendNext(gameStarted);
			stream.SendNext(countdownStarted);

			stream.SendNext(JsonUtility.ToJson(alivePlayers));
		}else if(stream.IsReading)
		{
			CurPlayers = (int)stream.ReceiveNext();
			gameStarted = (bool)stream.ReceiveNext();
			countdownStarted = (bool)stream.ReceiveNext();

			JsonUtility.FromJson<List<GameObject>>((string)stream.ReceiveNext());
		}
	}
}
