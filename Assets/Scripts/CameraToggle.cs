
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraToggle : MonoBehaviour
{
    public Camera playerCamera;
    public Camera handheldCamera;

    private bool isUsingHandheld = false;

    void Start()
    {
        // Start in third-person mode
        playerCamera.gameObject.SetActive(true);
        handheldCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        // Press C to toggle cameras
        if (Keyboard.current != null &&
            Keyboard.current.cKey.wasPressedThisFrame)
        {
            isUsingHandheld = !isUsingHandheld;

            playerCamera.gameObject.SetActive(!isUsingHandheld);
            handheldCamera.gameObject.SetActive(isUsingHandheld);
        }
    }
}
