using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
 	public float force;

    void FixedUpdate()
    {
		float moveHorizontal = Input.acceleration.x;
		float moveVertical   = Input.acceleration.z + .5f;
        
		GetComponent<Rigidbody>().AddRelativeForce( new Vector3(0, 0, force) );

		GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(-moveVertical/2, 0, -moveHorizontal/2) );
    }

	public void setForce(float newForce)
	{
		force = newForce;
	}
}