using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LevelGenerator_ChunkOptimizer
{
    private Cell[] _cells;
    private int _horizontal, _vertical;
    private UtilityCell[] _utilityCells;
    private List<CellPack> _cellPacks = new List<CellPack>();

    public LevelGenerator_ChunkOptimizer(Cell[] cells, int horizontal, int vertical) 
    {
        _cells = cells;
        _utilityCells = new UtilityCell[cells.Length];
        _horizontal = horizontal;
        _vertical = vertical;
    }
    public void SetupUtilityCell() 
    {
        for (int x = 0; x < _horizontal; x++)
            for (int y = 0; y < _vertical; y++)
                _utilityCells[x * _vertical + y] = _cells[x * _vertical + y].isActive ? new UtilityCell(_cells[x * _vertical + y]) : null;

        for (int x = 0; x < _horizontal; x++)
        {
            for (int y = 0; y < _vertical; y++)
            {
                UtilityCell currentUtilityCell = _utilityCells[x * _vertical + y];
                if (currentUtilityCell == null)
                    continue;
                currentUtilityCell.SetNeighbors((y + 1) < _vertical ? _utilityCells[x * _vertical + y + 1] : null, 0);
                currentUtilityCell.SetNeighbors((x + 1) < _horizontal ? _utilityCells[(x + 1) * _vertical + y] : null, 1);
                currentUtilityCell.SetNeighbors((y - 1) >= 0 ? _utilityCells[x * _vertical + y - 1] : null, 2);
                currentUtilityCell.SetNeighbors((x - 1) >= 0 ? _utilityCells[(x - 1) * _vertical + y] : null, 3);
            }
        }
    }

    public CellPack[] PackCells()
    {
        foreach (UtilityCell c in _utilityCells)
        {
            if (c == null)
                continue;

            CellPack newPack = new CellPacker(c, this).CreateCellPack();
            _cellPacks.Add(newPack);
        }
        return _cellPacks.ToArray();
    }
    public UtilityCell GetUtilityCell(int xIndex, int yIndex) 
    {
        return _utilityCells[xIndex * _vertical + yIndex];
    }
    public void SetUtilityCell(int xIndex, int yIndex, UtilityCell value)
    {
        _utilityCells[xIndex * _vertical + yIndex] = value;
    }
}

public class CellPack 
{
    private Vector2Int _dimension;
    private UtilityCell _origin;
    private UtilityCell[] _containedCells;

    public CellPack(Vector2Int dimension, UtilityCell origin, UtilityCell[] containedCells) 
    {
        _dimension = dimension;
        _origin = origin;
        _containedCells = containedCells;
    }
    public UtilityCell[] GetContainedCells() 
    {
        return _containedCells;
    }

    public Vector3 GetLocalScale() 
    {
        Vector3 size = _containedCells[0].GetCell().size;
        Vector3 packedCellSize = new Vector3(size.x * _dimension.x, size.y, size.z * _dimension.y);
        return packedCellSize;
    }

    public Vector3 GetWorldPosition() 
    {
        Vector3 size = _containedCells[0].GetCell().size;
        Vector3 packedCellSize = new Vector3(size.x * _dimension.x, 0, size.z * _dimension.y);
        return _origin.GetCell().position - new Vector3(_origin.GetCell().size.x / 2, 0, _origin.GetCell().size.z / 2) + packedCellSize / 2 + Vector3.up * size.y/2;
    }
}
public class CellPacker
{
    public UtilityCell CrystalizationPoint;
    private Vector2Int _c_pointIndex;
    public LevelGenerator_ChunkOptimizer Optimizer;
    public List<UtilityCell> RectPivots = new List<UtilityCell>();

    public CellPacker(UtilityCell point, LevelGenerator_ChunkOptimizer optimizer)
    {
        CrystalizationPoint = point;
        _c_pointIndex = new Vector2Int(CrystalizationPoint.GetCell().index.x, CrystalizationPoint.GetCell().index.y);
        Optimizer = optimizer;
    }

    public void FindingPotentialPivots() 
    {
        UtilityCell rightNeighbor = CrystalizationPoint;
        while (rightNeighbor != null)
        {
            UtilityCell topNeighbor = rightNeighbor;
            while (topNeighbor != null)
            {
                if (topNeighbor.GetNeighbor(0) == null)
                    RectPivots.Add(topNeighbor);
                topNeighbor = topNeighbor.GetNeighbor(0);
            }
            rightNeighbor = rightNeighbor.GetNeighbor(1);
        }
    }
 
    public UtilityCell[] DefineRectCorners() 
    {  
        for (int i = 0; i < RectPivots.Count; i++) 
            while (CellHasEmptyOnBotLeft(RectPivots[i]) && RectPivots[i].GetCell().index.x != 0)
                RectPivots[i] = RectPivots[i].GetNeighbor(2);
        return RectPivots.Distinct().ToArray();
    }
    private bool CellHasEmptyOnBotLeft(UtilityCell uc)
    {
        for (int i = uc.GetCell().index.x; i >= _c_pointIndex.x; i--)
            for (int j = uc.GetCell().index.y; j >= _c_pointIndex.y; j--)
                if (Optimizer.GetUtilityCell(i, j) == null)
                    return true;
        return false;
    }
    public UtilityCell[] GetContainedCells(UtilityCell uc) 
    {
        UtilityCell[] containedCells = new UtilityCell[(uc.GetCell().index.x - _c_pointIndex.x + 1) * (uc.GetCell().index.y - _c_pointIndex.y+ 1)];
        int index = 0;
        for (int i = uc.GetCell().index.x; i >= _c_pointIndex.x; i--)
        {
            for (int j = uc.GetCell().index.y; j >= _c_pointIndex.y; j--)
            {
                containedCells[index] =  Optimizer.GetUtilityCell(i, j);
                Optimizer.SetUtilityCell(i, j, null);
                index++;
            }
        }
        return containedCells;
    }
    private Vector2Int GetDimension(UtilityCell c) 
    {
        return new Vector2Int(c.GetCell().index.x + 1, c.GetCell().index.y + 1) - _c_pointIndex;
    }
    public UtilityCell GetLargestCorner(UtilityCell[] corners) 
    {
        float largestArea = 0;
        UtilityCell largestCell = null;
        foreach (UtilityCell c in corners) 
        {
            Vector2Int dimension = GetDimension(c);
            float area = dimension.x * dimension.y;
            if (area >= largestArea)
            {
                largestCell = c;
                largestArea = area;
            }
        }
        return largestCell;
    }
    public CellPack CreateCellPack() 
    {
        FindingPotentialPivots();
        UtilityCell[] corners = DefineRectCorners();
        UtilityCell largest = GetLargestCorner(corners);
        UtilityCell[] containedCells = GetContainedCells(largest);
        return new CellPack(GetDimension(largest), CrystalizationPoint, containedCells);
    }

}
public class UtilityCell
{
    private Cell _cell;
    // 0,1,2,3 => Top,Right,Bottom,Left
    private UtilityCell[] neighbors;

    public UtilityCell(Cell cell)
    {
        _cell = cell;
        neighbors = new UtilityCell[4];
    }

    public void SetNeighbors(UtilityCell cell, int num)
    {
        neighbors[num] = cell;
    }

    public UtilityCell GetNeighbor(int num) 
    {
        return neighbors[num];
    }
    public UtilityCell[] GetAllNeighbor() 
    {
        return neighbors;
    }
    public Cell GetCell() 
    {
        return _cell;
    }
}
