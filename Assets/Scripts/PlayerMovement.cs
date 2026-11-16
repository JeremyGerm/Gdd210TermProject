
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    public Camera playerCamera;
    public Camera handheldCamera;

    public float mouseSensitivity = 50f;

    public float cameraDistance = 4f;
    public float cameraHeight = 1.5f;

    private float xRotation = 0f;
    private InputAction lookAction;
    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        lookAction = new InputAction(
            type: InputActionType.Value,
            binding: "<Mouse>/delta"
        );
        lookAction.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // WASD movement
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.aKey.isPressed) x = -1f;
        if (Keyboard.current.dKey.isPressed) x = 1f;
        if (Keyboard.current.wKey.isPressed) z = 1f;
        if (Keyboard.current.sKey.isPressed) x = 0f; // overwritten below
        if (Keyboard.current.sKey.isPressed) z = -1f;

        Vector3 move =
            transform.right * x +
            transform.forward * z;

        controller.Move(
            move.normalized * moveSpeed * Time.deltaTime
        );

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        // Mouse look
        Vector2 mouseDelta =
            lookAction.ReadValue<Vector2>() *
            mouseSensitivity * Time.deltaTime;

        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        // Camera pivot near player's head
        Vector3 pivot = transform.position +
                        Vector3.up * cameraHeight;

        Quaternion cameraRotation =
            Quaternion.Euler(xRotation, transform.eulerAngles.y, 0f);

        // Orbit camera around player
        playerCamera.transform.position =
            pivot + cameraRotation * new Vector3(0f, 0f, -cameraDistance);

        playerCamera.transform.rotation = cameraRotation;

        // Handheld camera looks in the same direction
        // handheldCamera.transform.rotation =
        // Quaternion.Euler(xRotation, transform.eulerAngles.y, 0f);
    }

    private void OnDestroy()
    {
        lookAction?.Disable();
        lookAction?.Dispose();
    }
}
