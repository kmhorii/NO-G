using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    public InputField input;

    [SerializeField]
    private Text _roomName;
    private Text RoomName
    {
        get { return _roomName; }
    }

    public void OnClick_CreateRoom()
    {
        if (PhotonNetwork.NickName.Length == 0)
        {
            string[] Names = new string[6] { "Zach", "Matt", "David", "Ori", "Derek", "Chris" };
            PhotonNetwork.NickName = Names[Random.Range(0, 6)];
        }

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };

        if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
        {
            print("create room successfully sent.");
        }
        else
        {
            print("create room failed to send");
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("create room failed: " + codeAndMessage[1]);
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
        else if (!input.isFocused && cursor)
        {
            cursor = false;
            cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);
            input.text = cursorChar;
        }
    }
}
