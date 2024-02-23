using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralDrawer : MonoBehaviour
{
    public Texture2D SourceTexture;
    public MeshRenderer GroundRenderer;
    public ComputeShader Painter;
    private ComputeBuffer DataBuffer;
    private UnitData[] _unitDataArray;
     
    public struct UnitData 
    {
        public Vector3 pixelColor;
        public Vector3 pixelPos_WS;
        public Vector2 UV;

        public UnitData(Vector3 _pixelColor, Vector3 _pixelPos_WS, Vector2 _UV) 
        {
            pixelColor = _pixelColor;
            pixelPos_WS = _pixelPos_WS;
            UV = _UV;
        }
    }
    void GetDataFromTexture() 
    {
        if (SourceTexture == null) return;
        if (GroundRenderer == null) return;
        _unitDataArray = new UnitData[SourceTexture.width * SourceTexture.height];
        Vector3 textureOffset = GroundRenderer.bounds.center - GroundRenderer.bounds.extents;
        Vector2 textureSize = new Vector2(GroundRenderer.bounds.size.x, GroundRenderer.bounds.size.z);
        for (int x = 0; x < SourceTexture.width; x++) 
        {
            for (int y = 0; y < SourceTexture.height; y++) 
            {
                Vector2 UV = new Vector2((float)(x / SourceTexture.width), (float)(y /SourceTexture.height));
                Vector2 absolutePos = new Vector2(Mathf.Lerp(0, textureSize.x, UV.x), Mathf.Lerp(0, textureSize.y, UV.y));
                Vector3 pixelWorldPos = textureOffset + new Vector3(absolutePos.x, 0, absolutePos.y);
                Color pixelCol = SourceTexture.GetPixel(x, y);
                Vector3 color = new Vector3(pixelCol.r, pixelCol.g, pixelCol.b);
                _unitDataArray[x * SourceTexture.height + y] = new UnitData(color,pixelWorldPos,UV);

            }
        }
    }
    void SetupComputeShader() 
    {
        GetDataFromTexture();
        if (Painter == null) return;
        DataBuffer = new ComputeBuffer(SourceTexture.width * SourceTexture.height, sizeof(float) * 8);
        DataBuffer.SetData(_unitDataArray);
        Painter.SetInt("_width", SourceTexture.width);
        Painter.SetInt("_height", SourceTexture.height);
        Painter.SetBuffer(0, "UnitDataBuffer", DataBuffer);
        Painter.Dispatch(0,Mathf.CeilToInt(SourceTexture.width / 8), Mathf.CeilToInt(SourceTexture.height / 8), 1);

    }
    private void OnDrawGizmos()
    {
        if (_unitDataArray == null)
            return;
    
    }
    private void OnDisable()
    {
        DataBuffer.Release();
    }
    void Start()
    {
        SetupComputeShader();
    }

    void Update()
    {
        
    }
}
