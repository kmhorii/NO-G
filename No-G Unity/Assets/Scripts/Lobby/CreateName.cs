using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateName : MonoBehaviour
{
    public InputField input;

    public string Name;

    public void OnClick_CreateName()
    {
        if (cursor)
        {
            PhotonNetwork.NickName = input.text.Substring(0, cursorChar.Length - 1);
        }
        else
        {
            PhotonNetwork.NickName = input.text;
        }

        Name = PhotonNetwork.NickName;
    }

    public void NameUpdate(string name)
    {
        PhotonNetwork.NickName = name;
    }

    private float m_TimeStamp;
    private bool cursor = false;
    private string cursorChar = "";
    private int maxStringLength = 24;

    void Update()
    {
        if (input.isFocused && Time.time - m_TimeStamp >= 0.5)
        {            
            m_TimeStamp = Time.time;
            cursorChar = input.text;
            if (cursor == false)
            {
                cursor = true;
                cursorChar += "_";
            }
            else
            {
                cursor = false;                
                cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);
            }
            input.text = cursorChar;
        }
        else if(!input.isFocused && cursor)
        {
            cursor = false;
            cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);
            input.text = cursorChar;
        }
    }
}
