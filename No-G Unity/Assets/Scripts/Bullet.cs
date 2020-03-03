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

    public int defaultDamage = 26;      
    public int damageReduction = 2;
    public int damageIncrease = 5;

	public bool exitPlayer = false;

    void Start()
    {
        gunReference = GameObject.FindGameObjectWithTag("Gun").GetComponent<ShootingGun>();
        bounceNumber = gunReference.maxBounces;
        maxBounces = bounceNumber;
    }

    private void OnCollisionEnter(Collision collision)
    {
  //      if (collision.collider.tag == "Player")
  //      {
		//	Destroy(gameObject);
		//	if (collision.gameObject.GetPhotonView().IsMine)
		//	{
		//		PlayerHealth pd = collision.gameObject.GetComponent<PlayerHealth>();

		//		if (bounceNumber <= maxBounces)
		//		{
		//			int bulletDamage = defaultDamage /*- (damageReduction * (maxBounces - bounceNumber - 1))*/;
		//			pd.DealDamage(shooter, bulletDamage);

		//			gunReference.savedLineRender.enabled = false;
		//		}
		//		else
		//		{
		//			gunReference.savedLineRender.enabled = false;
		//		}

		//		//photonView.RPC("DestroyBullet", RpcTarget.All);
		//	}
		//}else if (collision.collider.tag == "Wall")
        {
            Debug.Log(bounceNumber);
            if (bounceNumber - 1 == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                bounceNumber--;
                Vector3 scale = gameObject.transform.localScale;
                gameObject.transform.localScale = new Vector3(scale.x + .1f, scale.y + .1f, scale.z + .1f);
                gameObject.GetComponent<TrailRenderer>().startWidth += 0.1f;
                gameObject.GetComponent<TrailRenderer>().endWidth += 0.1f;

                gameObject.GetComponent<SphereCollider>().enabled = false;
                gameObject.GetComponent<SphereCollider>().enabled = true;
            }
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            Destroy(gameObject);
            if (collider.gameObject.GetPhotonView().IsMine)
            {
                PlayerHealth pd = collider.gameObject.GetComponent<PlayerHealth>();

                if (bounceNumber <= maxBounces)
                {
                    int bulletDamage = defaultDamage + (maxBounces-bounceNumber) * damageIncrease;/*- (damageReduction * (maxBounces - bounceNumber - 1))*/
                    pd.DealDamage(shooter, bulletDamage);

                    gunReference.savedLineRender.enabled = false;
                }
                else
                {
                    gunReference.savedLineRender.enabled = false;
                }

                //photonView.RPC("DestroyBullet", RpcTarget.All);
            }
        }
    }

    [PunRPC]
	void DestroyBullet()
	{
		Destroy(gameObject);
	}
}
