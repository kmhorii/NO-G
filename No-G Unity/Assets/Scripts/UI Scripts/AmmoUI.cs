using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{

    public float fireRate = 15f;

    public int maxAmmo = 10;
    private int currentAmmo;

    public float reloadTime = 1f;

    private float nextTimetoFire = 0f;


    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimetoFire)
        {
            nextTimetoFire = Time.time + 1f / fireRate;

        }
    }

   
}
