
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhotoItem
{
    public string filePath;
    public bool isPlaced;

    public PhotoItem(string path)
    {
        filePath = path;
        isPlaced = false;
    }
}

public class Inventory : MonoBehaviour
{
    public List<PhotoItem> photos = new List<PhotoItem>();

    public void AddPhoto(string path)
    {
        photos.Add(new PhotoItem(path));
        Debug.Log("Photo added to inventory: " + path);
    }

    public void PlacePhoto(PhotoItem photo)
    {
        if (photo == null || photo.isPlaced)
            return;

        photo.isPlaced = true;
        Debug.Log("Photo placed on map!");
    }
}

