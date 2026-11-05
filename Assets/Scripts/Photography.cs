using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Photography : MonoBehaviour
{
    private CameraToggle cameraToggle;

    public Camera handheldCamera;
    public RawImage rawImage;
    public RenderTexture renderTexture;

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
        //handheldCamera.targetTexture = renderTexture;

        handheldCamera.Render();

        //rawImage.texture = renderTexture;
    }
}
