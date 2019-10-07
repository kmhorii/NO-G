using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmo : MonoBehaviour
{
    public float CurrentAmmo;
    public float MaxAmmo;

    public Slider ammobar;

    // Start is called before the first frame update
    void Start()
    {
        MaxAmmo = 9f;
        CurrentAmmo = MaxAmmo;

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
        CurrentAmmo--;
        ammobar.value = CalculateAmmo();
        Debug.Log(CurrentAmmo);
        if (CurrentAmmo <= 0)
            Reload();
    }

    float CalculateAmmo()
    {
        return CurrentAmmo / MaxAmmo;
    }

    void Reload()
    {
        Debug.Log("Reloading");

        CurrentAmmo = MaxAmmo;
        ammobar.value = CalculateAmmo();
        Debug.Log(CurrentAmmo);

    }
}
