using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=GttdLYKEJAM

public class ShootingGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform muzzle;

    public bool isShooting = false;

    public int bulletSpeed;
    public float fireSpeed;

    public int maxBounces = 4;
    public float maxStepDistance = 1000;

    public Material previewMaterial;

    public Vector3[] bouncePoints;

    public LineRenderer lineRender;

    public bool aiming;

    public Image LeftMouseButton;
    public Image RightMouseButton;

    public 

    void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        lineRender.enabled = false;
    }

    private void OnDrawGizmos()
    {
        DrawPredictionShot(muzzle.position, muzzle.forward, maxBounces);
    }

    void Update()
    {
        DrawPredictionShotLong(muzzle.position, muzzle.forward, maxBounces);

        for (int i = 0; i <= 4; i++)
        {
            GetComponent<LineRenderer>().SetPosition(i, bouncePoints[i]);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RightMouseButton.GetComponent<Image>().color = Color.grey;
            aiming = true;
            lineRender.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            RightMouseButton.GetComponent<Image>().color = Color.white;
            aiming = false;
            lineRender.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            LeftMouseButton.GetComponent<Image>().color = Color.grey;
            Shoot();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            LeftMouseButton.GetComponent<Image>().color = Color.white;
        }
    }

    private void Shoot()
    {
        if (!isShooting)
        {
            isShooting = true;
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

    private void DrawPredictionShot(Vector3 position, Vector3 direction, int bouncesRemaining)
    {
        if (bouncesRemaining == 0)
        {
            return;
        }

        Vector3 startingPosition = position;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }
        else
        {
            position += direction * maxStepDistance;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(startingPosition, position);
    }
}
