using System;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Cam : MonoBehaviour
{
    [SerializeField] private Camera rtcam;
    [SerializeField] private Transform photoContainer;

    private CameraToggle cameraToggle;
    private bool showingPhotos = false;

    void Start()
    {
        cameraToggle = GetComponentInParent<CameraToggle>();

        photoContainer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (cameraToggle != null &&
            cameraToggle.isUsingHandheld &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            int width = 1280;
            int height = Mathf.RoundToInt(width / rtcam.aspect);

            RenderTexture rt = new RenderTexture(width, height, 16);
            rtcam.targetTexture = rt;
            rtcam.Render();

            Texture2D tex = new Texture2D(rt.width, rt.height);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/photos"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/photos");
            }
            string name = $"photo_{DateTime.Now.ToLongTimeString().Replace(":", "")}";
            name = new string(name.Where(x => char.IsWhiteSpace(x) || char.IsLetterOrDigit(x)).ToArray()) + ".png";
            System.IO.File.WriteAllBytes(Application.persistentDataPath + $"/photos/{name}", bytes);

            if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
            {
            showingPhotos = false;
            photoContainer.gameObject.SetActive(false);
            }

            UpdatePhotos();

            showingPhotos = true;
            photoContainer.gameObject.SetActive(true);
        }
    }

    

private void UpdatePhotos()
    {
        // Remove previously displayed photos
        foreach (Transform child in photoContainer)
        {
            Destroy(child.gameObject);
        }

        string folder = Application.persistentDataPath + "/photos";

        if (!System.IO.Directory.Exists(folder))
        {
            return;
        }

        // Find the most recently saved photo
        string[] photos = System.IO.Directory.GetFiles(folder, "*.png");

        if (photos.Length == 0)
        {
            return;
        }

        string latestPhoto = photos
            .OrderByDescending(f => System.IO.File.GetLastWriteTimeUtc(f))
            .First();

        // Load the latest photo
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(System.IO.File.ReadAllBytes(latestPhoto));

        // Create the UI image
        GameObject photoObject = new GameObject(
            "LatestPhoto",
            typeof(RectTransform),
            typeof(RawImage)
        );

        photoObject.transform.SetParent(photoContainer, false);

        RawImage img = photoObject.GetComponent<RawImage>();
        img.texture = texture;

        RectTransform rect = photoObject.GetComponent<RectTransform>();

        // Maintain the photograph's aspect ratio
        float displayWidth = 500f;
        float displayHeight = displayWidth * texture.height / texture.width;

        rect.sizeDelta = new Vector2(displayWidth, displayHeight);

        // Center the photo in its container
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.localRotation = Quaternion.identity;
    }

}