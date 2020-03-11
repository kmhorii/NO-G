using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{

    public Player PhotonPlayer { get; private set; }

    [SerializeField]
    private Text _playerName;
    private Text PlayerName
    {
        get { return _playerName; }
    }

    [SerializeField]
    private Text _playerPing;
    private Text m_playerPing
    {
        get { return _playerPing; }
    }

    public void ApplyPhotonPlayer(Player photonPlayer)
    {
        PhotonPlayer = photonPlayer;
        PlayerName.text = photonPlayer.NickName;

        //StartCoroutine(C_ShowPing());
    }

    private IEnumerator C_ShowPing()
    {
        while (PhotonNetwork.IsConnected)
        {
            int ping = (int)PhotonPlayer.CustomProperties["Ping"];
            m_playerPing.text = ping.ToString();
            yield return new WaitForSeconds(1f);
        }

        yield break;
    }
}
