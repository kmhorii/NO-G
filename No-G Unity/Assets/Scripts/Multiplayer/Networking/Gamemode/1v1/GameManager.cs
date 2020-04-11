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
	int currentPlayersNumber;
	int CurPlayersPlaying;

    public GameObject winner;
    private float highestScore;
    private bool isTie;

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
	public List<GameObject> currentPlayers;
    public List<GameObject> tiedPlayers;

	// Start is called before the first frame update
	void Start()
    {
		alivePlayers = new List<GameObject>();
		currentPlayers = new List<GameObject>();
        tiedPlayers = new List<GameObject>();

		currentPlayersNumber = PhotonNetwork.CurrentRoom.PlayerCount;
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
        CheckPlayerCount();
		//CurPlayersPlaying = GameObject.FindGameObjectsWithTag("Player").Length;

		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (player.name.ToLower().Contains("player(clone)")) player.name = player.GetComponent<PhotonView>().Owner.NickName;
		}
        ToggleSettings();
        
        if (!gameOver)
        {
            if (!gameStarted)
            {
                if (!countdownStarted)
                {
                    if (currentPlayersNumber >= 2) StartCountdown();
                }
                else
                {
                    if (currentPlayersNumber < 2) StopCountdown();
                    else if (countdownTimer.isFinished)
                    {
                        StartGame();
                    }
                }
            }
            else
            {
                DeathCheck();
                if (gameTimer.isFinished || alivePlayers.Count == 1)
                {
                    EndGame();
					//endGameTimerRunOut();
					//gameOver = true;
                }
            }
            
        }
        else
        {
            //foreach(GameObject plyr in currentPlayers)
            //{
            //    if(plyr != null && plyr.GetPhotonView().IsMine)
            //    {
            //        if(!tieGame.activeInHierarchy && !loseGame.activeInHierarchy)
            //        {
            //            WinGame();
            //        }
            //    }
            //}
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

    private void DeathCheck()
    {
        foreach(GameObject player in alivePlayers)
        {
            if (player.GetComponent<PlayerHealth>().isDead)
            {
                //Do we need this?
                player.GetComponent<PlayerHealth>().hasBeenDead = true;
                alivePlayers.Remove(player);
                if (player.GetPhotonView().IsMine)
                {
                    LoseGame();
                }
                Spectate(player);
                Debug.Log("Player " + player.name + " died");
                
            }
        }
    }
	//public void DeathCheck(GameObject plyr)
	//{
	//	//Debug.Log(plyr.name + " is " + ((plyr.GetComponent<PlayerHealth>().isDead) ? "Dead" : "Not Dead"));
	//	if (plyr.GetComponent<PlayerHealth>().isDead)
	//	{
	//		plyr.GetComponent<PlayerHealth>().hasBeenDead = true;

	//		alivePlayers.Remove(plyr);
 //           if (alivePlayers.Count < 2 && !gameTimer.isFinished)
 //           {
 //               endGame();
 //           }

 //           Spectate(plyr);

			
	//	}
	//}
    public void CheckPlayerCount()
    {
        if(currentPlayersNumber != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            currentPlayersNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            currentPlayers.Clear();
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                currentPlayers.Add(player);
            }
            alivePlayers.Clear();
            foreach (GameObject player in currentPlayers)
            {
                if (!player.GetComponent<PlayerHealth>().isDead)
                {
                    alivePlayers.Add(player);
                }
            }
        }
    }

    private void EndGame()
    {
        if(alivePlayers.Count == 1 /*&& currentPlayers.Count > 2*/)
        {
            if (alivePlayers[0].GetPhotonView().IsMine)
            {
                WinGame();
            }
            else if (!loseGame.activeInHierarchy)
            {
                LoseGame();
            }
        }
        else if (alivePlayers.Count == 0)
        {
            if(currentPlayersNumber > 0)
            {
                foreach(GameObject player in currentPlayers)
                {
                    if(!loseGame.activeInHierarchy && !winGame.activeInHierarchy && player.GetPhotonView().IsMine)
                    {
                        TieGame();
                    }
                }
            }
        }
        else
        {
            //Method makes scores and declares a winner
            CalculateWinner();
        }
        gameOver = true;
    }
    private void CalculateWinner()
    {
        foreach(GameObject player in alivePlayers)
        {
            player.GetComponent<PlayerHealth>().score += 100 * player.GetComponent<PlayerHealth>().lives;
            player.GetComponent<PlayerHealth>().score += player.GetComponent<PlayerHealth>().currentHealth;
            //player.GetComponent<PlayerHealth>().score += 0.5 * player.blahblahblah.shotslanded;
        }
        SortListByScore();
        CheckForTies();
        if(!isTie && winner.GetPhotonView().IsMine)
        {
            WinGame();
        }
        else if(!isTie && !winner.GetPhotonView().IsMine)
        {
            if (!loseGame.activeInHierarchy)
            {
                LoseGame();
            }
        }
        else if (isTie)
        {
            Debug.Log("Tie");
            foreach(GameObject player in tiedPlayers)
            {
                if (player.GetPhotonView().IsMine)
                {
                    TieGame();
                }
            }
            foreach(GameObject player in currentPlayers)
            {
                if (player.GetPhotonView().IsMine && !loseGame.activeInHierarchy && !tieGame.activeInHierarchy)
                {
                    LoseGame();
                }
            }
        }
    }
    private void SortListByScore()
    {
        int swapCount = 0;
        for(int i = 0; i < alivePlayers.Count -1; i++)
        {
            if(alivePlayers[i].GetComponent<PlayerHealth>().score > alivePlayers[i+1].GetComponent<PlayerHealth>().score)
            {
                GameObject tempHolder = alivePlayers[i + 1];
                alivePlayers[i + 1] = alivePlayers[i];
                alivePlayers[i] = tempHolder;
                swapCount++;
            }
        }
        if(swapCount != 0)
        {
            SortListByScore();
        }
        else
        {
            highestScore = alivePlayers[alivePlayers.Count - 1].GetComponent<PlayerHealth>().score;
        }
    }
    private void CheckForTies()
    {
        for(int i = alivePlayers.Count - 2; i >= 0; i--)
        {
            if(alivePlayers[i].GetComponent<PlayerHealth>().score == highestScore)
            {
                tiedPlayers.Add(alivePlayers[i]);
                isTie = true;
            }
        }
        if (isTie)
        {
            bool inTie = false;
            foreach (GameObject player in tiedPlayers)
            {
                if (player == alivePlayers[alivePlayers.Count - 1])
                {
                    inTie = true;
                }
            }
            if(!inTie)
                tiedPlayers.Add(alivePlayers[alivePlayers.Count - 1]);
        }
        else
        {
            winner = alivePlayers[alivePlayers.Count -1];
        }
    }
	//public void endGame()
	//{
 //       foreach(GameObject player in alivePlayers)
 //       {
 //           DeathCheck(player);
 //       }
 //       if(alivePlayers.Count == 0)
 //       {
 //           foreach(GameObject player in currentPlayers)
 //           {
 //               if (player.GetPhotonView().IsMine)
 //               {
 //                   if(loseGame.activeInHierarchy == false)
 //                   {
 //                       TieGame();
 //                   }
 //                   else
 //                   {
 //                       LoseGame();
 //                   }
 //               }
 //           }
 //       }
	//	foreach(GameObject player in currentPlayers)
	//	{
	//		if(player.GetPhotonView().IsMine)
	//		{
	//			if (alivePlayers.Contains(player))
	//			{
	//				WinGame();
	//				player.GetComponent<StatsManager>().incrementWins();
	//			}
	//			else LoseGame();
	//		}

	//		gameOver = true;
	//	}
	//}

	//public void endGameTimerRunOut()
	//{
 //       foreach(GameObject player in alivePlayers)
 //       {
 //           DeathCheck(player);
 //       }
	//	Dictionary<float, List<GameObject>> standings = new Dictionary<float, List<GameObject>>();

	//	foreach(GameObject player in alivePlayers)
	//	{
	//		float playerHealth = (player.GetComponent<PlayerHealth>().lives - 1) * 100 + player.GetComponent<PlayerHealth>().currentHealth;
	//		if(standings.ContainsKey(playerHealth)) standings[playerHealth].Add(player);
	//		else
	//		{
	//			standings.Add(playerHealth, new List<GameObject>());
	//			standings[playerHealth].Add(player);
	//		}
	//	}

	//	float largestHealth = standings.Keys.Max();/*MaxHealth(standings.Keys.ToList<float>())*/;
	//	if(standings[largestHealth].Count == 1)
	//	{
	//		foreach (GameObject player in alivePlayers)
	//		{
	//			if(player.GetPhotonView().IsMine)
	//			{
	//				if (standings[largestHealth].Contains(player))
	//				{
	//					WinGame();
	//				}
	//				else LoseGame();
	//			}
	//		}
	//	}
	//	else
	//	{
	//		foreach (GameObject player in alivePlayers)
	//		{
	//			if(player.GetPhotonView().IsMine)
	//			{
	//				if (standings[largestHealth].Contains(player)) TieGame();
	//				else LoseGame();
	//			}
	//		}
	//	}

	//	gameOver = true;
	//}

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
    //Turn Settings on/off
    private void ToggleSettings()
    {
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
		currentPlayersNumber++;
	}

	[PunRPC]
	void RemovePlayerCount()
	{
		currentPlayersNumber--;
	}

	public void WinGame()
	{
		winGame.SetActive(true);
        foreach(GameObject player in currentPlayers)
        {
            if (player.GetPhotonView().IsMine)
            {
                player.GetComponent<StatsManager>().incrementWins();

            }
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
	}

	public void LoseGame()
	{
		loseGame.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Apparently we don't have a stat for losses??
        //gameObject.GetComponent<StatsManager>().incrementLosses();
    }

	public void TieGame()
	{
		tieGame.SetActive(true);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
        //Apparently we don't have a stat for ties??
        //gameObject.GetComponent<StatsManager>().incrementTies();
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
			currentPlayers.Add(plyr);

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
			stream.SendNext(currentPlayersNumber);
			stream.SendNext(gameStarted);
			stream.SendNext(countdownStarted);

			stream.SendNext(JsonUtility.ToJson(alivePlayers));
		}else if(stream.IsReading)
		{
			currentPlayersNumber = (int)stream.ReceiveNext();
			gameStarted = (bool)stream.ReceiveNext();
			countdownStarted = (bool)stream.ReceiveNext();

			JsonUtility.FromJson<List<GameObject>>((string)stream.ReceiveNext());
		}
	}
}
