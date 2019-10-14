using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI: MonoBehaviour
{
    public float CurrentAmmo;
    public float MaxAmmo;

    public float timeRemaining;
    public float timerMax;

    public bool isReloading = false;



    public Image Realod;

    public Slider ammobar;
    public Slider reloadbar;

    public Text ammotext;

    // Start is called before the first frame update
    void Start()
    {
        MaxAmmo = 9f;
        CurrentAmmo = MaxAmmo;

        timerMax = 2f;
        timeRemaining = timerMax;

        ammobar.value = CalculateAmmo();
        reloadbar.value = CalculateReload();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && isReloading == false && CurrentAmmo >= 1)
            Shoot();
        if (Input.GetKeyDown(KeyCode.R) && isReloading == false)
        {
            isReloading = true;
            //   Debug.Log("Start Reload");
            //ReloadTimer();
            Debug.Log("Start Reload");
            timeRemaining -= Time.deltaTime;
            reloadbar.value = CalculateReload();
            Invoke("Reload", 2f);
           // Debug.Log("End Reload");
        }
    }

    void Shoot()
    {
        CurrentAmmo--;
        ammobar.value = CalculateAmmo();
        ammotext.text = ConvertAmmoFloattoString();
       // Debug.Log(CurrentAmmo);

        if (CurrentAmmo <= 0)
        {
            isReloading = true;
           // Debug.Log("Start Reload");
            //ReloadTimer();
            Invoke("Reload", 2f);
           // Debug.Log("End Reload");
        }
    }



    float CalculateAmmo()
    {
        return CurrentAmmo / MaxAmmo;
    }


    string ConvertAmmoFloattoString()
    {
       float convertammo = CalculateAmmo() * 10;
       return convertammo.ToString("f0");
    }


    float CalculateReload()
    {
        return timeRemaining / timerMax;
    }

    void Reload()
    {
        isReloading = true;
      //  Debug.Log("Reloading");

        CurrentAmmo = MaxAmmo;
        ammobar.value = CalculateAmmo();
        ammotext.text = "Max";
        isReloading = false;
       // Debug.Log(CurrentAmmo);
       // Debug.Log("End Reload");
    }

    /*
    void ReloadTimer()
    {
      Debug.Log("Start Reload");
      timeRemaining -= Time.deltaTime;
      reloadbar.value = CalculateReload();
    }
    */
}
