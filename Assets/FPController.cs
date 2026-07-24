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
    //public AudioSource shotAudioSource;

    public AudioSource[] walkAudioSourceArr;
    public AudioSource jumpAudioSource;
    public AudioSource landAudioSource;
    public AudioSource ammoPickUpAudioSource;
    public AudioSource medikitPickUpAudioSource;
    public AudioSource dryFireAudioSource;
    public AudioSource painfulAudioSource;
    public AudioSource gunRelodAudioSource;


    Quaternion cameraRotation;
    Quaternion characterRotation;


    // Inventory system variables
    int ammoCount = 0;
    int loadedAmmoCount = 0;
    readonly int maxAmmoCount = 20;
    readonly int maxLoadedAmmoCount = 6;

    int characterHealth = 100;
    int medikitCount = 0;
    readonly int healthPerMedikit = 15;
    readonly int maxMedikitCount = 4;

    private static readonly int shouldAimHash = Animator.StringToHash("shouldAim");
    private static readonly int fireHash = Animator.StringToHash("fire");
    private static readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int reloadHash = Animator.StringToHash("reload");


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
            animator.SetBool(shouldAimHash, !animator.GetBool(shouldAimHash));
        }

        bool shouldFire = Input.GetMouseButtonDown(0) && !animator.GetBool(fireHash);

        if (shouldFire)
        {
            this.PerformGunFiring();

        }

        bool shouldUseMedikit = Input.GetKeyDown(KeyCode.H) && this.medikitCount > 0 && this.characterHealth < 100;
        if (shouldUseMedikit)
        {
            this.ApplyMedikit();
        }


        bool shouldReload = Input.GetKeyDown(KeyCode.R) && this.loadedAmmoCount < this.maxLoadedAmmoCount && this.ammoCount > 0;
        //Debug.Log("Should reload: " + shouldReload);
        if (shouldReload)
        {
            this.PerformAmmoRelod();
        }


        //bool isWalking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        //animator.SetBool("isWalking", isWalking);

        bool isWalking = Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0;
        bool isAlreadyWalking = animator.GetBool(isWalkingHash);

        if (isWalking)
        {
            if (!isAlreadyWalking)
            {
                animator.SetBool(isWalkingHash, true);
                InvokeRepeating("PlayWalkAudio", 0, 0.4f);
            }

        }
        else
        {
            if (isAlreadyWalking)
            {
                animator.SetBool(isWalkingHash, false);
                CancelInvoke("PlayWalkAudio");
            }
        }


        bool shouldJump = Input.GetKeyDown(KeyCode.Space);

        if (shouldJump && isGrounded())
        {
            this.PerformJump();
        }

    }


    void PlayWalkAudio()
    {
        AudioSource audioSource = new AudioSource();
        int n = Random.Range(1, walkAudioSourceArr.Length);

        audioSource = walkAudioSourceArr[n];
        audioSource.Play();

        walkAudioSourceArr[n] = walkAudioSourceArr[0];
        walkAudioSourceArr[0] = audioSource;
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

    void OnCollisionEnter(Collision collision)
    {
        CollidedWithCollectables(collision);

        CollidedWithHarmFullFloor(collision);

        bool hasCollidedWithPlane = collision.gameObject.name == "Plane";
        if (hasCollidedWithPlane)
        {
            landAudioSource.Play();
            if (animator.GetBool("isWalking"))
            {
                InvokeRepeating("PlayWalkAudio", 0, 0.4f);
            }
        }
    }

    private void CollidedWithHarmFullFloor(Collision collision)
    {
        Debug.Log("Character Health: " + this.characterHealth);

        bool hasCollidedWithLarva = collision.gameObject.CompareTag("larva");
        if (hasCollidedWithLarva)
        {
            if (this.characterHealth > 10)
            {
                this.characterHealth = Mathf.Clamp(this.characterHealth - 10, 0, 100);

                bool isCharacterDead = this.characterHealth == 0;
                if (isCharacterDead)
                {
                    Debug.Log("Character has died");
                }

            }


            painfulAudioSource.Play();
        }
    }
    private void CollidedWithCollectables(Collision collision)
    {
        bool collidedWithAmmoBox = collision.gameObject.tag == "Ammo";
        bool canCollectAmmo = collidedWithAmmoBox && this.ammoCount < this.maxAmmoCount;

        if (canCollectAmmo)
        {
            ammoPickUpAudioSource.Play();
            Debug.Log("Collided with Ammo Box");
            this.ammoCount = Mathf.Clamp(this.ammoCount + 6, 0, this.maxAmmoCount);
            Destroy(collision.gameObject);

            Debug.Log("Ammo Count: " + this.ammoCount);
        }

        bool collidedWithMediKit = collision.gameObject.tag == "Medikit";
        bool canCollectMediKit = collidedWithMediKit && this.medikitCount < this.maxMedikitCount;

        if (canCollectMediKit)
        {
            medikitPickUpAudioSource.Play();
            Debug.Log("Collided with MediKit");
            this.medikitCount = Mathf.Clamp(this.medikitCount + 1, 0, this.maxMedikitCount);
            Destroy(collision.gameObject);

            Debug.Log("Medikit Count: " + this.medikitCount);
        }
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



    private void PerformJump()
    {
        rigidbody.AddForce(0, 300, 0);
        jumpAudioSource.Play();
        if (animator.GetBool(isWalkingHash))
        {
            CancelInvoke("PlayWalkAudio");
        }
    }
    private void PerformAmmoRelod()
    {
        Debug.Log("Before reload - loaded ammo: " + this.loadedAmmoCount);
        Debug.Log("Before reload - current ammo count: " + this.ammoCount);

        animator.SetTrigger(reloadHash);
        gunRelodAudioSource.Play();
        int ammoToReload = Mathf.Min(this.maxLoadedAmmoCount - this.loadedAmmoCount, this.ammoCount);

        this.loadedAmmoCount = Mathf.Clamp(this.loadedAmmoCount + ammoToReload, 0, this.maxLoadedAmmoCount);
        this.ammoCount -= ammoToReload;
        Debug.Log("After reload - loaded ammo: " + this.loadedAmmoCount);
        Debug.Log("After reload - current ammo count: " + this.ammoCount);

    }

    private void ApplyMedikit()
    {
        this.characterHealth = Mathf.Clamp(this.characterHealth + this.healthPerMedikit, 0, 100);
        this.medikitCount--;
        Debug.Log("Used medikit. Current health: " + this.characterHealth + ", remaining medikits: " + this.medikitCount);
    }

    private void PerformGunFiring()
    {
        if (this.loadedAmmoCount > 0)
        {
            animator.SetTrigger(fireHash);
            this.loadedAmmoCount--;

            //shotAudioSource.Play();
        }
        else if (animator.GetBool(shouldAimHash))
        {
            //Dry fire sound or play empty magazine animation
            Debug.Log("Dry fire - no ammo left in the magazine");
            dryFireAudioSource.Play();
        }
    }

}
