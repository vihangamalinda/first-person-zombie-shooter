using UnityEngine;

public class FPController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float speed = 0.1f;
    float mouseSensitivity = 2.0f;

    float minimumX = -90.0f;
    float maximumX = 90.0f;
    //float minimumY = -360.0f;
    //float maximumY = 360.0f;

    bool isCursorLocked = true;
    bool shouldLockCursor = true;



    Rigidbody rigidbody;
    CapsuleCollider capsuleCollider;
    public GameObject camera;
    public Animator animator;


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
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("shouldAim", !animator.GetBool("shouldAim"));
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("fire");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("reload");
        }


        //bool isWalking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        //animator.SetBool("isWalking", isWalking);

        bool isWalking = Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0;
        bool isAlreadyWalking = animator.GetBool("isWalking");

        if (isWalking)
        {
            if (!isAlreadyWalking)
            {
                animator.SetBool("isWalking", true);
            }

        }
        else
        {
            if (isAlreadyWalking)
            {
                animator.SetBool("isWalking", false);
            }
        }

    }

    void FixedUpdate()
    {

        float yRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        float xRotation = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraRotation *= Quaternion.Euler(-xRotation, 0, 0);
        characterRotation *= Quaternion.Euler(0, yRotation, 0);

        cameraRotation = ClampRoationAroundXAxis(cameraRotation);

        this.transform.localRotation = characterRotation;
        camera.transform.localRotation = cameraRotation;


        bool shouldJump = Input.GetKeyDown(KeyCode.Space);

        if (shouldJump && isGrounded())
        {
            rigidbody.AddForce(0, 300, 0);
        }



        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

        transform.position += camera.transform.forward * z + camera.transform.right * x; //new Vector3(x * speed, 0, z * speed);

        UpdateCursorLock();
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

    Quaternion ClampRoationAroundXAxis(Quaternion quaternion)
    {
        // Convert the quaternion to an angle-axis representation (nomralizing the quaternion first)
        quaternion.x /= quaternion.w;
        quaternion.y /= quaternion.w;
        quaternion.z /= quaternion.w;
        quaternion.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quaternion.x);
        angleX = Mathf.Clamp(angleX, minimumX, maximumX);
        quaternion.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return quaternion;
    }

    public void SetCursorLock(bool value)
    {
        shouldLockCursor = value;

        if (!shouldLockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (shouldLockCursor)
        {
            InternalLockUpdate();
        }
    }

    public void InternalLockUpdate()
    {
        bool isEscapeReleased = Input.GetKeyUp(KeyCode.Escape);
        bool isLeftMousePressed = Input.GetMouseButtonUp(0); //left  mouse button index is 0, right mouse button index is 1, middle mouse button index is 2

        if (isEscapeReleased)
        {
            isCursorLocked = false;
        }
        else if (isLeftMousePressed)
        {
            isCursorLocked = true;
        }



        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
