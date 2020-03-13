using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public GameObject winner;

	public GameObject winGame;
	public GameObject loseGame;
	public GameObject tieGame;
	public Timer countdownTimer;
	public Timer gameTimer;

	public bool gameStarted = false;
	public bool countdownStarted = false;
    public bool gameOver = false;
	public GameObject[] spawnPoints;

	public List<GameObject> alivePlayers;
	public List<GameObject> allPlayers;

	// Start is called before the first frame update
	void Start()
    {
		alivePlayers = new List<GameObject>();
		allPlayers = new List<GameObject>();

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

        if (Input.GetButtonDown("Settings"))
        {
            if (!SceneManager.GetSceneByName("Settings").isLoaded)
            {
                SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                SceneManager.UnloadSceneAsync("Settings");
                if (!gameOver)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                
            }
        }
        if (!gameOver)
        {
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
            }
            else
            {
                if (gameTimer.isFinished && alivePlayers.Count > 1)
                {
					endGameTimerRunOut();
					//gameOver = true;
                }
                foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
                {
                    DeathCheck(plyr);
                }
            }
            
        }
        else
        {
            foreach(GameObject plyr in allPlayers)
            {
                if(plyr != null && plyr.GetPhotonView().IsMine)
                {
                    if(!tieGame.activeInHierarchy && !loseGame.activeInHierarchy)
                    {
                        WinGame();
                    }
                }
            }
        }
        /*else
        {
            if (gameStarted && gameTimer.isFinished)
            {
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    player.GetComponentInChildren<ShootingGun>().enabled = false;
                    player.GetComponent<PlayerMovement>().enabled = false;
                }

				endGameTimerRunOut();
                CheckForWinner();
                Debug.Log("The winner is: " + winner.name);
                if (winner.GetPhotonView().IsMine)
                {
                    WinGame();
                }
                else
                {
                    LoseGame();
                }

            }
        }*/
		
	}

	public void DeathCheck(GameObject plyr)
	{
		//Debug.Log(plyr.name + " is " + ((plyr.GetComponent<PlayerHealth>().isDead) ? "Dead" : "Not Dead"));
		if (plyr.GetComponent<PlayerHealth>().isDead)
		{
			plyr.GetComponent<PlayerHealth>().hasBeenDead = true;

			alivePlayers.Remove(plyr);
            if (alivePlayers.Count < 2 && !gameTimer.isFinished)
            {
                endGame();
            }

            Spectate(plyr);

			
		}
	}

	public void endGame()
	{
        foreach(GameObject player in alivePlayers)
        {
            DeathCheck(player);
        }
        if(alivePlayers.Count == 0)
        {
            foreach(GameObject player in allPlayers)
            {
                if (player.GetPhotonView().IsMine)
                {
                    if(loseGame.activeInHierarchy == false)
                    {
                        TieGame();
                    }
                    else
                    {
                        LoseGame();
                    }
                }
            }
        }
		foreach(GameObject player in allPlayers)
		{
			if(player.GetPhotonView().IsMine)
			{
				if (alivePlayers.Contains(player))
				{
					WinGame();
					player.GetComponent<StatsManager>().incrementWins();
				}
				else LoseGame();
			}

			gameOver = true;
		}
	}

	public void endGameTimerRunOut()
	{
        foreach(GameObject player in alivePlayers)
        {
            DeathCheck(player);
        }
		Dictionary<float, List<GameObject>> standings = new Dictionary<float, List<GameObject>>();

		foreach(GameObject player in alivePlayers)
		{
			float playerHealth = (player.GetComponent<PlayerHealth>().lives - 1) * 100 + player.GetComponent<PlayerHealth>().currentHealth;
			if(standings.ContainsKey(playerHealth)) standings[playerHealth].Add(player);
			else
			{
				standings.Add(playerHealth, new List<GameObject>());
				standings[playerHealth].Add(player);
			}
		}

		float largestHealth = standings.Keys.Max();/*MaxHealth(standings.Keys.ToList<float>())*/;
		if(standings[largestHealth].Count == 1)
		{
			foreach (GameObject player in alivePlayers)
			{
				if(player.GetPhotonView().IsMine)
				{
					if (standings[largestHealth].Contains(player))
					{
						WinGame();
					}
					else LoseGame();
				}
			}
		}
		else
		{
			foreach (GameObject player in alivePlayers)
			{
				if(player.GetPhotonView().IsMine)
				{
					if (standings[largestHealth].Contains(player)) TieGame();
					else LoseGame();
				}
			}
		}

		gameOver = true;
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
                child.gameObject.SetActive(false);
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
        player.GetComponent<StatsManager>().incrementWins();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
	}

	public void LoseGame()
	{
		loseGame.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; 
    }

	public void TieGame()
	{
		tieGame.SetActive(true);
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

		foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet"))
		{
			Destroy(bullet);
		}

		foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
		{
			alivePlayers.Add(plyr);
			allPlayers.Add(plyr);

			if(plyr.GetPhotonView().IsMine)
			{
				plyr.GetComponent<StatsManager>().incrementGames();

				plyr.GetComponent<PlayerHealth>().takeDamage = true;
                plyr.GetComponentInChildren<ShootingGun>().currentAmmo = plyr.GetComponentInChildren<ShootingGun>().maxAmmo;
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
