using UnityEngine;
using UnityEngine.InputSystem;

public class Photography : MonoBehaviour
{
    private CameraToggle cameraToggle;

    void Start()
    {
        cameraToggle = GetComponent<CameraToggle>();
    }

    void Update()
    {
        if (cameraToggle.isUsingHandheld &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TakePicture();
        }
    }

    void TakePicture()
    {
        Debug.Log("Picture Taken!");
    }
}
