using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CaptureLevel
{
    public Camera Cam { get; set; }

    public void CaptureCameraToTexture(LayerMask cullMask, string path, string name)
    {
        if (Cam == null)
        {
            Debug.LogError("Camera reference is null!");
            return;
        }
        LayerMask tempCullMask = Cam.cullingMask;

        Cam.targetTexture = new RenderTexture(Cam.pixelWidth, Cam.pixelHeight, 24);
        RenderTexture.active = Cam.targetTexture;
        Cam.cullingMask = cullMask;
        Cam.Render();

        Texture2D image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToJPG();
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(Application.dataPath, path,name + ".jpg"), bytes);

        Debug.Log($"Saved camera capture to: {path}");

        Cam.cullingMask = tempCullMask;
        Cam.targetTexture = null;

        AssetDatabase.Refresh();

    }

}
