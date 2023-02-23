using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float crouchSpeed = 3f;
    public float sneakSpeed = 2f;
    public float jumpSpeed = 8f;
    public float mouseSensitivity = 100f;
    public float maxFallSpeed = -20f;
    public float crouchHeight = 0.5f;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private bool isGrounded = true;
    private bool isCrouching = false;
    private bool isSneaking = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Rotation
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, horizontalRotation, 0f);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Movement
        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            speed = runSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            speed = crouchSpeed;
            isCrouching = true;
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            speed = sneakSpeed;
            isSneaking = true;
        }
        else
        {
            isCrouching = false;
            isSneaking = false;
        }

        float horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        if (controller.isGrounded)
        {
            isGrounded = true;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpSpeed;
            }
            else
            {
                verticalVelocity = 0f; // reset vertical velocity when on the ground
            }
        }
        else
        {
            isGrounded = false;
            if (verticalVelocity > maxFallSpeed)
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
        }

        Vector3 movement = new Vector3(horizontalMovement, verticalVelocity * Time.deltaTime, verticalMovement);
        movement = transform.rotation * movement;

        if (isCrouching)
        {
            controller.height = crouchHeight;
            controller.center = new Vector3(0f, crouchHeight / 2f, 0f);
        }
        else
        {
            controller.height = 2f;
            controller.center = new Vector3(0f, 1f, 0f);
        }

        if (isSneaking)
        {
            movement *= 0.5f;
        }

        controller.Move(movement);

        if (!isGrounded && !isSneaking && !isCrouching)
        {
            // limit falling speed
            float yVelocity = controller.velocity.y;
            if (yVelocity < maxFallSpeed)
            {
                controller.Move(new Vector3(0f, maxFallSpeed - yVelocity, 0f) * Time.deltaTime);
            }
        }
    }

}
