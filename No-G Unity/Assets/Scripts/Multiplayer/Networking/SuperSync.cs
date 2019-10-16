using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class SuperSync : MonoBehaviourPun, IPunObservable
{
	public Vector3 objPos;
	public Quaternion objRot;
	public Vector3 objScl;

	public float LerpSpeed = 3f;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine)
		{
			UpdateTransform();
		}
    }

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			stream.SendNext(gameObject.transform.position);
			stream.SendNext(gameObject.transform.rotation);
			stream.SendNext(gameObject.transform.localScale);
		}
		else if(stream.IsReading)
		{
			objPos = (Vector3)stream.ReceiveNext();
			objRot = (Quaternion)stream.ReceiveNext();
			objScl = (Vector3)stream.ReceiveNext();
		}
	}

	private void UpdateTransform()
	{
		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, objPos, LerpSpeed * Time.deltaTime);
		gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, objRot, LerpSpeed * Time.deltaTime);
		gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, objScl, LerpSpeed * Time.deltaTime);
	}
}
