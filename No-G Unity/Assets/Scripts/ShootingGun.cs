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

    public int maxAmmo = 9;
    public int currentAmmo;
    public float reloadTime = 1.5f;

    public int maxBounces = 8;
    public float maxStepDistance = 1000;

    public Material previewMaterial;

    public Vector3[] bouncePoints;
    public Vector3[] savedBounces;

    public LineRenderer lineRender;

    public LineRenderer savedLineRender;

    public float savedPreviewTime = 1.5f;

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
		DrawPredictionShotLong(muzzle.position, muzzle.forward, 4);

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
	public void Reloading()
	{
		StartCoroutine(Reload());
		return;
	}

    //Shooting
	public void Shooting()
	{
		if (currentAmmo == 0)
			StartCoroutine(Reload());
		else if (!isShooting && !isReloading)
		{
			Shoot();
			isShooting = true;

			if (Input.GetMouseButton(1) || Input.GetAxisRaw("Preview") == 1)
			{
				SavePreview();
			}
			currentAmmo--;

            Invoke("FireDelay", fireSpeed);
        }
		else
		{
            Invoke("FireDelay", fireSpeed);
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;

        isReloading = false;
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

    // raycasts from muzzle till it hits a wall
    // creates new raycast from where the raycast hits in direction based on Vector3.Reflect() 
    // stores point in array
    // repeats til last bounce
    /*private void DrawPredictionShotLong(Vector3 position, Vector3 direction, int bouncesRemaining)
    {
        //Debug.Log("Drawing");

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
    }*/

    public void RespawnGun()
    {
        currentAmmo = maxAmmo;
    }

    // recursive version, couldn't figure out how to store the vector points with this
    private void DrawPredictionShotLong(Vector3 position, Vector3 direction, int bouncesRemaining)
    {
        int currentBounce = 4 - bouncesRemaining;

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
        }
        
        bouncePoints[currentBounce] = position;

        DrawPredictionShotLong(position, direction, --bouncesRemaining);
    }
}
