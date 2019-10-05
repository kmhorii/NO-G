using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ShootingGun gunReference;

    public int bounceNumber;

    void Start()
    {
        gunReference = GameObject.FindGameObjectWithTag("Gun").GetComponent<ShootingGun>();
        bounceNumber = gunReference.maxBounces;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    transform.Translate(Vector3.forward * speed);

    //    Ray ray = new Ray(transform.position, transform.forward);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit, Time.deltaTime * speed))
    //    {
    //        Vector3 reflectDirection = Vector3.Reflect(ray.direction, hit.normal);
    //    }
    //}

    //private void OnTriggerEnter(Collider collider)
    //{
    //    if (bounceNumber == 0)
    //    {
    //        Destroy(gameObject);
    //    }

    //    bounceNumber--;
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (bounceNumber - 1 == 0)
        {
            Destroy(gameObject);
        }

        bounceNumber--;
    }
}
