using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<MeshRenderer>().enabled = false;
                player.GetComponentInChildren<ShootingGun>().enabled = false;
                player.GetComponentInChildren<Bullet>().enabled = false;
                player.layer = 14;

                if(player.GetComponent<PlayerHealthandAmmo>().killCount == 3)
                {
                    winner = player;
                }
                player.GetComponent<PlayerHealthandAmmo>().enabled = false;
            }
            panel.SetActive(true);
            winText.text = "The winner is " + winner;
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("Photon");
    }
}
