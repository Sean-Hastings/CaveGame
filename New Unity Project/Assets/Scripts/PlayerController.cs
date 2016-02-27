using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
 	public float force;
	public GameObject sparks;

    void FixedUpdate()
    {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical   = Input.GetAxis ("Forward");
        
		GetComponent<Rigidbody>().AddRelativeForce( new Vector3(0, 0, force) );

		GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(-moveVertical/2, 0, -moveHorizontal/2) );
	}

	void OnCollisionEnter(Collision collision) {
		foreach (ContactPoint contact in collision.contacts) {
			Instantiate (sparks, transform.position, transform.rotation);
		}
	}

	public void setForce(float newForce)
	{
		force = newForce;
	}

	public void shoot()
	{
		Debug.Break ();
	}
}