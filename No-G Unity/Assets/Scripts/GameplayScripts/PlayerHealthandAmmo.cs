using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;

public class PlayerHealthandAmmo : MonoBehaviourPun, IPunObservable
{
	private Canvas canvas;

	public bool playerJustJoined = true;
    public int killCount;

    public float maxHealth = 100;
    public float currentHealth = 100;

    public Slider healthbar;
    public Text healthtext;
	
    ShootingGun gun;

    public float currentAmmo;
    public float maxAmmo;

    private float timeSpent;
    private float reloadFill;

	public GameObject winScreen;
	public GameObject loseScreen;
	public Text endText;

    public bool isReloading = false;

    public Slider ammobar;
    public Slider reloadbar;

    public Text ammotext;

    public AudioSource impactSound;

    // Start is called before the first frame update
    void Start()
    {
        killCount = 0; 

		healthbar = GameObject.Find("Healthbar").GetComponent<Slider>();
		healthtext = GameObject.Find("HealthText").GetComponent<Text>();

		ammobar = GameObject.Find("Ammobar").GetComponent<Slider>();
		reloadbar = GameObject.Find("Reloadbar").GetComponent<Slider>();

		ammotext = GameObject.Find("AmmoText").GetComponent<Text>();

		winScreen = GameObject.Find("WinScreen");
		loseScreen = GameObject.Find("LoseScreen");

        impactSound = GetComponent<AudioSource>();

		if (maxHealth == 0)
        {
            maxHealth = 100f;
        }
        currentHealth = maxHealth;

        healthbar.value = CalculateHealth();
        healthtext.text = ConvertHealthFloattoString();

        gun = GetComponentInChildren<ShootingGun>();
        maxAmmo = gun.maxAmmo;
        currentAmmo = gun.currentAmmo;

        ammobar.value = CalculateAmmo();
        ammotext.text = ConvertAmmoFloattoString();

        isReloading = gun.isReloading;
        reloadbar.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
		if (photonView.IsMine)
		{
			healthbar.value = CalculateHealth();
			healthtext.text = ConvertHealthFloattoString();

			if (currentHealth <= 0) Die();

			if (ammotext == null)
			{
				healthbar = GameObject.Find("Healthbar").GetComponent<Slider>();
				healthtext = GameObject.Find("HealthText").GetComponent<Text>();

				ammobar = GameObject.Find("Ammobar").GetComponent<Slider>();
				reloadbar = GameObject.Find("Reloadbar").GetComponent<Slider>();

				ammotext = GameObject.Find("AmmoText").GetComponent<Text>();
			}

			isReloading = gun.isReloading;

			if (gun == null)
			{
				gun = GetComponentInChildren<ShootingGun>();
			}
			isReloading = gun.isReloading;
			if (currentAmmo != gun.currentAmmo)
			{
				currentAmmo = gun.currentAmmo;
				ammobar.value = CalculateAmmo();
				ammotext.text = ConvertAmmoFloattoString();
			}
			if (isReloading)
			{
				if (timeSpent < gun.reloadTime || currentAmmo == 0)
				{
					timeSpent += Time.deltaTime;
					reloadFill += (1f / gun.reloadTime) * Time.deltaTime;
					reloadbar.value = reloadFill;

				}
			}
			else
			{
				timeSpent = 0;
				reloadFill = 0;
				reloadbar.value = reloadFill;
			}
		}
    }   

    private float CalculateHealth()
    {
        return currentHealth / maxHealth;
    }

    private string ConvertHealthFloattoString()
    {
        float converthealth = CalculateHealth() * 100;
        return converthealth.ToString("f00");
    }

    public void DealDamage(float damagevalue)
    {
		if(photonView.IsMine)
		{
			photonView.RPC("Damage", RpcTarget.All, damagevalue);
		}
    }

	[PunRPC]
	void Damage(float damageValue)
	{
		//Minus player health w/ damage value
		currentHealth -= damageValue;
		impactSound.Play();
	}

    private void Die()
    {
        currentHealth = 0;
        Debug.Log("Die");
        healthtext.text = "Dead";

		/*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerHealthandAmmo>().killCount == 3)
            {
                break;
            }
        }*/

		//photonView.RPC("GameOver", RpcTarget.All);
        //Respawn();
        //SceneManager.LoadScene("MergeTest");
    }

	/*public void Win()
	{
		winScreen.SetActive(true);
	}

	public void Lose()
	{
		loseScreen.SetActive(true);
	}*/

    float CalculateAmmo()
    {
        return currentAmmo / maxAmmo;
    }


    string ConvertAmmoFloattoString()
    {
        float convertammo = CalculateAmmo() * 10;
        return convertammo.ToString("f0");
    }

    void Respawn()
    {
        currentHealth = maxHealth;
        healthbar.value = CalculateHealth();
        healthtext.text = ConvertHealthFloattoString();

        gun.currentAmmo = (int) maxAmmo;
        currentAmmo = gun.currentAmmo;
        ammobar.value = CalculateAmmo();
        ammotext.text = ConvertAmmoFloattoString();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if(player != this.gameObject)
            {
                player.GetComponent<PlayerHealthandAmmo>().killCount++;
            }
        }
        //optional reset location
    }

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			stream.SendNext(currentHealth);
		}else if(stream.IsReading)
		{
			currentHealth = (float)stream.ReceiveNext();
		}
	}
    
}
