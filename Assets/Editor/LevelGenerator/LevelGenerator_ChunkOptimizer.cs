using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

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
        CellPacker newPack = new CellPacker(GetUtilityCell(1,1));
        newPack.PropergateToRight();
        newPack.PropergateToTop();
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
    public UtilityCell crystalizationPoint;
    public List<List<UtilityCell>> RightPropergatedCells = new List<List<UtilityCell>>();
    public List<List<UtilityCell>> UpPropergatedCells = new List<List<UtilityCell>>();

    public CellPacker(UtilityCell point) 
    {
        crystalizationPoint = point;
    }

    public void PropergateToRight() 
    {
        UtilityCell rightNeighbor = crystalizationPoint.GetNeighbor(1);
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
    public void PropergateToTop()
    {
        UtilityCell topNeighbor = crystalizationPoint.GetNeighbor(0);
        while (topNeighbor != null)
        {
            List<UtilityCell> rightStackingCells = new List<UtilityCell>();
            UpPropergatedCells.Add(rightStackingCells);
            UtilityCell rightNeighbor = topNeighbor;
            while (rightNeighbor != null)
            {
                rightStackingCells.Add(rightNeighbor);
                rightNeighbor = rightNeighbor.GetNeighbor(1);
            }
            topNeighbor = topNeighbor.GetNeighbor(0);

        }
    }

    public void CompareList() 
    {
        int potentialWidth = RightPropergatedCells.Count + 1;
        int potentialHeight = UpPropergatedCells.Count + 1;
       
        int longerListLength = Mathf.Max(RightPropergatedCells.Count, UpPropergatedCells.Count);
        int horizontalCounter;
        int verticalCounter;

        for (int i = 0; i < longerListLength; i++) 
        {
            horizontalCounter = i;
            verticalCounter = i;
            if (horizontalCounter > RightPropergatedCells.Count-1) 
            {
                horizontalCounter = RightPropergatedCells.Count-1;
            }
            if (verticalCounter > UpPropergatedCells.Count-1)
            {
                verticalCounter = UpPropergatedCells.Count - 1;
            }

            bool verticalPassed =  RightPropergatedCells[horizontalCounter].Count >= i + 2;
            bool horizontalPassed = UpPropergatedCells[verticalCounter].Count >= i + 2;

            if (!verticalPassed) 
                potentialHeight = Mathf.Min(potentialHeight, RightPropergatedCells[i].Count);
            if (!horizontalPassed )
                potentialWidth = Mathf.Min(potentialWidth, UpPropergatedCells[i].Count);



        }
        Debug.Log(potentialWidth );
        Debug.Log(potentialHeight );
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
