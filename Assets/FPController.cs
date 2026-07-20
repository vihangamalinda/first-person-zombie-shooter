using UnityEngine;

public class FPController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float speed = 5f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.position = new Vector3(x* speed, 0, z* speed);
    }
}
