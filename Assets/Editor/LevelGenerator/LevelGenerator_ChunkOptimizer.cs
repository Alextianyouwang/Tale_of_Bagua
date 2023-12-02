using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class LevelGenerator_ChunkOptimizer
{
    private Cell[] _cells;
    private int _horizontal, _vertical;
    private UtilityCell[] _utilityCells;

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
                UtilityCell topNeighbor = (y + 1) < _vertical? _utilityCells[x * _vertical + y + 1] : null;
                UtilityCell rightNeighbor = (x + 1) < _horizontal ? _utilityCells[(x+1) * _vertical + y] : null;
                UtilityCell botNeighbor = (y - 1) >= 0 ? _utilityCells[x * _vertical + y - 1] : null;
                UtilityCell leftNeighbor = (x - 1) >= 0 ? _utilityCells[(x - 1) * _vertical + y] : null;
                if (topNeighbor != null)
                    currentUtilityCell.SetNeighbors(topNeighbor.GetCell().isActive ? topNeighbor : null, 0);
                if (rightNeighbor != null)
                    currentUtilityCell.SetNeighbors(rightNeighbor.GetCell().isActive ? rightNeighbor : null, 1);
                if (botNeighbor != null)
                    currentUtilityCell.SetNeighbors(botNeighbor.GetCell().isActive ? botNeighbor : null, 2);
                if (leftNeighbor != null)
                    currentUtilityCell.SetNeighbors(leftNeighbor.GetCell().isActive ? leftNeighbor : null, 3);
              
            }
        }
    }

    public void PackCells() 
    {
        foreach (UtilityCell c in _utilityCells) 
        {
            if (c == null)
                continue;
            if (c.GetVisitedState())
                continue;

            
        }
        CellPacker newPack = new CellPacker(GetUtilityCell(0,0),this);
        newPack.PropergateToRight();
        UtilityCell[] corners =  newPack.DefineRectCorners();
        foreach (UtilityCell c in corners) 
        {
            Debug.Log(c.GetCell().index);
        }
        //newPack.CompareList();
      
    }

    public UtilityCell[] GetAllUtilityCells() 
    {
        return _utilityCells;
    }
    public UtilityCell GetUtilityCell(int xIndex, int yIndex) 
    {
        return _utilityCells[xIndex * _vertical + yIndex];
    }
}

public class CellPacker
{
    public UtilityCell CrystalizationPoint;
    public LevelGenerator_ChunkOptimizer Optimizer;
    public List<List<UtilityCell>> RightPropergatedCells = new List<List<UtilityCell>>();

    public CellPacker(UtilityCell point, LevelGenerator_ChunkOptimizer optimizer)
    {
        CrystalizationPoint = point;
        Optimizer = optimizer;
    }

    public void PropergateToRight() 
    {
        UtilityCell rightNeighbor = CrystalizationPoint;
        while (rightNeighbor != null)
        {
            List<UtilityCell> topStackingCells = new List<UtilityCell>();
            RightPropergatedCells.Add(topStackingCells);
            UtilityCell topNeighbor = rightNeighbor;
            while (topNeighbor != null)
            {
                topStackingCells.Add(topNeighbor);
                topNeighbor = topNeighbor.GetNeighbor(0);
            }
            rightNeighbor = rightNeighbor.GetNeighbor(1);
           
        }
    }
 
    public UtilityCell[] DefineRectCorners() 
    {
        List<UtilityCell> cornerCells = new List<UtilityCell>();
        foreach (List<UtilityCell> cList in RightPropergatedCells)
            cornerCells.Add(cList[cList.Count-1]);
                    
        
        for (int i = 0; i < cornerCells.Count; i++) 
        {
            if (cornerCells[i].GetCell().index.x == 0)
                continue;
            
            while (CellHasEmptyOnBotLeft(cornerCells[i]))
                cornerCells[i] = cornerCells[i].GetNeighbor(2);
            while (cornerCells[i].GetNeighbor(1) != null)
                cornerCells[i] = cornerCells[i].GetNeighbor(1);

        }
        return cornerCells.Distinct().ToArray();

    }
    private bool CellHasEmptyOnBotLeft(UtilityCell uc) 
    {
        
        bool hasEmptyOnLeft = false;
        int xIndex = (int)uc.GetCell().index.x;
        int yIndex = (int)uc.GetCell().index.y;
        for (int i = xIndex; i >= 0; i--) 
        {
            for (int j = yIndex; j >= 0; j--) 
            {
                 UtilityCell current = Optimizer.GetUtilityCell(i, j);
                if (current == null)
                {
                    hasEmptyOnLeft = true;
                    continue;
                }
            }
        }
        
        return hasEmptyOnLeft;
    }
}
public class UtilityCell
{
    private Cell _cell;
    private bool hasBeenVisited = false;
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

    public void SetVisitedState(bool value) 
    {
        hasBeenVisited = value;
    }
    public bool GetVisitedState() 
    {
        return hasBeenVisited;
    }
    public Cell GetCell() 
    {
        return _cell;
    }
}
