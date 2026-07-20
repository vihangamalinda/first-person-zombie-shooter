using UnityEngine;

public class FPController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float speed = 0.1f;
    float mouseSensitivity = 2.0f;

    Rigidbody rigidbody;
    CapsuleCollider capsuleCollider;
    public GameObject camera;

    Quaternion cameraRotation;
    Quaternion characterRotation;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        cameraRotation = camera.transform.rotation;
        characterRotation = this.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

        float yRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        float xRotation = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraRotation *= Quaternion.Euler(-xRotation, 0, 0);
        characterRotation *= Quaternion.Euler(0, yRotation, 0);

        this.transform.localRotation = characterRotation;
        camera.transform.localRotation = cameraRotation;


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
