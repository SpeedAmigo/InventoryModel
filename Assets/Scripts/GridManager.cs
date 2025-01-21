using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows = 7;
    public int columns = 5;

    private void InitializeGridPositions()
    { 
        CellScript[] cells = GetComponentsInChildren<CellScript>();

        if (cells.Length != rows * columns)
        {
            Debug.LogError("Grid size mismatch!");
            return;
        }
        
        int index = 0;
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                cells[index].SetGridPosition(gridPosition);
                ItemManager.Instance.cellDictionary.Add(gridPosition, cells[index]);
                index++;
            }
        }
    }

    private void Start()
    {
        InitializeGridPositions();
    }
}
