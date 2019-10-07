using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI: MonoBehaviour
{
    public float CurrentAmmo;
    public float MaxAmmo;

    public float reloadtimer;

    public bool isReloading = false;





    public Slider ammobar;
    public Slider cooldownbar;

    public Text ammotext;

    // Start is called before the first frame update
    void Start()
    {
        MaxAmmo = 9f;
        CurrentAmmo = MaxAmmo;

       // reloadtimer = 5;

        ammobar.value = CalculateAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && isReloading == false && CurrentAmmo >= 1)
            Shoot();
        if (Input.GetKeyDown(KeyCode.R) && isReloading == false)
        {
            isReloading = true;
            Debug.Log("Start Reload");
            Invoke("Reload", 2f);
           // Debug.Log("End Reload");
        }
    }

    void Shoot()
    {
        CurrentAmmo--;
        ammobar.value = CalculateAmmo();
        ammotext.text = ConvertAmmoFloattoString();
        Debug.Log(CurrentAmmo);

        if (CurrentAmmo <= 0)
        {
            isReloading = true;
            Debug.Log("Start Reload");
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

    void Reload()
    {
        isReloading = true;
        Debug.Log("Reloading");

        CurrentAmmo = MaxAmmo;
        ammobar.value = CalculateAmmo();
        ammotext.text = "Max";
        isReloading = false;
        Debug.Log(CurrentAmmo);
        Debug.Log("End Reload");
    }
}
