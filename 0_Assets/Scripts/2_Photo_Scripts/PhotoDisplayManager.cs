using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhotoDisplayManager : MonoBehaviour
{
    public Transform photoContainer; // 照片父物体 (UI Canvas 内)
    public GameObject photoPrefab;   // 照片预制体

    private GameObject lastPhotoUI;  // 最后一张照片的 UI
    private List<GameObject> photoUIList = new List<GameObject>();

    public void CaptureAndDisplay(Camera photoCamera)
    {
        if (photoCamera == null)
        {
            Debug.LogError("Photo camera is not assigned!");
            return;
        }

        // 截图逻辑
        RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
        photoCamera.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        photoCamera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        screenshot.Apply();

        photoCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        CreatePhotoUI(screenshot);
    }

    private void CreatePhotoUI(Texture2D screenshot)
    {
        GameObject photoUI = Instantiate(photoPrefab, photoContainer);
        RawImage rawImage = photoUI.GetComponentInChildren<RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = screenshot;
        }
        lastPhotoUI = photoUI;
        photoUIList.Add(photoUI);
    }

    public void DisplayPhoto(Texture2D photo)
    {
        if (photo == null)
        {
            Debug.LogError("No photo provided for display!");
            return;
        }

        if (lastPhotoUI == null)
        {
            CreatePhotoUI(photo);
        }
        else
        {
            RawImage rawImage = lastPhotoUI.GetComponentInChildren<RawImage>();
            if (rawImage != null)
            {
                rawImage.texture = photo;
            }
        }
    }

    public Texture2D GetLastPhoto()
    {
        RawImage rawImage = lastPhotoUI?.GetComponentInChildren<RawImage>();
        return rawImage != null ? rawImage.texture as Texture2D : null;
    }

    public void ReplaceLastPhoto(Texture2D newTexture)
    {
        RawImage rawImage = lastPhotoUI?.GetComponentInChildren<RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = newTexture;
        }
    }

    public void HideLastPhoto()
    {
        if (lastPhotoUI != null)
        {
            lastPhotoUI.SetActive(false);
        }
    }

    public void ClearAllPhotos()
    {
        foreach (var photoUI in photoUIList)
        {
            Destroy(photoUI);
        }
        photoUIList.Clear();
        lastPhotoUI = null;
    }

    public bool HasPhotos()
    {
        return photoUIList.Count > 0;
    }
}
