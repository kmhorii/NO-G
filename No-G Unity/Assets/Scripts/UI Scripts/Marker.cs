using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
    public GameObject marker;
    public Transform target;
    public Text meter;
    public GameObject player;
    public Vector3 playerPos;
    public float offsetY;

    void Start()
    {
        player = GameObject.Find("Player");
      
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        marker.transform.position = playerPos + new Vector3(0, 5f, 0);
        /*
        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;
        
        float minY = img.GetPixelAdjustedRect().width / 2;
        float maxY = Screen.width - minY;

       

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);

        img.transform.position = Camera.main.WorldToScreenPoint(target.position);

        if(Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            //Target is behind the player
            if (pos.x < Screen.width/2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        img.transform.position = pos;
        meter.text = ((int)Vector3.Distance(target.position, transform.position)).ToString();
        */
    }
}
