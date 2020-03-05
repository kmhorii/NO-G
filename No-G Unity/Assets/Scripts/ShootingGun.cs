using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;

//https://www.youtube.com/watch?v=GttdLYKEJAM
//https://www.youtube.com/watch?v=kAx5g9V5bcM

public class ShootingGun : MonoBehaviourPun
{
    public GameObject bulletPrefab;
    public Transform muzzle;
	public Transform mainCamera;

    public bool isShooting = false;
    public bool aiming = false;
    public bool isReloading = false;

    public int bulletSpeed;
    public float fireSpeed;

    public int maxAmmo = 3;
    public int currentAmmo;
    public float reloadTime = 1.5f;

    public int maxBounces = 8;
    public float maxStepDistance = 1000;

    public Material previewMaterial;
    public Material previewHitMaterial;

    public Vector3[] bouncePoints;
    public Vector3[] savedBounces;

    public LineRenderer lineRender;
    public LineRenderer savedLineRender;

	public StatsManager myStats;

    public float savedPreviewTime = 1.5f;

    public int shotsFired = 0;
    public int shotsHitEnemy = 0;
    public int shotsHitSelf = 0;

	public string shooter;

    public ParticleSystem particleMuzzle;

    public Renderer cartridge1;
    public Renderer cartridge2;
    public Renderer cartridge3;

    public Material cartridgePlain;
    public Material cartridgeGlow;

    public GameObject muzzleFlashLights;

    public AudioSource shootingSound;

    public GameObject gunComponents;

    public Animator recoil;

    void Start()
    {
        currentAmmo = maxAmmo;
        SetCartridgeMaterial();
        mainCamera = GameObject.Find("gunFake").GetComponent<Transform>();

        lineRender.enabled = false;
        savedLineRender.enabled = false;
    }

    //Tracking camera for Photon
    //Previous bug of not finding camera when loading in player
    private void Update()
    {
        if (photonView.IsMine)
        {
			gameObject.layer = 15; // myGun layer
			foreach(Transform child in transform) child.gameObject.layer = 15;
            //if (SceneManager.GetActiveScene().name == "MergeTest")
            //{
            //    transform.position = new Vector3(mainCamera.position.x, mainCamera.position.y, mainCamera.position.z);
            //    transform.rotation = new Quaternion(mainCamera.transform.rotation.x, mainCamera.transform.rotation.y, mainCamera.transform.rotation.z, mainCamera.transform.rotation.w);
            //}

            transform.position = new Vector3(mainCamera.position.x, mainCamera.position.y, mainCamera.position.z);
            transform.rotation = new Quaternion(mainCamera.transform.rotation.x, mainCamera.transform.rotation.y, mainCamera.transform.rotation.z, mainCamera.transform.rotation.w);

            if (currentAmmo < maxAmmo && !isReloading)
            {
                isReloading = true;
                Invoke("ChargeBullet", reloadTime);
            }
            else
            {
                if(currentAmmo > maxAmmo)
                {
                    currentAmmo = maxAmmo;
                    SetCartridgeMaterial();
                }
            }
        }
    }

    //Preview Shot
	public void DownAiming()
	{
		aiming = true;
		lineRender.enabled = true;
      //  Debug.Log(lineRender.enabled);
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
            SetCartridgeMaterial();

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

            SetCartridgeMaterial();
        }
        isReloading = false;

    }

    private void SetCartridgeMaterial()
    {
        Material[] tempCart1 = cartridge1.materials;
        Material[] tempCart2 = cartridge2.materials;
        Material[] tempCart3 = cartridge3.materials;

        if (currentAmmo == 0)
        {
            tempCart1[2] = cartridgePlain;
            tempCart2[2] = cartridgePlain;
            tempCart3[2] = cartridgePlain;
        }
        else if (currentAmmo == 1)
        {
            tempCart1[2] = cartridgeGlow;
            tempCart2[2] = cartridgePlain;
            tempCart3[2] = cartridgePlain;
        }
        else if (currentAmmo == 2)
        {
            tempCart1[2] = cartridgeGlow;
            tempCart2[2] = cartridgeGlow;
            tempCart3[2] = cartridgePlain;
        }
        else if (currentAmmo == 3)
        {
            tempCart1[2] = cartridgeGlow;
            tempCart2[2] = cartridgeGlow;
            tempCart3[2] = cartridgeGlow;
        }

        cartridge1.materials = tempCart1;
        cartridge2.materials = tempCart2;
        cartridge3.materials = tempCart3;

    }

    //Photon Shooting
    private void Shoot()
    {
		photonView.RPC("RPCShoot", RpcTarget.All, PlayerInfo.Name);

        particleMuzzle.Play();
        muzzleFlashLights.SetActive(true);
        Invoke("MuzzleLightsOff", 0.1f);

        shootingSound.Play();

        recoil.SetTrigger("playRecoil");

        ////gunComponents.transform.Translate(Vector3.back * Time.deltaTime, Space.Self);
        ////Vector3 destination = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.002f);
        //gunComponents.transform.localPosition = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, -0.003f), 0.2f * Time.deltaTime);
        //Invoke("RecoilBack", 0.3f);
    }

    private void MuzzleLightsOff()
    {
        muzzleFlashLights.SetActive(false);
    }

    private void RecoilBack()
    {
        //gunComponents.transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
        //gunComponents.transform.localPosition = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, 0.003f), 0.2f * Time.deltaTime);

    }

    //Spawning Bullet for Photon
    [PunRPC]
	public void RPCShoot(string shooter)
	{
		GameObject bullet = Instantiate(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation);

		Rigidbody rb = bullet.GetComponent<Rigidbody>();
		rb.velocity = muzzle.transform.forward * bulletSpeed;

		bullet.GetComponent<Bullet>().shooter = shooter;
        shotsFired++;
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
        SetCartridgeMaterial();
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
