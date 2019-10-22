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

	// Start is called before the first frame update
	void Start()
    {
		GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
		player.name = PhotonNetwork.NickName;

		photonView.RPC("AddPlayerCount", RpcTarget.All);
	}

    // Update is called once per frame
    void Update()
    {
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
