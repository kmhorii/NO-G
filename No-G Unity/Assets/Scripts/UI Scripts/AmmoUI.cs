using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI: MonoBehaviour
{
    public float CurrentAmmo;
    public float MaxAmmo;

    public float reloadtimer;

    public bool isFiring = false;





    public Slider ammobar;
    public Slider cooldownbar;

    public Text ammotext;

    // Start is called before the first frame update
    void Start()
    {
        MaxAmmo = 9f;
        CurrentAmmo = MaxAmmo;

        reloadtimer = 5;

        ammobar.value = CalculateAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Shoot();
        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    void Shoot()
    {
        isFiring = true;
        CurrentAmmo--;
        ammobar.value = CalculateAmmo();
        ammotext.text = ConvertAmmoFloattoString();
        Debug.Log(CurrentAmmo);
        isFiring = false;
        if (CurrentAmmo <= 0)
            Reload();
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
        isFiring = false;
        Debug.Log("Reloading");

        CurrentAmmo = MaxAmmo;
        ammobar.value = CalculateAmmo();
        ammotext.text = "Max";
        Debug.Log(CurrentAmmo);

    }
}
