using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour {
    private string LevelName = "Level 2";

    public GameObject[] LobbyCanvas;
    public GameObject[] Players;
    public bool[] Ready;

    public GameManager gm;
    public NetManager nm;
    //public SpawnManager sm;
    private PhotonView pv;
    // Use this for initialization
    void Start()
    {
        pv = PhotonView.Get(this);
        nm = GameObject.FindGameObjectWithTag("NetManager").GetComponent<NetManager>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        //sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();

        nm.Lobby = gameObject.transform;

        //sm.SpawnPlayer();
    }    
    /*
    public void StartGameButtonPressed()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log(gm.TotalPlayers);
            int count = 0;
            for(int i = 0; i < gm.TotalPlayers; ++i)
            {
                //Debug.Log(gm.LobbyReady[i] + ":" + sm.PlayerID);
                if (gm.LobbyReady[i])
                {
                    ++count;
                }
            }

            if (count == gm.TotalPlayers)
            {
                gm.StartGame(LevelName);
            }
        }  
    }
    */

    public void MapButtonPressed()
    {
        LobbyCanvas[0].SetActive(false);
        LobbyCanvas[1].SetActive(true);
    }

    public void LobbyButtonPressed()
    {
        LobbyCanvas[0].SetActive(true);
        LobbyCanvas[1].SetActive(false);
    }

    public void MapSelect(string MapName)
    {
        LevelName = MapName;
    }
}
