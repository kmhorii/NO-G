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

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == 9)
        {
            if (bounceNumber - 1 == 0)
            {
                Destroy(gameObject);
            }

            bounceNumber--;
        }
    }
}
