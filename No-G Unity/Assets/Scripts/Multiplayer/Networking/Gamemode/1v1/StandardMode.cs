using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class StandardMode : MonoBehaviourPun, IPunObservable
{
	public float SpawnTime;
	float timer = 0;
	bool hasPlayerSpawned = false;

	// Start is called before the first frame update
	void Start()
    {
		Debug.Log("Test");
    }

    // Update is called once per frame
    void Update()
    {
		timer += Time.deltaTime;
		if(timer >= SpawnTime)
		{
			if (!hasPlayerSpawned)
			{
				PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
				hasPlayerSpawned = true;
			}

			timer = 0;
		}
    }

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{

		}else if(stream.IsReading)
		{

		}
	}
}
