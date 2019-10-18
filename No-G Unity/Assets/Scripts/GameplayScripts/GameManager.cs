using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public bool gameOver;
    private GameObject[] players;
    private GameObject winner;

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Text winText;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        players = GameObject.FindGameObjectsWithTag("Player");
        if(panel == null)
        {
            panel = GameObject.FindGameObjectWithTag("GameOver");
        }
        panel.SetActive(false);
        Debug.Log(players.Length);
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);

        if (gameOver)
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<MeshRenderer>().enabled = false;
                player.GetComponentInChildren<ShootingGun>().enabled = false;
                player.layer = 14;

                if(player.GetComponent<PlayerHealthandAmmo>().killCount == 3)
                {
                    winner = player;
                }
                player.GetComponent<PlayerHealthandAmmo>().enabled = false;
            }
            panel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            winText.text = "The winner is " + winner;
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("Photon");
    }
}
