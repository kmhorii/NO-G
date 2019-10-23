using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIDisplay : MonoBehaviourPun, IPunObservable
{
    public PlayerHealth ph;
    public float maxHealth;
    public float currentHealth;
    public int lives;

    public Slider healthbar;
    public Text healthtext;

    ShootingGun gun;

    public float currentAmmo;
    public float maxAmmo;

    public bool isReloading;
    private float timeSpent;
    private float reloadFill;

    public GameObject winScreen;
    public GameObject loseScreen;
    public Text endText;

    public Slider ammobar;
    public Slider reloadbar;

    public Text ammotext;

    // Start is called before the first frame update
    void Start()
    {
        ph = GetComponent<PlayerHealth>();
        maxHealth = ph.maxHealth;
        currentHealth = ph.currentHealth;
        lives = ph.lives;

        gun = GetComponentInChildren<ShootingGun>();
        maxAmmo = gun.maxAmmo;
        currentAmmo = gun.currentAmmo;

        healthbar = GameObject.Find("Healthbar").GetComponent<Slider>();
        healthtext = GameObject.Find("HealthText").GetComponent<Text>();

        healthbar.value = CalculateHealth();
        healthtext.text = ConvertHealthFloattoString();

        ammobar = GameObject.Find("Ammobar").GetComponent<Slider>();
        reloadbar = GameObject.Find("Reloadbar").GetComponent<Slider>();
        isReloading = gun.isReloading;

        ammotext = GameObject.Find("AmmoText").GetComponent<Text>();

        winScreen = GameObject.Find("WinScreen");
        loseScreen = GameObject.Find("LoseScreen");
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (currentHealth != ph.currentHealth)
            {
                currentHealth = ph.currentHealth;
                healthbar.value = CalculateHealth();
                healthtext.text = ConvertHealthFloattoString();
            }
            if (currentAmmo != gun.currentAmmo)
            {
                currentAmmo = gun.currentAmmo;
                ammobar.value = CalculateAmmo();
                ammotext.text = ConvertAmmoFloattoString();
            }
            if (isReloading != gun.isReloading)
            {
                isReloading = gun.isReloading;
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
            if (ph.isDead)
            {
                healthtext.text = "Dead";

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
    float CalculateAmmo()
    {
        return currentAmmo / maxAmmo;
    }


    string ConvertAmmoFloattoString()
    {
        float convertammo = CalculateAmmo() * 10;
        return convertammo.ToString("f0");
    }

    //public void RespawnUI()
    //{
    //    currentHealth = maxHealth;
    //    healthbar.value = CalculateHealth();
    //    healthtext.text = ConvertHealthFloattoString();

    //    gun.currentAmmo = (int)maxAmmo;
    //    currentAmmo = gun.currentAmmo;
    //    ammobar.value = CalculateAmmo();
    //    ammotext.text = ConvertAmmoFloattoString();
    //}


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
