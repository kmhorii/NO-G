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

    public int defaultDamage = 34;      
    public int damageReduction = 2;

	public bool exitPlayer = false;

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
			Destroy(gameObject);
			if (collision.gameObject.GetPhotonView().IsMine)
			{
				PlayerHealth pd = collision.gameObject.GetComponent<PlayerHealth>();

				if (bounceNumber <= maxBounces - 1)
				{
					int bulletDamage = defaultDamage - (damageReduction * (maxBounces - bounceNumber - 1));
					pd.DealDamage(shooter, bulletDamage);

					gunReference.savedLineRender.enabled = false;
				}
				else
				{
					gunReference.savedLineRender.enabled = false;
				}

				//photonView.RPC("DestroyBullet", RpcTarget.All);
			}
		}else if (collision.collider.tag == "Wall")
        {
            if (bounceNumber - 1 == 0)
            {
				Destroy(gameObject);
			}
            else bounceNumber--;
        }
    }

	[PunRPC]
	void DestroyBullet()
	{
		Destroy(gameObject);
	}
}
