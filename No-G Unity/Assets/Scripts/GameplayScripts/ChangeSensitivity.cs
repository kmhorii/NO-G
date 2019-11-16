using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChangeSensitivity : MonoBehaviour
{
    public Slider sensitivity;
    public Text sliderText;
    [SerializeField]
    float valueOffset;
    [SerializeField]
    float valueScale;

    float newSensitivity;

    PlayerMovement myControls;
    GameObject[] players;
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetPhotonView().IsMine)
            {
                myControls = player.GetComponent<PlayerMovement>();

            }
        }
        sensitivity.value = (myControls.rotateSpeed - valueOffset) / valueScale;
        if(valueOffset == 0)
        {
            valueOffset = 0.5f;
        }
        if(valueScale == 0)
        {
            valueScale = 4.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSliderChanged()
    {
        newSensitivity = sensitivity.value * valueScale + valueOffset;
        ChangeMouseSensitivity(newSensitivity);
        DisplayText();
    }
    public void ChangeMouseSensitivity(float rotateSpeed)
    {
        myControls.rotateSpeed = rotateSpeed;
    }
    public void DisplayText()
    {
        sliderText.text = ((int)(sensitivity.value * 100)).ToString();
    }
}
