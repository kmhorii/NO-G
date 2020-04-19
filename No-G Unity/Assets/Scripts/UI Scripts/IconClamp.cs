using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconClamp : MonoBehaviour
{

    public GameObject playerIcon;

    // Start is called before the first frame update
    void Start()
    {
        playerIcon = GameObject.Find("PlayerIcon");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 iconPos = Camera.main.WorldToScreenPoint(this.transform.position);
        playerIcon.transform.position = iconPos;
    }
}
