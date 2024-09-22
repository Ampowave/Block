using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    public int rows = 0;
    public int columns = 0;
    [SerializeField] Vector3 startPosition = new Vector3(0f,0f,0f); // In case we wanna change the usual way of generating grid. (not used)
    public GameObject GridBlockPrefab;
    public List<GridBlockScript> GridBlockList;
    public Dictionary<string, List<GridBlockScript>> columnRowDictionary;
    public List<string> ReadyToPopKeys;
    void Start()
    {
        columnRowDictionary = new Dictionary<string, List<GridBlockScript>>();
        for (int i = 0;  i < rows; i++)
        {
            columnRowDictionary.Add($"row{i}", new List<GridBlockScript>());
        }
        for (int i = 0; i < rows; i++)
        {
            columnRowDictionary.Add($"column{i}", new List<GridBlockScript>());
        }
        SpawnGrid();
    }   
    void SpawnGrid()
    {
        Vector3 currentPosition = new Vector3(0f,0f,0f) ;
        Vector3 nextPosition = GridBlockPrefab.transform.position;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                currentPosition = nextPosition;
                var newGO = Instantiate(GridBlockPrefab, currentPosition, GridBlockPrefab.transform.rotation, this.gameObject.transform).GetComponent<GridBlockScript>();
                //Set coordinates
                newGO.row = i;
                newGO.column = j;
                //Add to list
                GridBlockList.Add(newGO);
                columnRowDictionary[$"row{i}"].Add(newGO);
                columnRowDictionary[$"column{j}"].Add(newGO);
                nextPosition.x += 1f;
            }
            nextPosition.x = GridBlockPrefab.transform.position.x;
            nextPosition.y -= 1f;
        }
    }

    public void AddFilledRowsAndColumnsToPopList()
    {
        int result = 0;
        for (int i = 0; i < (columnRowDictionary.Count/2); i++)
        {
            for(int j = 0; j < columnRowDictionary[$"row{i}"].Count; j++)
            {
                if (true == columnRowDictionary[$"row{i}"][j].IsOccupied)
                    result++;
            }
            if (result == 10)
                ReadyToPopKeys.Add($"row{i}");

            result = 0;
            for (int j = 0; j < columnRowDictionary[$"column{i}"].Count; j++)
            {
                if (true == columnRowDictionary[$"column{i}"][j].IsOccupied)
                    result++;
            }
            
            if (result == 10)
                ReadyToPopKeys.Add($"column{i}");
            result = 0;
        }
    }

    public void PopFilledRowsAndColumns()
    {
        foreach(string key in  ReadyToPopKeys)
        {
            for(int i = 0; i < columnRowDictionary[key].Count; i++)
            {
                columnRowDictionary[key][i].IsOccupied = false;
            }          
            Debug.Log("popped");
        }
        ReadyToPopKeys.Clear();
    }
}
