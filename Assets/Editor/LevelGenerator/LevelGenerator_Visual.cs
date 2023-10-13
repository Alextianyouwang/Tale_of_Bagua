
using UnityEngine;
using System.Linq;

public class LevelGenerator_Visual 
{
    private ComputeShader _levelVisual_cs;
    public RenderTexture _levelVisual_tex { get; private set; }
    private ComputeBuffer _levelVisual_buffer;

    public void PrepareVisualSetup(int texWidth, int texHeight)
    {
        _levelVisual_cs = EditorUtil.GetObject<ComputeShader>("CS_LevelVisual.compute");
        _levelVisual_tex = RenderTexture.GetTemporary(texWidth, texHeight, 0, RenderTextureFormat.ARGB32);
        _levelVisual_tex.filterMode = FilterMode.Point;
        _levelVisual_tex.enableRandomWrite = true;
        _levelVisual_tex.Create();

        _levelVisual_cs.SetTexture(0, "Result", _levelVisual_tex);
        _levelVisual_cs.SetVector("_screenParam", new Vector4(texWidth, texHeight, 0, 0));
    }

    public void UpdateVisualSetup(Cell[] cells, int stride, int texWidth, int texHeight)
    {
        if (_levelVisual_buffer != null)
            _levelVisual_buffer.Dispose();
        _levelVisual_buffer = new ComputeBuffer(cells.Length, stride);
        _levelVisual_buffer.SetData(cells.Select(x => x.cellStruct).ToArray(), 0, 0, cells.Length);
        _levelVisual_cs.SetInt("_cellCount", cells.Length);
        _levelVisual_cs.SetBuffer(0, "_CellBuffer", _levelVisual_buffer);
        _levelVisual_cs.Dispatch(0, (texWidth / 10), (texHeight / 10), 1);
    }

    public void RemoveVisualSetup()
    {
        if (_levelVisual_buffer != null)
            _levelVisual_buffer.Dispose();
        _levelVisual_tex.Release();

    }
}
