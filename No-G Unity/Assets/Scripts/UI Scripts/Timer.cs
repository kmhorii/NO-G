using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    //Round Timer
    //public Text timerText;
    private int startTime = 120;

	public bool starting = false;
	public bool started = false;
    public int currentTime = 0;

    
    [SerializeField] Text timerText, countdownText;

    //GameStart CountDown
    private int countdownTime = 10;
    public int currCDTime = 0;



    // Start is called before the first frame update
    void Start()
    {
        // startTime = Time.time;
        countdownText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
		if(!started)
		{
            if (starting)
            {
                started = true;
                currCDTime = countdownTime;
                InvokeRepeating("CountdownTimer", 0, 1);
                //countdownText.gameObject.SetActive(false);
            }
		}



		/*
        float t = Time.time - startTime;

        string minutes = ((int)(t) / 60).ToString();
        string seconds = (t % 60).ToString("f2");

        //timerText.text = seconds;

        timerText.text = minutes + ":" + seconds;
        */
	}

    void RoundTimer()
    {
        currentTime -= 1;
        timerText.text = currentTime.ToString();
        if (currentTime <= 0)
        {
            currentTime = 1;
        }
    }

    void CountdownTimer()
    {
        currCDTime -= 1;
        countdownText.text = currCDTime.ToString();
        if (currCDTime == -1)
        {
           countdownText.text = "START";
           countdownText.gameObject.SetActive(false);
           currentTime = startTime;
           InvokeRepeating("RoundTimer", 0, 1);
        }
    }

}
