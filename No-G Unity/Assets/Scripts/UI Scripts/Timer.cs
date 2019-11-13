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

    
    public Text timerText;

    // Start is called before the first frame update
    void Start()
    {
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

			timerText.text = ((int)timerLeft/60).ToString() + ": " + ((int)timerLeft % 60).ToString();
			if(timerLeft <= 0)
			{
				timerText.gameObject.SetActive(false);
				started = false;
				isFinished = true;
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
		}
		else if (stream.IsReading)
		{
			timerLeft = (float)stream.ReceiveNext();
		}
	}
}
