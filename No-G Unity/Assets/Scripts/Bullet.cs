using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ShootingGun gunReference;

    public int bounceNumber;
    private int maxBounces;

    public int playerHealth;

    public int defaultDamage = 34;
    public int damageReduction = 2;

    void Start()
    {
        gunReference = GameObject.FindGameObjectWithTag("Gun").GetComponent<ShootingGun>();
        bounceNumber = gunReference.maxBounces;
        maxBounces = bounceNumber;
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

        if (collision.gameObject.layer == 11)
        {
            if (bounceNumber <= maxBounces - 1)
            {
                //collision.gameObject.GetComponent<Player>().playerHealth;
                int bulletDamage = defaultDamage - (damageReduction * (maxBounces - bounceNumber));
            }
        }


    } 
}
