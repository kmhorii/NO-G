using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    Rigidbody rigidbody;
    Vector3 cameraDirection;
    Vector3 currentDirection;

    [SerializeField]
    float rotateSpeed;
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

		if (photonView.IsMine) cam.SetActive(true);
		else cam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if (photonView.IsMine)
		{
			if (CurrentWeapon != null) ShootGun();
			RotatePlayer();
			MovePlayer();

			if (Input.GetKeyUp(KeyCode.W))
			{
				ReturnCameraRotation();
			}
		}
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
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			CurrentWeapon.GetComponent<ShootingGun>().DownAiming();
		}

		if (Input.GetKey(KeyCode.Mouse1))
		{
			CurrentWeapon.GetComponent<ShootingGun>().Aiming();
		}

		if (Input.GetKeyUp(KeyCode.Mouse1))
		{
			CurrentWeapon.GetComponent<ShootingGun>().DoneAiming();
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			CurrentWeapon.GetComponent<ShootingGun>().Reloading();
		}

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
            if (!CurrentWeapon.GetComponent<ShootingGun>().isShooting)
            {
                CurrentWeapon.GetComponent<ShootingGun>().Shooting();
            }
        }
	}

    //Very basic movement toward center of camera
    //May add raycasts to see if there's anything in the way.
    void MovePlayer()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            GetCameraDirection();
            rigidbody.AddForce(cameraDirection*movementSpeed, ForceMode.VelocityChange);
            Debug.Log("Moving");
            //AddForce();
        }
        //transform.position += currentDirection * Time.deltaTime * movementSpeed;
    }

    //Uses mouse movement to rotate player and camera
    //It's a little unwieldy right now, but we'll smooth it out later
    void RotatePlayer()
    {
        xRotation += -Input.GetAxis("Mouse Y") * rotateSpeed;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * rotateSpeed);
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

    void ReturnCameraRotation()
    {
        
        //mainCamera.transform.localRotation = Quaternion.Euler(-mainCamera.transform.localRotation.x *rotateSpeed, 0, 0);
    }
}
