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

    public Vector3[] GetScreenInWorldSpace(float depth) 
    {
        Vector3[] corners = new Vector3[4];
        if (Cam == null)
        {
            Debug.LogError("Camera reference is null!");
            return corners;
        }
        corners[0] = Cam.ScreenToWorldPoint(new Vector3(0, 0, depth));
        corners[1] = Cam.ScreenToWorldPoint(new Vector3(Cam.pixelWidth, 0, depth));
        corners[2] = Cam.ScreenToWorldPoint(new Vector3(Cam.pixelWidth, Cam.pixelHeight, depth));
        corners[3] = Cam.ScreenToWorldPoint(new Vector3(0, Cam.pixelHeight, depth));
        return corners;
    }

    public GameObject CreatePlaneLevel(string name, Mesh mesh, Material mat) 
    {
        GameObject g = new GameObject(name);
         MeshFilter mf = g.AddComponent<MeshFilter>();
         MeshRenderer mr = g.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mf.mesh = mesh;
        return g;
    }

    public void AdjustQuadDepth(Mesh target,float depth) 
    {
        target.vertices = GetScreenInWorldSpace(depth);
    }
    public Mesh CreateQuad(Vector3[] corners) 
    {
       
        Mesh m = new Mesh();

        m.vertices = new Vector3[] { corners[0], corners[1], corners[2], corners[3] };
        m.triangles = new int[] { 0, 3, 2, 2, 1, 0 };
        m.uv = new Vector2[] { Vector2.zero,Vector2.right,Vector2.one,Vector2.up };

        m.RecalculateNormals();
        m.RecalculateBounds();

        return m;
    }

}
