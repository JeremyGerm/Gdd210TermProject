
using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Cam : MonoBehaviour
{
    [SerializeField] private Camera rtcam;
    [SerializeField] private Transform photoContainer;

    private CameraToggle cameraToggle;
    private Inventory inventory;
    private bool showingPhotos = false;

    private Texture2D previewTexture;

    void Start()
    {
        cameraToggle = GetComponentInParent<CameraToggle>();
        inventory = GetComponentInParent<Inventory>();

        photoContainer.gameObject.SetActive(false);
    }

    void Update()
    {
        // Close the photo preview with Escape
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            showingPhotos = false;
            photoContainer.gameObject.SetActive(false);
        }

        // Take photos only when using the handheld camera
        if (cameraToggle != null &&
            cameraToggle.isUsingHandheld &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TakePicture();
        }
    }

    private void TakePicture()
    {
        // Fixed 16:9 aspect ratio
        int width = 1280;
        int height = 720;

        RenderTexture originalTarget = rtcam.targetTexture;
        RenderTexture previous = RenderTexture.active;

        RenderTexture rt = new RenderTexture(width, height, 16);
        rt.Create();

        Texture2D tex = new Texture2D(
            width,
            height,
            TextureFormat.RGB24,
            false
        );

        try
        {
            // Render the handheld camera into the temporary texture
            rtcam.targetTexture = rt;
            rtcam.Render();

            RenderTexture.active = rt;

            tex.ReadPixels(
                new Rect(0, 0, width, height),
                0,
                0
            );

            tex.Apply();

            // Save the photo
            string folder = Path.Combine(
                Application.persistentDataPath,
                "photos"
            );

            Directory.CreateDirectory(folder);

            string name = "photo_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") +
                ".png";

            string path = Path.Combine(folder, name);

            File.WriteAllBytes(path, tex.EncodeToPNG());

            // Add photo to inventory
            if (inventory != null)
            {
                inventory.AddPhoto(path);
            }

            // Show the latest photo
            UpdatePhotos(path);

            showingPhotos = true;
            photoContainer.gameObject.SetActive(true);

            Debug.Log("Photo saved: " + path);
        }
        finally
        {
            // Restore the camera's original rendering state
            rtcam.targetTexture = originalTarget;
            RenderTexture.active = previous;

            rt.Release();
            Destroy(rt);
            Destroy(tex);
        }
    }

    private void UpdatePhotos(string latestPhoto)
    {
        // Remove previously displayed photos
        foreach (Transform child in photoContainer)
        {
            Destroy(child.gameObject);
        }

        // Clean up the previous preview texture
        if (previewTexture != null)
        {
            Destroy(previewTexture);
        }

        // Load the newest photograph
        previewTexture = new Texture2D(2, 2);
        previewTexture.LoadImage(File.ReadAllBytes(latestPhoto));

        // Create the UI image
        GameObject photoObject = new GameObject(
            "LatestPhoto",
            typeof(RectTransform),
            typeof(RawImage)
        );

        photoObject.transform.SetParent(photoContainer, false);

        RawImage img = photoObject.GetComponent<RawImage>();
        img.texture = previewTexture;

        RectTransform rect =
            photoObject.GetComponent<RectTransform>();

        // Maintain the photograph's aspect ratio
        float displayWidth = 500f;
        float displayHeight =
            displayWidth * previewTexture.height / previewTexture.width;

        rect.sizeDelta = new Vector2(
            displayWidth,
            displayHeight
        );

        // Center the photograph
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = Vector2.zero;
        rect.localRotation = Quaternion.identity;
    }

    private void OnDestroy()
    {
        if (previewTexture != null)
        {
            Destroy(previewTexture);
        }
    }
}
