using UnityEngine;

public class FPController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float speed = 0.1f;
    Rigidbody rigidbody;
    CapsuleCollider capsuleCollider;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        bool shouldJump = Input.GetKeyDown(KeyCode.Space);

        if (shouldJump && isGrounded())
        {
            rigidbody.AddForce(0, 300, 0);
        }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.position += new Vector3(x * speed, 0, z * speed);
    }

    bool isGrounded()
    {
        RaycastHit hitInformation;

        if (Physics.SphereCast(transform.position, capsuleCollider.radius, Vector3.down, out hitInformation,
            (capsuleCollider.height / 2f) - capsuleCollider.radius + 0.1f))
        {
            return true;
        }
        return false;
    }
}
