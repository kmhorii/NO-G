using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    //public Text timerText;
    private int startTime = 120;

    public int currentTime = 0;

    [SerializeField] Text timerText;


    // Start is called before the first frame update
    void Start()
    {
        currentTime = startTime;
        InvokeRepeating("CountdownTimer", 0,1);
        // startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {


        
       

        /*
        float t = Time.time - startTime;

        string minutes = ((int)(t) / 60).ToString();
        string seconds = (t % 60).ToString("f2");

        //timerText.text = seconds;

        timerText.text = minutes + ":" + seconds;
        */
    }

    void CountdownTimer()
    {
        currentTime -= 1;
        timerText.text = currentTime.ToString();
        if (currentTime <= 0)
        {
            currentTime = 0;
        }
    }
}
