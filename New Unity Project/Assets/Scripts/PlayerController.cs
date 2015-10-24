using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float tilt;
    public Boundary boundary;
    public float angle;

    void FixedUpdate()
    {
        float moveHorizontal = Input.acceleration.x;
        angle = (angle + moveHorizontal * 2) % 360;
        
        GetComponent<Rigidbody>().position += new Vector3(Mathf.Sin(angle * Mathf.PI / 180), 0.0f, Mathf.Cos(angle * Mathf.PI / 180)) * speed;

        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, angle + moveHorizontal * tilt * 5, moveHorizontal * -tilt * 5);
    }
}