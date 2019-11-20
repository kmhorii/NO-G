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
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
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
                if (gameTimer.isFinished)
                {
                    gameOver = true;
                }
                foreach (GameObject plyr in GameObject.FindGameObjectsWithTag("Player"))
                {
                    DeathCheck(plyr);
                }
            }
            
        }
        else
        {
            if (gameStarted && gameTimer.isFinished)
            {
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    player.GetComponentInChildren<ShootingGun>().enabled = false;
                    player.GetComponent<PlayerMovement>().enabled = false;
                }

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
        }
		
	}

	public void DeathCheck(GameObject plyr)
	{
		//Debug.Log(plyr.name + " is " + ((plyr.GetComponent<PlayerHealth>().isDead) ? "Dead" : "Not Dead"));
		if (plyr.GetComponent<PlayerHealth>().isDead)
		{
			plyr.GetComponent<PlayerHealth>().hasBeenDead = true;

			alivePlayers.Remove(plyr);

			Spectate(plyr);

			if (alivePlayers.Count >= 2) { }
			else endGame();
		}
	}

	public void endGame()
	{
		foreach(GameObject player in allPlayers)
		{
			if(player.GetPhotonView().IsMine)
			{
				if (alivePlayers.Contains(player)) WinGame();
				else LoseGame();
			}

			gameOver = true;
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

    //Will use lexicographic order to determine winner:
    //Checks who has the most lives
    //If that's one person, they win
    //If there are ties, check who has the most health left
    //If that's one person, they win
    //If there's a tie there, maybe in the future we could then check who has the most kills or shots landed
    //Should maybe in the distant distant future implement a tie-breaker/"sudden death" round in the rare case that 
    //everything is a tie
    public void CheckForWinner()
    {
        int tiedForLives = 0;
        List<int> livesOnly = new List<int>();
        List<GameObject> playersOnly = new List<GameObject>();
        IOrderedEnumerable<KeyValuePair<GameObject, int>> sortedLives = SortByLives(GameObject.FindGameObjectsWithTag("Player"));
        foreach (KeyValuePair<GameObject, int> pair in sortedLives)
        {
            livesOnly.Add(pair.Value);
            playersOnly.Add(pair.Key);
        }
        for(int i = livesOnly.Count -1; i >= 1; i--)
        {
            Debug.Log("The current indices are: " + i + " " + (i-1));

            if (livesOnly[i] == livesOnly[i - 1])
            {
                tiedForLives++;
            }
        }
        if (tiedForLives == 0)
        {
            winner = playersOnly[playersOnly.Count - 1];
        }
        else
        {
            int tiedForHealth = 0;
            List<int> healthOnly = new List<int>();
            playersOnly = new List<GameObject>();
            IOrderedEnumerable<KeyValuePair<GameObject, int>> sortedHealth = SortByHealth(GameObject.FindGameObjectsWithTag("Player"));
            foreach (KeyValuePair<GameObject, int> pair in sortedHealth)
            {
                healthOnly.Add(pair.Value);
                playersOnly.Add(pair.Key);
            }
            for (int i = healthOnly.Count - 1; i >= 1; i--)
            {
                if (healthOnly[i] == healthOnly[i - 1])
                {
                    tiedForHealth++;
                }
            }
            if (tiedForHealth == 0)
            {
                winner = playersOnly[playersOnly.Count - 1];
            }
            else
            {
                Debug.Log("It's a tie");
            }
        }

    }
    private IOrderedEnumerable<KeyValuePair<GameObject, int>> SortByLives(GameObject[] players)
    {
        Dictionary<GameObject, int> remainingLives = new Dictionary<GameObject, int>();
        foreach(GameObject player in players)
        {
            remainingLives.Add(player, player.GetComponent<PlayerHealth>().lives);
        }

        IOrderedEnumerable<KeyValuePair<GameObject, int>> sortedByLives;
        sortedByLives = from pair in remainingLives
                      orderby pair.Value 
                      select pair;

        return sortedByLives;
    }
    private IOrderedEnumerable<KeyValuePair<GameObject, int>> SortByHealth(GameObject[] players)
    {
        Dictionary<GameObject, int> remainingHealth = new Dictionary<GameObject, int>();
        foreach (GameObject player in players)
        {
            remainingHealth.Add(player, (int)player.GetComponent<PlayerHealth>().currentHealth);
        }

        IOrderedEnumerable<KeyValuePair<GameObject, int>> sortedByHealth;
        sortedByHealth = from pair in remainingHealth
                      orderby pair.Value
                      select pair;

        return sortedByHealth;
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
