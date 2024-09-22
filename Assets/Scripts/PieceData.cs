using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;

[System.Serializable]
public class Row
{
    public bool[] Column;
    private int _rowSize;

    public Row() { }

    public Row(int columns)
    {
        CreateRow(columns);
    }

    public void CreateRow(int columns)
    {
        _rowSize = columns;
        Column = new bool[columns];
        SetRowToFalse();
    }

    public void SetRowToFalse()
    {
        for (int i = 0; i < _rowSize; i++)
        {
            Column[i] = false;
        }
    }
}


[CreateAssetMenu(fileName = "Piece", menuName = "Piece")]
[System.Serializable]
public class PieceData : ScriptableObject
{

    [SerializeField] private PieceType _type;
    public PieceType PieceType => _type;

    public int Rows = 0;    

    public int Columns = 0;
    public Row[] PieceMap;

    public void CreateNewMap()
    {
        PieceMap = new Row[Rows];
        for (int i = 0; i < Rows; i++)
        {
            PieceMap[i] = new Row(Columns);
        }
    }

    public void ClearMap()
    {
        for(int i = 0; i < Rows; i++)
        {
            PieceMap[i].SetRowToFalse();
        }
    }

}

