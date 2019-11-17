using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

//https://www.youtube.com/watch?v=GttdLYKEJAM
//https://www.youtube.com/watch?v=kAx5g9V5bcM

public class ShootingGun : MonoBehaviourPun
{
    public GameObject bulletPrefab;
    public Transform muzzle;
	public GameObject mainCamera;

    public bool isShooting = false;
    public bool aiming = false;
    public bool isReloading = false;

    public int bulletSpeed;
    public float fireSpeed;

    public int maxAmmo = 3;
    public int currentAmmo;
    public float reloadTime = 3f;

    public int maxBounces = 8;
    public float maxStepDistance = 1000;

    public Material previewMaterial;
    public Material previewHitMaterial;

    public Vector3[] bouncePoints;
    public Vector3[] savedBounces;

    public LineRenderer lineRender;
    public LineRenderer savedLineRender;



    public float savedPreviewTime = 1.5f;

	public string shooter;
    void Start()
    {
        currentAmmo = maxAmmo;

        lineRender.enabled = false;
        savedLineRender.enabled = false;
    }

    //Tracking camera for Photon
    //Previous bug of not finding camera when loading in player
    private void Update()
    {
        if (photonView.IsMine)
        {
			gameObject.layer = 15;
			foreach(Transform child in transform) child.gameObject.layer = 15;
            transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
            transform.rotation = new Quaternion(mainCamera.transform.rotation.x, mainCamera.transform.rotation.y, mainCamera.transform.rotation.z, mainCamera.transform.rotation.w);
            if(currentAmmo < maxAmmo && !isReloading)
            {
                isReloading = true;
                Invoke("ChargeBullet", reloadTime);
            }
            else
            {
                if(currentAmmo > maxAmmo)
                {
                    currentAmmo = maxAmmo;
                }
            }
        }
    }

    //Preview Shot
	public void DownAiming()
	{
		aiming = true;
		lineRender.enabled = true;
        Debug.Log(lineRender.enabled);
    }

	public void Aiming()
	{
        lineRender.enabled = true;
		DrawPredictionShotLong(muzzle.position, muzzle.forward, 5);

		for (int i = 0; i <= 4; i++)
		{
			lineRender.SetPosition(i, bouncePoints[i]);
		}
	}

	public void DoneAiming()
	{
		aiming = false;
		lineRender.enabled = false;
	}

    //Reload
	/*public void Reloading()
	{
		StartCoroutine(Reload());
		return;
	}*/

    //Shooting
	public void Shooting()
	{
		//if (currentAmmo == 0)
		//	StartCoroutine(Reload());
		if (!isShooting && currentAmmo > 0)
		{
			Shoot();
			isShooting = true;

			if (Input.GetMouseButton(1) || Input.GetAxisRaw("Preview") == 1)
			{
				SavePreview();
			}
			currentAmmo--;
            isShooting = false;
            //Invoke("FireDelay", fireSpeed);
        }
		else
		{
            //Invoke("FireDelay", fireSpeed);
        }
    }

    /*private IEnumerator Reload()
    {
        if(currentAmmo < maxAmmo)
        {
            isReloading = true;

            yield return new WaitForSeconds(reloadTime);

            currentAmmo++;

        }
        else
        {
            isReloading = false;
            if(currentAmmo > maxAmmo)
            {
                currentAmmo = maxAmmo;
            }
        }
        
    }*/

    private void ChargeBullet()
    {
        if(currentAmmo < maxAmmo)
        {
            currentAmmo++;
            isReloading = false;
        }
    }

    //Photon Shooting
    private void Shoot()
    {
		photonView.RPC("RPCShoot", RpcTarget.All, PlayerInfo.Name);
    }


    //Spawning Bullet for Photon
	[PunRPC]
	public void RPCShoot(string shooter)
	{
		GameObject bullet = Instantiate(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation);

		Rigidbody rb = bullet.GetComponent<Rigidbody>();
		rb.velocity = muzzle.transform.forward * bulletSpeed;

		bullet.GetComponent<Bullet>().shooter = shooter;
	}

    private void FireDelay()
    {
        isShooting = false;
    }

    private IEnumerator FireDelayTwo()
    {
        yield return new WaitForSeconds(fireSpeed);

        isShooting = false;
    }

    private void SavePreview()
    {
        savedLineRender.enabled = true;
        
        savedBounces = bouncePoints;

        for (int i = 0; i <= 4; i++)
        {
            savedLineRender.GetComponent<LineRenderer>().SetPosition(i, savedBounces[i]);
        }

        Invoke("SavePreviewCooldown", savedPreviewTime);
    }

    private void SavePreviewCooldown()
    {
        savedLineRender.enabled = false;
    }

    private void ReloadFillBar(float value)
    {
        value += (value < 1) ? 0.1f : 0f;

        if (value > 1) value = 1;
    }

    public void RespawnGun()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
    }

	// raycasts from muzzle till it hits a wall
	// creates new raycast from where the raycast hits in direction based on Vector3.Reflect() 
	// stores point in array
	// repeats til last bounce
	private void DrawPredictionShotLong(Vector3 position, Vector3 direction, int bouncesRemaining)
    {
        int currentBounce = 5 - bouncesRemaining;

        if (bouncesRemaining == 0) return;
        else if(currentBounce == 0)
        {
            bouncePoints[0] = position;
            DrawPredictionShotLong(position, direction, --bouncesRemaining);

            return;
        }
        Vector3 startingPosition = position;
        
        Ray ray = new Ray(startingPosition, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;

            if (hit.collider.gameObject.CompareTag("Player"))
                lineRender.material = previewHitMaterial;
            else if(bouncesRemaining == 1)
                lineRender.material = previewMaterial;
        }
        
        bouncePoints[currentBounce] = position;

        DrawPredictionShotLong(position, direction, --bouncesRemaining);
    }
}
