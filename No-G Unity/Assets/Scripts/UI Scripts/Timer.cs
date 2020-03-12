using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

public class Timer : MonoBehaviourPun, IPunObservable
{
    //Round Timer
    //public Text timerText;
    public float timeLength = 120;

	public bool starting = false;
	public bool started = false;
	public bool isFinished = false;

    public float timerLeft = 0;



    public GameObject eliminateText;


    public Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        eliminateText = GameObject.Find("EliminateText");
        eliminateText.gameObject.SetActive(true);
        // startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
		if(!started)
		{
            if (starting)
            {
                started = true;
                timerLeft = timeLength;

				timerText.gameObject.SetActive(true);
				starting = false;
            }
		}
		else
		{
			timerLeft -= Time.deltaTime;
            string newText = (int)timerLeft % 60 < 10 ?
                             ((int)timerLeft / 60).ToString() + ": 0" + ((int)timerLeft % 60).ToString():
                             ((int)timerLeft / 60).ToString() + ": " + ((int)timerLeft % 60).ToString();

            timerText.text = newText;
			if(timerLeft <= 0)
			{
				timerText.gameObject.SetActive(false);
				started = false;
				isFinished = true;
                eliminateText.gameObject.SetActive(false);
                Debug.Log("Text false");
			}
		}
	}

	public void StartClock()
	{
		starting = true;
	}

	public void ResetClock()
	{
		started = false;
		starting = false;

		isFinished = false;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(timerLeft);
			stream.SendNext(starting);
			stream.SendNext(started);
			stream.SendNext(isFinished);
}
		else if (stream.IsReading)
		{
			timerLeft = (float)stream.ReceiveNext();
			starting = (bool)stream.ReceiveNext();
			started = (bool)stream.ReceiveNext();
			isFinished = (bool)stream.ReceiveNext();
		}
	}
}
