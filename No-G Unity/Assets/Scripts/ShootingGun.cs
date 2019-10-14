using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=GttdLYKEJAM
//https://www.youtube.com/watch?v=kAx5g9V5bcM

public class ShootingGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform muzzle;

    public bool isShooting = false;
    public bool aiming = false;
    public bool isReloading = false;

    public int bulletSpeed;
    public float fireSpeed;

    public int maxAmmo = 9;
    public int currentAmmo;
    public float reloadTime = 1.5f;

    public int maxBounces = 4;
    public float maxStepDistance = 1000;

    public Material previewMaterial;

    public Vector3[] bouncePoints;
    public Vector3[] savedBounces;

    public LineRenderer lineRender;

    public GameObject savedLineRendererObject;
    public LineRenderer savedLineRender;

    public float savedPreviewTime = 1.5f;

    public Text ammoText;
    public Image ammoFill;

    void Start()
    {
        currentAmmo = maxAmmo;
        //UpdateAmmoText();

        lineRender = GetComponent<LineRenderer>();
        savedLineRender = savedLineRendererObject.GetComponent<LineRenderer>();

        lineRender.enabled = false;
        savedLineRender.enabled = false;
    }

    private void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aiming = true;
            lineRender.enabled = true;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            DrawPredictionShotLong(muzzle.position, muzzle.forward, maxBounces);

            for (int i = 0; i <= 4; i++)
            {
                GetComponent<LineRenderer>().SetPosition(i, bouncePoints[i]);
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            aiming = false;
            lineRender.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentAmmo == 0)
                Reload();
            else
                Shoot();
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        //UpdateAmmoText();

        isReloading = false;
    }

    private void Shoot()
    {
        if (!isShooting)
        {
            isShooting = true;

            if (Input.GetMouseButton(1))
            {
                SavePreview();
            }

            currentAmmo--;
            //UpdateAmmoText();

            GameObject bullet = Instantiate(bulletPrefab) as GameObject;

            bullet.transform.position = muzzle.transform.position;
            bullet.transform.rotation = muzzle.transform.rotation;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = muzzle.transform.forward * bulletSpeed;

            Invoke("FireDelay", fireSpeed);
        }
        else
        {
            Invoke("FireDelay", fireSpeed);
        }
    }

    private void FireDelay()
    {
        isShooting = false;
    }

    //private void UpdateAmmoText()
    //{
    //    ammoText.text = "Ammo: " + currentAmmo + "/" + maxAmmo;
    //    ammoFill.fillAmount = (float)currentAmmo / maxAmmo;
    //}

    private void SavePreview()
    {
        savedLineRender.enabled = true;
        
        savedBounces = bouncePoints;

        for (int i = 0; i <= 4; i++)
        {
            savedLineRender.GetComponent<LineRenderer>().SetPosition(i, savedBounces[i]);
        }

        Invoke("SavePreviewCooldown", savedPreviewTime);

        //savedLineRendererObject.SetActive(false);
    }

    private void SavePreviewCooldown()
    {
        savedLineRender.enabled = false;
        //ammoFill.
    }

    private void ReloadFillBar(float value)
    {
        value += (value < 1) ? 0.1f : 0f;

        if (value > 1) value = 1;
    }
    
    // raycasts from muzzle til it hits a wall

    // raycasts from muzzle till it hits a wall
    // creates new raycast from hit point, stores point in array
    // repeats til last bounce
    private void DrawPredictionShotLong(Vector3 position, Vector3 direction, int bouncesRemaining)
    {
        Vector3 startingPosition = position;
        Vector3 startingDirection = direction;

        bouncePoints[0] = position;

        Ray ray = new Ray(startingPosition, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }

        startingPosition = position;
        startingDirection = direction;
        bouncePoints[1] = position;

        ray = new Ray(startingPosition, direction);
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }

        startingPosition = position;
        bouncePoints[2] = position;

        ray = new Ray(startingPosition, direction);
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }

        startingPosition = position;
        bouncePoints[3] = position;

        ray = new Ray(startingPosition, direction);
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }

        bouncePoints[4] = position;
    }

    //private void DrawPredictionShot(Vector3 position, Vector3 direction, int bouncesRemaining)
    //{
    //    if (bouncesRemaining == 0)
    //    {
    //        return;
    //    }

    //    Vector3 startingPosition = position;

    //    Ray ray = new Ray(position, direction);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, maxStepDistance))
    //    {
    //        direction = Vector3.Reflect(direction, hit.normal);
    //        position = hit.point;
    //    }
    //    else
    //    {
    //        position += direction * maxStepDistance;
    //    }

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(startingPosition, position);
    //}
}
