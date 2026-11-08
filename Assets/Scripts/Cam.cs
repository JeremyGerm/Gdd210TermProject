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

    void Start()
    {
        cameraToggle = GetComponentInParent<CameraToggle>();
    }

    private void Update()
    {
        if (cameraToggle != null &&
            cameraToggle.isUsingHandheld &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            RenderTexture rt = new RenderTexture(1024, 1024, 16);
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

            UpdatePhotos();
        }
    }

    private void UpdatePhotos()
    {
        string[] photos = System.IO.Directory.GetFiles(Application.persistentDataPath + "/photos");
        int i = 0;
        foreach (string photo in photos)
        {
            Texture2D texture = new Texture2D(1024, 1024);
            RawImage img = Instantiate(new GameObject(), photoContainer).AddComponent<RawImage>();
            texture.LoadImage(System.IO.File.ReadAllBytes(photo));
            img.texture = texture;
            img.GetComponent<RectTransform>().position += new Vector3(i * 100, UnityEngine.Random.Range(-10, 10));
            img.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
            i++;
        }
    }
}