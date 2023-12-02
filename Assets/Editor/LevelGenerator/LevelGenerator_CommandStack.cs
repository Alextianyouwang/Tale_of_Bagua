using UnityEngine;
using System.Collections.Generic;
public interface ICommand 
{
    public void Undo();
    public void Redo();
}
public class LevelGenerator_MouseDragAction : ICommand
{
    public List<Cell> affectedCells = new List<Cell>();
    public List<bool> originalState= new List<bool>();
    public List<bool> newState= new List<bool>();

    public LevelGenerator_MouseDragAction() { }
    public void Execute(Cell currentCell, bool value) 
    {
        if (!affectedCells.Contains(currentCell))
        {
            affectedCells.Add(currentCell);
            originalState.Add(currentCell.isActive);
            newState.Add(value);
        }
        currentCell.isActive = value;

    }
    public bool ChangeMade() 
    {
        int index = 0;
        foreach (bool original in originalState) 
        {
            if (newState[index] != original)
                return true;
            index++;
        }
        return false;
    }

    public void Undo() 
    {
        for (int i = 0; i < affectedCells.Count; i++) 
            affectedCells[i].isActive = originalState[i];
          
    }
    public void Redo()
    {
        for (int i = 0; i < affectedCells.Count; i++)
            affectedCells[i].isActive = newState[i];
    }
}
public class LevelGenerator_CommandStack 
{
  public Stack<ICommand> UndoStack = new Stack<ICommand>();
  public Stack<ICommand> RedoStack = new Stack<ICommand>();

    public void RecordAction(LevelGenerator_MouseDragAction action) 
    {
        UndoStack.Push(action);
    }

    public void UndoAction() 
    {
        if (UndoStack.Count >= 1) 
        {
            ICommand current = UndoStack.Pop();
            current.Undo();
            RedoStack.Push(current) ;
        }
        else 
        {
            Debug.LogWarning("There is no actions to Undo");
        }
    }

    public void RedoAction() 
    {
        if (RedoStack.Count >= 1) 
        {
            ICommand current = RedoStack.Pop();
            current.Redo();
            UndoStack.Push(current);
        }
        else
        {
            Debug.LogWarning("There is no actions to Redo");
        }

    }

    public void ClearRedoStack() 
    {
        RedoStack.Clear();
    }
}
