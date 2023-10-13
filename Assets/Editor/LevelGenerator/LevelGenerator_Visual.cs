
using UnityEngine;
using System.Linq;
using static Cell;

public class LevelGenerator_Visual
{
    private ComputeShader _levelVisual_cs;
    public RenderTexture _levelVisual_tex { get; private set; }
    private ComputeBuffer _levelVisual_buffer;
    private ComputeBuffer _levelFindClosest_buffer;

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

    public void UpdateVisualSetup(Cell[] cells, int stride)
    {
        if (_levelVisual_buffer != null)
            _levelVisual_buffer.Dispose();
        _levelVisual_buffer = new ComputeBuffer(cells.Length, stride);
  
        _levelVisual_cs.SetInt("_cellCount", cells.Length);
       
    }

    public void UpdateVisualPerFrame(Cell[] cells, int texWidth, int texHeight) 
    {
        _levelVisual_buffer.SetData(cells.Select(x => x.cellStruct).ToArray(), 0, 0, cells.Length);
        _levelVisual_cs.SetBuffer(0, "_CellBuffer", _levelVisual_buffer);
        _levelVisual_cs.Dispatch(0, Mathf.CeilToInt(texWidth / 10), Mathf.CeilToInt(texHeight / 10), 1);
    }

    public void UpdateSearchClosestSetup(int count)
    {
        if (_levelFindClosest_buffer != null)
            _levelFindClosest_buffer.Dispose();
        _levelFindClosest_buffer = new ComputeBuffer(count, sizeof(float) * 4);
    }

    public void TogglePaintAndErase(float value) 
    {
        _levelVisual_cs.SetFloat("_erase", value);
    }
    public void UpdateSearchClosestPerFrame(Vector3 cursorPos, Cell[] cells)
    {
        _levelFindClosest_buffer.SetData(cells.Select(x => x.cellStruct_WS).ToArray(), 0, 0, cells.Length);
        _levelVisual_cs.SetBuffer(1, "_CellBuffer_WS", _levelFindClosest_buffer);
        _levelVisual_cs.SetVector("_cursorPos", cursorPos);
        _levelVisual_cs.Dispatch(1, Mathf.CeilToInt(cells.Length / 32), 1, 1);

        CellWorldSpace[] data = new CellWorldSpace[cells.Length];
        _levelFindClosest_buffer.GetData(data);

        for (int i = 0; i < cells.Length; i++)
            cells[i].isActive = data[i].isActive == 0 ? false : true;
    }
    public void RemoveSearchClosestSetup() 
    {
        if (_levelFindClosest_buffer != null)
            _levelFindClosest_buffer.Dispose();
    }  
    public void RemoveVisualSetup()
    {
        if (_levelVisual_buffer != null)
            _levelVisual_buffer.Dispose();
        _levelVisual_tex.Release();

        RemoveSearchClosestSetup();

    }
}
