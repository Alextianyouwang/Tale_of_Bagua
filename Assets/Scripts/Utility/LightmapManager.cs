using System.Collections.Generic;
using UnityEngine;

public class LightmapManager : MonoBehaviour
{
    public Texture2D[] darkLightmapDir, darkLightmapColor;
    public Texture2D[] brightLightmapDir, brightLightmapColor;

    private LightmapData[] darkLightmap, brightLightmap;

    void Start()
    {
        List<LightmapData> dlightmap = new List<LightmapData>();

        for (int i = 0; i < darkLightmapDir.Length; i++)
        {
            LightmapData lmdata = new LightmapData();

            lmdata.lightmapDir = darkLightmapDir[i];
            lmdata.lightmapColor = darkLightmapColor[i];

            dlightmap.Add(lmdata);
        }

        darkLightmap = dlightmap.ToArray();

        List<LightmapData> blightmap = new List<LightmapData>();

        for (int i = 0; i < brightLightmapDir.Length; i++)
        {
            LightmapData lmdata = new LightmapData();

            lmdata.lightmapDir = brightLightmapDir[i];
            lmdata.lightmapColor = brightLightmapColor[i];

            blightmap.Add(lmdata);
        }

        brightLightmap = blightmap.ToArray();

      
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnLight1Switched();
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            OnLight2Switched();
    }

    private void OnLight1Switched()
    {
        LightmapSettings.lightmaps = darkLightmap;
    }

    private void OnLight2Switched()
    {
        LightmapSettings.lightmaps = brightLightmap;
    }

}