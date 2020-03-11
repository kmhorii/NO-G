using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public Text PlayerName;
    public GameObject ready;

    public GameObject[] CharacterSelection;
    public GameObject SelectedCharacter;

    public float CanChange = 0;
    public int SelectionIndex = 0;
    public bool Selected = false;

    //private SpawnManager sm;
    private GameManager gm;
    private PhotonView photonView;
	// Use this for initialization
    /*
	void Start ()
    {   
        photonView = PhotonView.Get(this);

        //sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        gameObject.transform.parent = GameObject.FindGameObjectWithTag("Lobby").transform;
        if (photonView.isMine)
        {
            photonView.RPC("UpdateName", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName);
            sm.PlayerID = PhotonNetwork.player.ID;
            if (sm.PlayerID == 1)
            {
                //Debug.Log("Player 1 spawned");
                gameObject.transform.localPosition = new Vector3(-250, 0, 0);
            }
            else if (sm.PlayerID == 2)
            {
                //Debug.Log("Player 2 spawned");
                gameObject.transform.localPosition = new Vector3(-125, 0, 0);
            }
            else if (sm.PlayerID == 3)
            {
                gameObject.transform.localPosition = new Vector3(125, 0, 0);
            }
            else if (sm.PlayerID == 4)
            {
                gameObject.transform.localPosition = new Vector3(250, 0, 0);
            }

            photonView.RPC("UpdateSprite", PhotonTargets.AllBuffered, 0, 0);
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }

        if (!Selected && Time.time > CanChange)
        {
            int PrevIndex;
                
            if (Input.GetAxisRaw("Bumper Input") == 1)
            {
                PrevIndex = SelectionIndex;

                CharacterSelection[SelectionIndex].SetActive(false);
                SelectionIndex += 1;
                if (SelectionIndex == CharacterSelection.Length)
                {
                    SelectionIndex = 0;
                }
                CharacterSelection[SelectionIndex].SetActive(true);

                photonView.RPC("UpdateSprite", PhotonTargets.AllBuffered, PrevIndex, SelectionIndex);

                CanChange = 0;
                CanChange = Time.time + 1;
            }
            else if (Input.GetAxisRaw("Bumper Input") == -1)
            {
                //Debug.Log(Input.GetAxisRaw("Bumper Input"));

                PrevIndex = SelectionIndex;

                CharacterSelection[SelectionIndex].SetActive(false);
                SelectionIndex -= 1;
                if (SelectionIndex == -1)
                {
                    SelectionIndex = CharacterSelection.Length - 1;
                }
                CharacterSelection[SelectionIndex].SetActive(true);

                photonView.RPC("UpdateSprite", PhotonTargets.AllBuffered, PrevIndex, SelectionIndex);

                CanChange = 0;
                CanChange = Time.time + 1;
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            photonView.RPC("UpdateSprite", PhotonTargets.AllBuffered, SelectionIndex, SelectionIndex);
            if (!Selected)
            {
                photonView.RPC("ReadyUp", PhotonTargets.AllBuffered, true);

                Selected = true;
                //Debug.Log("Player: " + sm.PlayerID + " is ready");
                gm.pv.RPC("ReadyPlayer", PhotonTargets.All, sm.PlayerID, true);                    

                sm.PlayerPick = CharacterSelection[SelectionIndex].name;
            }
            else
            {
                photonView.RPC("ReadyUp", PhotonTargets.AllBuffered, false);

                Selected = false;

                gm.pv.RPC("ReadyPlayer", PhotonTargets.All, sm.PlayerID, false);
            }
        }
    }

    [PunRPC]
    void ReadyUp(bool active)
    {
        ready.SetActive(active);
    }

    [PunRPC]
    void UpdateSprite(int prev, int cur)
    {
        gameObject.transform.GetChild(prev).gameObject.SetActive(false);
        gameObject.transform.GetChild(cur).gameObject.SetActive(true);
    }

    [PunRPC]
    void UpdateName(string name)
    {
        PlayerName.text = name;
    }
    */
}
