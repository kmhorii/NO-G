using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 cameraDirection;
    Vector3 currentDirection;
    [SerializeField]
    float rotateSpeed;
    [SerializeField]
    Camera mainCamera;
    float cameraStartRotation_x;
    float xRotation;
    // Start is called before the first frame update
    void Start()
    {
        currentDirection = Vector3.zero;
        cameraStartRotation_x = mainCamera.transform.rotation.x;
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();
        MovePlayer();
        if (Input.GetKeyUp(KeyCode.W))
        {
            ReturnCameraRotation();
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

    //Very basic movement toward center of camera
    //May add raycasts to see if there's anything in the way.
    void MovePlayer()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            AddForce();
        }
        transform.position += currentDirection * Time.deltaTime;
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
        Debug.Log(cameraStartRotation_x);
        mainCamera.transform.rotation = new Quaternion(cameraStartRotation_x, mainCamera.transform.rotation.y, mainCamera.transform.rotation.z, 0);
    }
}
