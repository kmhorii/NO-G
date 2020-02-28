using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class UIDisplay : MonoBehaviourPun, IPunObservable
{
    public PlayerHealth ph;
    public float maxHealth;
    public float currentHealth;
    public int lives;

    public GameObject life1;
    public GameObject life2;
    public GameObject life3;

    public GameObject bullet1;
    public GameObject bullet2;
    public GameObject bullet3;

    public GameObject[] damageFeed;

    public float enemyHealth;

    public Slider healthbar;
    public Text healthtext;

    public GameObject enemyObject;
    public Slider enemyHealthbar;
    public Text enemyHealthtext;

    ShootingGun gun;

    public float currentAmmo;
    public float maxAmmo;

    public bool isReloading;
    private float timeSpent;
    private float reloadFill;

    public GameObject winScreen;
    public GameObject loseScreen;
    public Text endText;

    //public Slider ammobar;
    public Slider reloadbar;
    public Text ammotext;
    public Camera myCam;

    /*
    //Texture for Damage UI
    public Texture health100;
    public Texture health66;
    public Texture health32;
    public Texture health0;
    */

   // public Image damageUI;
    public Image crackedUI;
    public Image crackedUI2;

    public Image speedUI;
    public Image flashUI;
    public float flashAlphaDefault = 0.5f;
    public float currentFlashAlpha;
    public bool flashOn = false;

  ////  public Color startColor = new Color32(255, 0, 0, 255);
    public Color endColor = new Color32(0, 0, 0, 0);

    public float fadeSpeed = 2f;

    public GameObject shakeUI;

    public AudioSource thudsound;

    public bool isShaking = false;

    // Start is called before the first frame update
    void Start()
    {
        ph = GetComponent<PlayerHealth>();
        maxHealth = ph.maxHealth;
        currentHealth = ph.currentHealth;
        lives = ph.lives;

        life1 = GameObject.Find("Life 1");
        life2 = GameObject.Find("Life 2");
        life3 = GameObject.Find("Life 3");

        bullet1 = GameObject.Find("bullet1Full");
        bullet2 = GameObject.Find("bullet2Full");
        bullet3 = GameObject.Find("bullet3Full");

        damageFeed = new GameObject[3];

		for(int i = 0; i < 3; i++)
		{
			damageFeed[i] = GameObject.Find("Kill_" + (i + 1));
		}
        gun = GetComponentInChildren<ShootingGun>();
        maxAmmo = gun.maxAmmo;
        currentAmmo = gun.currentAmmo;

        healthbar = GameObject.Find("Healthbar").GetComponent<Slider>();
        healthtext = GameObject.Find("HealthText").GetComponent<Text>();

        enemyHealthbar = GameObject.Find("Healthbar_Enemy").GetComponent<Slider>();
        enemyHealthtext = GameObject.Find("HealthText_Enemy").GetComponent<Text>();

        healthbar.value = CalculateHealth();
        healthtext.text = ConvertHealthFloattoString();

        //ammobar = GameObject.Find("Ammobar").GetComponent<Slider>();
        reloadbar = GameObject.Find("Reloadbar").GetComponent<Slider>();
        isReloading = gun.isReloading;

        ammotext = GameObject.Find("AmmoText").GetComponent<Text>();

        winScreen = GameObject.Find("WinScreen");
        loseScreen = GameObject.Find("LoseScreen");

        //
        myCam = GameObject.Find("Main Camera").GetComponent<Camera>();

        shakeUI = GameObject.Find("ShakeObjects");

 
       

        reloadbar.gameObject.SetActive(false);

        enemyHealthbar.gameObject.SetActive(false);
        //DamageUI Color
        //damageUI = GameObject.Find("DamageUI").GetComponent<Image>();
        crackedUI = GameObject.Find("Cracked1UI").GetComponent<Image>();
        Debug.Log("find cracked 1");
        crackedUI2 = GameObject.Find("Cracked2UI").GetComponent<Image>();
        Debug.Log("find cracked 2");
        crackedUI.gameObject.SetActive(false);
        Debug.Log("false cracked 1");
        crackedUI2.gameObject.SetActive(false);
        Debug.Log("false cracked 2");
        //damageUI.GetComponent<Image>().color = new Color32(194, 194, 194, 0);
        //SpeedUI
        //speedUI = GameObject.Find("SpeedUI").GetComponent<Image>();
        //speedUI.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        //DamageFlash
        flashUI = GameObject.Find("DamageFlash").GetComponent<Image>();
        flashUI.enabled = false;
        //flashUI.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        currentFlashAlpha = flashAlphaDefault;

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (lives != ph.lives)
            {
                lives = ph.lives;
                DisplayLives();
            }
            if (currentHealth != ph.currentHealth)
            {
                currentHealth = ph.currentHealth;
                healthbar.value = CalculateHealth();
                healthtext.text = ConvertHealthFloattoString();
            }
            if (currentAmmo != gun.currentAmmo)
            {
                currentAmmo = gun.currentAmmo;
                //ammobar.value = CalculateAmmo();
                ammotext.text = ConvertAmmoFloattoString();
            }
            if (isReloading != gun.isReloading)
            {
                isReloading = gun.isReloading;
            }
            if (isReloading)
            {
                reloadbar.gameObject.SetActive(false);
                if (timeSpent < gun.reloadTime || currentAmmo == 0)
                {
                    timeSpent += Time.deltaTime;
                    reloadFill += (1f / gun.reloadTime) * Time.deltaTime;
                    reloadbar.value = reloadFill;
                    reloadbar.gameObject.SetActive(true);
                }
            }
            else
            {
                timeSpent = 0;
                reloadFill = 0;
                reloadbar.value = reloadFill;
                reloadbar.gameObject.SetActive(false);
            }
            if (ph.isDead)
            {
                healthtext.text = "Dead";

            }
            RayUI();

            if (enemyObject != null)
            {
                enemyHealth = enemyObject.GetComponent<PlayerHealth>().currentHealth;
                enemyHealthbar.value = enemyHealth / maxHealth;
                if (enemyHealthtext.text != enemyObject.GetPhotonView().name)
                {
                    enemyHealthtext.text = enemyObject.GetPhotonView().name;
                }
            }
            //if (Input.GetKeyDown(KeyCode.R) && !flashOn)
            //{
            //    flashOn = true;
            //}
            if (flashOn)
            {
                DamageFlashUI(currentFlashAlpha);
            }
            else if(currentFlashAlpha == 0)
            {
                currentFlashAlpha = flashAlphaDefault;
            }
        }
        else
        {
         
        }
        ////
        //BloodUI();
        
    }

	public void UpdateKillFeed(string shooter, string defender, bool kill)
	{
		photonView.RPC("KillFeedUpdate", RpcTarget.All, shooter, defender, kill);
	}

	[PunRPC]
	void KillFeedUpdate(string shooter, string defender, bool kill)
	{
		for (int i = 0; i < 2; i++)
		{
			damageFeed[i].GetComponent<Text>().text = damageFeed[i + 1].GetComponent<Text>().text;
			damageFeed[i].GetComponent<Text>().color = damageFeed[i + 1].GetComponent<Text>().color;
		}

		if (kill) damageFeed[2].GetComponent<Text>().color = new Color(255, 0, 0, 255);
		else damageFeed[2].GetComponent<Text>().color = new Color(89, 235, 245, 255);

		damageFeed[2].GetComponent<Text>().text = shooter + ((kill) ? " killed " : " shot ") + defender;

		if (shooter != defender && kill)
        {
			if (GameObject.Find(defender).GetPhotonView().IsMine)
			{
				GetComponent<StatsManager>().incrementDeaths();
			}

            if (GameObject.Find(shooter).GetPhotonView().IsMine)
            {
				GetComponent<StatsManager>().incrementKills();
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
        if (currentAmmo == 0)
        {
            bullet1.SetActive(false);
            bullet2.SetActive(false);
            bullet3.SetActive(false);
        }
        else if (currentAmmo == 1)
        {
            bullet1.SetActive(true);
            bullet2.SetActive(false);
            bullet3.SetActive(false);
        }
        else if (currentAmmo == 2)
        {
            bullet1.SetActive(true);
            bullet2.SetActive(true);
            bullet3.SetActive(false);
        }
        else if (currentAmmo == 3)
        {
            bullet1.SetActive(true);
            bullet2.SetActive(true);
            bullet3.SetActive(true);
        }

        return currentAmmo / maxAmmo;
    }


    string ConvertAmmoFloattoString()
    {
        float convertammo = CalculateAmmo() * maxAmmo;
        return convertammo.ToString("f0");
    }

    void DisplayLives()
    {
        switch (lives)
        {
            case 0:
                life1.SetActive(false);
                life2.SetActive(false);
                life3.SetActive(false);
                break;
            case 1:
                life2.SetActive(false);
                life3.SetActive(false);
                break;
            case 2:
                life3.SetActive(false);
                break;
            case 3:
                life1.SetActive(true);
                life2.SetActive(true);
                life3.SetActive(true);
                break;
            default:
                life1.SetActive(false);
                life2.SetActive(false);
                life3.SetActive(false);
                break;
        }
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

    void RayUI()
    {
        //Raycast For UI

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       // Rayc
        RaycastHit hit;

        //Vector3 startingPosition;
        //Vector3 fwd = transform.TransformDirection(Vector3.forward);
        // Ray ray = new Ray(startingPosition, direction);


        if (Physics.Raycast(myCam.transform.position, myCam.transform.forward, out hit))
        {
            Debug.DrawLine(ray.origin,hit.point);

            if(hit.collider.tag == "Player" && hit.collider.gameObject.name != this.name)
            {
                enemyObject = hit.collider.gameObject;
                enemyHealthbar.gameObject.SetActive(true);
                enemyHealthtext.gameObject.SetActive(true);
            }
            else
            {
                enemyObject = null;
                enemyHealthbar.gameObject.SetActive(false);
                enemyHealthtext.gameObject.SetActive(false);
            }
        
          // Debug.Log(hit.transform.gameObject.name);
         //  Debug.Log(hit.transform.gameObject.tag);
         }
        
    }

    
    public void CrackedUI()
    {
        if (ph.currentHealth == 100)
        {
            // Debug.Log("100");
            // damageUI.color = new Color32(194, 194, 194, 0);
            // GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), health100);
            crackedUI.gameObject.SetActive(false);
            crackedUI2.gameObject.SetActive(false);
            Debug.Log("100");
        }
        if(ph.currentHealth < 100 && ph.currentHealth > 50)
        {
            // Debug.Log("66")
            //damageUI.color = new Color32(194, 194, 194, 50);
            // GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), health66);
            crackedUI.gameObject.SetActive(true);
            crackedUI2.gameObject.SetActive(false);
            Debug.Log("less 100");
        }
        if(ph.currentHealth < 50 && ph.currentHealth > 0)
        {
            // Debug.Log("32");
            //damageUI.color = new Color32(194, 194, 194, 100);
            // GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), health32);
            crackedUI.gameObject.SetActive(false);
            crackedUI2.gameObject.SetActive(true);
            Debug.Log("less 50");
        }

    
    }
    /*
    void SpeedUI()
    {
        if()
        {
            Debug.Log("Zoom Zoom");

            speedUI.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
       
    }
    */
    public void DamageFlashUI(float alphaValue)
    {
        if(alphaValue > 0)
        {
            if (!flashUI.enabled)
            {
                Debug.Log("Activating flash");
                flashUI.enabled = true;
            }

            Color temp = flashUI.color;
            temp.a = alphaValue;
            flashUI.color = temp;

            
            //Debug.Log("fade");
            temp.a -= Time.deltaTime;
            if(temp.a < 0)
            {
                temp.a = 0;
                flashUI.enabled = false;
                flashOn = false;
            }
            flashUI.color = temp;
            currentFlashAlpha = temp.a;
            
            
            //else
            //{
            //    temp.a = flashAlphaDefault;
            //    flashUI.color = temp;
            //    flashUI.enabled = false;
            //    Debug.Log("no longer fading");
            //    return;
            //}
        }
        else
        {
            currentFlashAlpha = flashAlphaDefault;
            flashUI.enabled = false;
            flashOn = false;
            return;
        }
        //Debug.Log("Flash");
        
     
        
       
    }

    void DisableFlash()
    {
        flashUI.gameObject.SetActive(false);
    }

    /*
    void Fade()
    {
        if (flashUI.color.a > 0)
        {
            temp.a -= Time.deltaTime / 2;
            flashUI.color = temp;
        }
    }
    */
    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "NotBouncyWall") && (isShaking == false))
            {
                shakeUI.transform.position = new Vector3(shakeUI.transform.position.x, shakeUI.transform.position.y - 12f, shakeUI.transform.position.z);
                thudsound.Play();
                isShaking = true;
                //shakeUI.transform.localPosition = new Vector3(0, -10, 0);
                Invoke("ShakeBack", .2f);
            }
        
    }


    void ShakeBack()
    {
        shakeUI.transform.position = new Vector3(shakeUI.transform.position.x, shakeUI.transform.position.y + 12f, shakeUI.transform.position.z);
        isShaking = false;
        //shakeUI.transform.localPosition = new Vector3(0, 10, 0);
    }
}
