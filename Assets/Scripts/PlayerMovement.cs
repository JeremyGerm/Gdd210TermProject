
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;

    [Header("Third Person Camera")]
    public Camera playerCamera;
    public float cameraDistance = 4f;
    public float cameraHeight = 1.5f;

    public Camera handheldCamera;

    private CharacterController controller;
    private Vector3 velocity;

    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        if (Keyboard.current == null)
            return;

        float x = 0f;
        float z = 0f;

        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.wKey.isPressed) z += 1f;
        if (Keyboard.current.sKey.isPressed) z -= 1f;

        Vector3 move = transform.right * x + transform.forward * z;
        move = Vector3.ClampMagnitude(move, 1f);

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Keep the player grounded
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the third-person camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
    }

    
    void LateUpdate()
    {
        if (playerCamera == null)
            return;

        // Third-person camera follows behind the player
        Vector3 pivot = transform.position + Vector3.up * cameraHeight;

        Quaternion cameraRotation = Quaternion.Euler(
            xRotation,
            transform.eulerAngles.y,
            0f
        );

        playerCamera.transform.position =
            pivot + cameraRotation *
            new Vector3(0f, 0f, -cameraDistance);

        playerCamera.transform.rotation = cameraRotation;

        // Handheld camera vertical mouse look
        if (handheldCamera != null)
        {
            handheldCamera.transform.localRotation =
                Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

}
