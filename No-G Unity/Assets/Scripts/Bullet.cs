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

    public Quaternion initialRotation;
    public Vector3 initialVelocity;

	public bool exitPlayer = false;
    public bool recalculateRotation = false;

    public AudioSource wallBounceSound;

    void Start()
    {
        gunReference = GameObject.FindGameObjectWithTag("Gun").GetComponent<ShootingGun>();
        bounceNumber = gunReference.maxBounces;
        maxBounces = bounceNumber;
        wallBounceSound = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (recalculateRotation)
        {
            this.transform.rotation = Quaternion.LookRotation(this.GetComponent<Rigidbody>().velocity);
            recalculateRotation = false;
        }
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
        if (collision.collider.CompareTag("Wall"))
        {
            if (bounceNumber - 1 == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                recalculateRotation = true;
                wallBounceSound.Play();
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
            if (collider.gameObject.GetPhotonView().IsMine)
            {
                PlayerHealth pd = collider.gameObject.GetComponent<PlayerHealth>();

                if (bounceNumber <= maxBounces)
                {
                    int bulletDamage = defaultDamage + (maxBounces-bounceNumber-1) * damageIncrease;/*- (damageReduction * (maxBounces - bounceNumber - 1))*/
                    pd.DealDamage(shooter, bulletDamage);

                    gunReference.savedLineRender.enabled = false;
                }
                else
                {
                    gunReference.savedLineRender.enabled = false;
                }

                //photonView.RPC("DestroyBullet", RpcTarget.All);
            }

            Destroy(gameObject);
        }
    }
   

    [PunRPC]
	void DestroyBullet()
	{
		Destroy(gameObject);
	}
}
