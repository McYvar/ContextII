using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkingForce;
    [SerializeField] float counterForce;
    [SerializeField] float maxWalkingSpeed;
    [SerializeField] Transform playerOrientation;
    Rigidbody rb;

    public Camera playerCameraTransform;
    [SerializeField] Vector3 playerCameraOffset;

    [SerializeField] float cameraSensitivity;

    float threshold = 0.01f;
    float horizontalInput;
    float verticalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // later added into the UI section
        Cursor.visible = false;
    }

    private void Update()
    {
        verticalInput = Convert.ToInt16(Input.GetKey(KeyCode.W)) - Convert.ToInt16(Input.GetKey(KeyCode.S));
        horizontalInput = Convert.ToInt16(Input.GetKey(KeyCode.D)) - Convert.ToInt16(Input.GetKey(KeyCode.A));
    }

    private void LateUpdate()
    {
        CameraFollowsPlayer();
        CameraMouseInput();
    }

    private void FixedUpdate()
    {
        PlayerPhysicsMovement();
    }

    void PlayerPhysicsMovement()
    {
        Vector2 magnitude = VelocityRelativeToCameraRotation();

        // Cancel out input if the magnitude of the axis gets too high
        float xMagnitude = magnitude.x, yMagnitude = magnitude.y;
        if (horizontalInput > 0 && xMagnitude > maxWalkingSpeed) horizontalInput = 0;
        if (horizontalInput < 0 && xMagnitude < -maxWalkingSpeed) horizontalInput = 0;
        if (verticalInput > 0 && yMagnitude > maxWalkingSpeed) verticalInput = 0;
        if (verticalInput < 0 && yMagnitude < -maxWalkingSpeed) verticalInput = 0;

        CounterMovement(magnitude);

        rb.AddForce(Quaternion.Euler(0, playerCameraTransform.transform.eulerAngles.y, 0) * SurfaceInputVector() * walkingForce);
    }
    
    Vector3 SurfaceInputVector()
    {
        Vector3 inputVector = new Vector3(horizontalInput, 0, verticalInput);
        if (inputVector.magnitude > 1) return inputVector.normalized;
        else return inputVector;
    }

    // the main camera is not in hierachy with the player object, instead it just follows the players head
    void CameraFollowsPlayer()
    {
        playerCameraTransform.transform.position = playerOrientation.position + playerCameraOffset;
    }

    void CameraMouseInput()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = -Input.GetAxisRaw("Mouse Y");

        float eulerX = playerCameraTransform.transform.eulerAngles.x;
        if (eulerX > 180 && eulerX <= 360) eulerX -= 360;

        float resultXRotation = Mathf.Clamp(eulerX + mouseY * cameraSensitivity, -90, 90);
        float resultYRotation = playerCameraTransform.transform.eulerAngles.y + mouseX * cameraSensitivity;

        playerCameraTransform.transform.rotation = Quaternion.Euler(resultXRotation, resultYRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, playerCameraTransform.transform.eulerAngles.y, 0);
    }

    private void CounterMovement(Vector2 relativeVelocity)
    {
        if (Mathf.Abs(relativeVelocity.x) > threshold && Mathf.Abs(horizontalInput) < 0.05f || (relativeVelocity.x < -threshold && horizontalInput > 0) || (relativeVelocity.x > threshold && horizontalInput < 0))
        {
            rb.AddForce(playerOrientation.transform.right * -relativeVelocity.x * counterForce);
        }

        if (Mathf.Abs(relativeVelocity.y) > threshold && Mathf.Abs(verticalInput) < 0.05f || (relativeVelocity.y < -threshold && verticalInput > 0) || (relativeVelocity.y > threshold && verticalInput < 0))
        {
            rb.AddForce(playerOrientation.transform.forward * -relativeVelocity.y * counterForce);
        }
    }

    private Vector2 VelocityRelativeToCameraRotation()
    {
        float cameraRotation = playerOrientation.eulerAngles.y;
        float velocityAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(cameraRotation, velocityAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMagnitude = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMagnitude = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMagnitude, yMagnitude);
    }
}
