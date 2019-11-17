using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LoadBack : MonoBehaviourPun
{

    private void Update()
    {
        //if (Input.GetButtonDown("Cancel"))
        //{
        //    DisconnectGame();
        //}
    }
    public void DisconnectGame()
	{
		StartCoroutine(Disconnecting());
	}

	IEnumerator Disconnecting()
	{
		PhotonNetwork.Disconnect();
		while (PhotonNetwork.IsConnected)
			yield return null;
		SceneManager.LoadScene("Photon");
	}
}
