﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    Rigidbody rigidbody;
    Vector3 cameraDirection;
    Vector3 currentDirection;

    public bool previewOn = false;
    public bool wasSettingsOpen = false;
    public bool alreadyFired = false;

    public Vector3 startPosition;
    public Quaternion startRotation;

    private string playerName;
    public string PlayerName
    {
        get
        {
            return playerName;
        }

        set
        {
            playerName = value;
        }
    }
   
    public float rotateSpeed;
    float modifiedRotateSpeed;
    [SerializeField]
    float movementSpeed;

    [SerializeField]
    Camera mainCamera;

	private GameObject cam;

    float cameraStartRotation_x;
    float xRotation;

	public GameObject CurrentWeapon;

    // Start is called before the first frame update
    void Start()
    {
		cam = gameObject.transform.GetChild(0).gameObject;

		gameObject.transform.localPosition = new Vector3(-1.8f, 2f, 1);
        rigidbody = GetComponent<Rigidbody>();
        currentDirection = Vector3.zero;
        cameraStartRotation_x = mainCamera.transform.rotation.x;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        startPosition = this.transform.position;
        startRotation = this.transform.rotation;

		if (photonView.IsMine) cam.SetActive(true);
		else cam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if (photonView.IsMine) 
		{
            if (!SceneManager.GetSceneByName("Settings").isLoaded)
            {
                modifiedRotateSpeed = rotateSpeed * (1 / Time.deltaTime);
                if (CurrentWeapon != null && !this.GetComponent<PlayerHealth>().isDead) ShootGun();
                else if (this.GetComponent<PlayerHealth>().isDead)
                {
                    CurrentWeapon.GetComponent<ShootingGun>().lineRender.enabled = false;
                }
                RotatePlayer();
                MovePlayer();
                EnemysGateIsDown();
                ReturnRotation();

                //if (Input.GetKeyUp(KeyCode.W))
                //{
                //    ReturnCameraRotation();
                //}
            }
            wasSettingsOpen = SceneManager.GetSceneByName("Settings").isLoaded;
		}
    }

    [PunRPC]
    void setPlayerName(string name)
    {
        if(!photonView.IsMine) this.gameObject.name = name;
    }
    void GetCameraDirection()
    {
        cameraDirection = Camera.main.transform.forward;
        
    }

    void AddForce()
    {
        GetCameraDirection();
        currentDirection += cameraDirection;
    }

	void ShootGun()
	{
        //Debug.Log(Input.GetKeyDown(KeyCode.Mouse1));
        //Debug.Log("Key: " + Input.GetKey(KeyCode.Mouse1));

        if (Input.GetKeyDown(KeyCode.Mouse1) || (Input.GetAxisRaw("Preview") == 1 && !previewOn))
		{
            previewOn = true;
			CurrentWeapon.GetComponent<ShootingGun>().DownAiming();
            CurrentWeapon.GetComponent<ShootingGun>().lineRender.enabled = true;
		}

		if (Input.GetKey(KeyCode.Mouse1) || (Input.GetAxisRaw("Preview") == 1 && previewOn))
		{
            CurrentWeapon.GetComponent<ShootingGun>().Aiming();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1) || (Input.GetAxisRaw("Preview") == 0 && previewOn))
		{
			CurrentWeapon.GetComponent<ShootingGun>().DoneAiming();
            previewOn = false;

        }

        //Reload set to x or r
        /*if (Input.GetButtonDown("Reload"))
		{
			CurrentWeapon.GetComponent<ShootingGun>().Reloading();
		}*/

        //Firing set to mouse0 or right trigger
		if (Input.GetKeyDown(KeyCode.Mouse0)|| (Input.GetAxisRaw("Fire1") == 1 && !alreadyFired))
		{
            alreadyFired = true;
            if (!CurrentWeapon.GetComponent<ShootingGun>().isShooting && CurrentWeapon.GetComponent<ShootingGun>().currentAmmo > 0)
            {
                CurrentWeapon.GetComponent<ShootingGun>().Shooting();
            }
        }
        if(Input.GetKeyUp(KeyCode.Mouse0)|| Input.GetAxisRaw("Fire1") == 0)
        {
            alreadyFired = false;
        }
	}

    //Very basic movement toward center of camera
    //May add raycasts to see if there's anything in the way.
    void MovePlayer()
    {
        if (Input.GetButtonDown("Move") && !wasSettingsOpen)
        {
            GetCameraDirection();
            rigidbody.AddForce(cameraDirection*movementSpeed, ForceMode.VelocityChange);
            //AddForce();
        }
        //transform.position += currentDirection * Time.deltaTime * movementSpeed;
    }

    //Uses mouse movement to rotate player and camera
    //It's a little unwieldy right now, but we'll smooth it out later
    void RotatePlayer()
    {
        xRotation += -Input.GetAxis("Mouse Y") * modifiedRotateSpeed *Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * modifiedRotateSpeed *Time.deltaTime);
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        
        //if(mainCamera.transform.rotation.x > 90)
        //{
        //    mainCamera.transform.rotation = new Quaternion(90, mainCamera.transform.rotation.y, mainCamera.transform.rotation.z, 0);
        //}
        //else if (mainCamera.transform.rotation.x < -90)
        //{
        //    mainCamera.transform.rotation = new Quaternion(-90, mainCamera.transform.rotation.y, mainCamera.transform.rotation.z, 0);
        //}


    }
    private void EnemysGateIsDown()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Quaternion rotationChange = mainCamera.transform.localRotation;
            //Will change to lerp later (must then be put in update)
            transform.rotation =  new Quaternion(transform.rotation.x + rotationChange.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            mainCamera.transform.localRotation = new Quaternion(0, 0, 0, 0);
            CurrentWeapon.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    void ReturnRotation()
    {
        if(Input.GetKeyDown(KeyCode.A))
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
    }
    public void RespawnPosition()
    {
        if (photonView.IsMine)
        {
            this.transform.position = startPosition;
            this.transform.rotation = startRotation;
        }
    }
}
