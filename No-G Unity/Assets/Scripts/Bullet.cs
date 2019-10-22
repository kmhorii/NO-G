using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public ShootingGun gunReference;

	public string shooter;
    public int bounceNumber;
    public int maxBounces;

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
        if (collision.collider.tag == "Player")
        {
            PlayerHealthandAmmo pd = collision.gameObject.GetComponent<PlayerHealthandAmmo>();

            if (bounceNumber <= maxBounces - 1)
            {
                int bulletDamage = defaultDamage - (damageReduction * (maxBounces - bounceNumber));
                pd.DealDamage(bulletDamage);

                Destroy(gameObject);
                gunReference.savedLineRender.enabled = false;
            }
            else
            {
                Destroy(gameObject);
                gunReference.savedLineRender.enabled = false;
            }
        }
        else if (collision.collider.tag == "Wall")
        {
            if (bounceNumber - 1 == 0)
            {
                Destroy(gameObject);
            }
            else bounceNumber--;
        }
    }
}
