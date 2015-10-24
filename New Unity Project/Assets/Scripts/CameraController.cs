using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    public GameObject player;
    private Vector3 offset;
    private float angle;
    public float distance;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        angle = player.GetComponent<PlayerController>().angle;

        transform.position = player.transform.position + new Vector3(-distance * Mathf.Sin(angle * Mathf.PI / 180), offset.y, -distance * Mathf.Cos(angle * Mathf.PI / 180));
        transform.rotation = Quaternion.Euler(10, angle, 0.0f);
    }
}
