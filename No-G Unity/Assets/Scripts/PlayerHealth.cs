using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun, IPunObservable
{
    public float score = 0;

    public bool playerJustJoined = true;
    public bool isDead = false;
	public bool hasBeenDead = false;

	public bool takeDamage = false;
    public float maxHealth = 100;
    public float currentHealth = 100;
    public int lives = 3;
    public int kills = 0;
    public int selfKills = 0;
    public int deaths = 0;

    public ShootingGun gun;
    public PlayerMovement player;

    public AudioSource impactSound;
    public AudioSource gruntSound;

    public GameObject hitParticles;

	public UIDisplay display;

    // Start is called before the first frame update
    void Start()
    {
        if (maxHealth == 0)
        {
            maxHealth = 100f;
        }
        currentHealth = maxHealth;

        //impactSound = GetComponent<AudioSource>();

        gun = GetComponentInChildren<ShootingGun>();
        player = GetComponent<PlayerMovement>();
		display = GetComponent<UIDisplay>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if(lives == 0)
            {
                Die();
            }
        }
    }

	void KillFeed(string attacker, string defender, bool death)
	{
		display.UpdateKillFeed(attacker, defender, death);
	}

	public void DealDamage(string shooter, float damagevalue)
    {
		if(takeDamage) photonView.RPC("Damage", RpcTarget.All, shooter, damagevalue);
    }

    [PunRPC]
    void Damage(string shooter, float damageValue)
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        Invoke("ChangeColorBack", 0.4f);
        if (photonView.IsMine)
		{
			//Minus player health w/ damage value
			currentHealth -= damageValue;

            gruntSound.Play();
			
			display.CrackedUI();
			display.flashOn = true;

			KillFeed(shooter, this.gameObject.name, (currentHealth <= 0));

			if (currentHealth <= 0)
			{
				currentHealth = 0;
				lives--;
				deaths++;
				if (lives <= 0)
				{
					lives = 0;
					Die();
				}
				else
				{
					Respawn();
				}
			}
		}
        else if(GameObject.Find(shooter).GetPhotonView().IsMine)
		{
            impactSound.Play();
            hitParticles.GetComponent<ParticleSystem>().Stop();
            hitParticles.GetComponent<ParticleSystem>().Play();

            Debug.Log("hit " + gameObject.name);
        }
	}

    private void ChangeColorBack()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
    }

    private void Die()
    {
        currentHealth = 0;
        isDead = true;
        Debug.Log("Die");
    }

    void Respawn()
    {
        currentHealth = maxHealth;
        display.CrackedUI();
        display.flashOn = false;
        //display.flashUI.enabled = false;
        display.currentFlashAlpha = display.flashAlphaDefault;
        gun.RespawnGun();
        //grab a gun and call ammo respawn
        //optional reset location
        player.RespawnPosition();

		GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
			stream.SendNext(takeDamage);
			stream.SendNext(isDead);

			stream.SendNext(hasBeenDead);
        }
        else if (stream.IsReading)
        {
            currentHealth = (float)stream.ReceiveNext();
			takeDamage = (bool)stream.ReceiveNext();
			isDead = (bool)stream.ReceiveNext();

			hasBeenDead = (bool)stream.ReceiveNext();
		}
    }
}
