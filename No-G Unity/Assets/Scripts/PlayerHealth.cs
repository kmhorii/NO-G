using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun, IPunObservable
{
    public bool playerJustJoined = true;
    public bool isDead = false;

	public bool takeDamage = false;
    public float maxHealth = 100;
    public float currentHealth = 100;
    public int lives = 3;

    public ShootingGun gun;
    public PlayerMovement player;

    public AudioSource impactSound;

	public UIDisplay display;

    // Start is called before the first frame update
    void Start()
    {
        if (maxHealth == 0)
        {
            maxHealth = 100f;
        }
        currentHealth = maxHealth;

        impactSound = GetComponent<AudioSource>();

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
        if (photonView.IsMine)
        {
            photonView.RPC("Damage", RpcTarget.All, shooter, damagevalue);
        }
    }

    [PunRPC]
    void Damage(string shooter, float damageValue)
    {
		//if (takeDamage)
		{
			//Minus player health w/ damage value
			currentHealth -= damageValue;
			impactSound.Play();
			if (currentHealth <= 0)
			{
				KillFeed(shooter, this.gameObject.name, true);
				currentHealth = 0;
				lives--;
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
			else KillFeed(shooter, this.gameObject.name, false);
		}
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
        gun.RespawnGun();
        //grab a gun and call ammo respawn
        //optional reset location
        player.RespawnPosition();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else if (stream.IsReading)
        {
            currentHealth = (float)stream.ReceiveNext();
        }
    }
}
