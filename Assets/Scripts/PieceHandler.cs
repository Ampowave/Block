using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class PieceHandler : MonoBehaviour
{
    private GameObject _tmpBlock;
    private Transform _transform;

    public List<PieceBlockScript> PieceBlocks;//PieceBlockScript
    public ContainerScript Container;
    public Vector3 SelectedPieceScale = new Vector3(1f, 1f);
    public GridScript Grid;
    public PieceGeneratorScript PieceGeneratorScript;
    public PieceData PieceData;
   
    public void PieceOnMouseDown()
    {
        _transform = this.GetComponent<Transform>();
        _transform.localScale = SelectedPieceScale;
        Debug.Log("clicked");
    }
    public void PieceOnMouseDrag()
    {
        _transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x , Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        FindClosestForFirstBlock();
        MakeClosestHover();
    }

    public void PieceOnMouseUp()
    {
        if (CheckIfAllClosestAreViable())
        {
            SetPiece();
            _transform.position = Container.transform.position;

            Grid.AddFilledRowsAndColumnsToPopList();
            Grid.PopFilledRowsAndColumns();

            if (PieceGeneratorScript.CheckAllContainersEmpty())
                PieceGeneratorScript.GeneratePieces();

            PieceGeneratorScript.CheckLoseCondition();
        }
        else
        {
            _transform.position = Container.transform.position;
            _transform.localScale = PieceGeneratorScript.IdlePieceScale;
        }
        Debug.Log("released");
        for (int i = 0; i < Grid.GridBlockList.Count; i++)
        {
            Grid.GridBlockList[i].IsHover = false;
        }
    }

    private void FindClosestForFirstBlock()
    {
            for (int i = 0; i < Grid.GridBlockList.Count; i++)
            {
                //checking if it's not too far
                if (Vector3.Distance(PieceBlocks[0].transform.position, Grid.GridBlockList[i].gameObject.GetComponent<Transform>().position) < 0.7f)
                {
                    if (PieceBlocks[0].ClosestBlock == null)
                    {
                        PieceBlocks[0].ClosestBlock = Grid.GridBlockList[i];
                    }
                    else
                    {
                        //checking if the new grid block is closer than the one we already have a reference to
                        if (Vector3.Distance(PieceBlocks[0].transform.position, Grid.GridBlockList[i].gameObject.GetComponent<Transform>().position) < Vector3.Distance(PieceBlocks[0].transform.position, PieceBlocks[0].ClosestBlock.transform.position))
                        {
                            PieceBlocks[0].ClosestBlock = Grid.GridBlockList[i];
                        }
                    }
                }
                
            }
            if (PieceBlocks[0].ClosestBlock != null && Vector3.Distance(PieceBlocks[0].transform.position, PieceBlocks[0].ClosestBlock.gameObject.GetComponent<Transform>().position) >= 1f)
                PieceBlocks[0].ClosestBlock = null;
    }

    public void MakeClosestHover()
    {
        if (CheckIfAllClosestAreViable())
        {
            for (int i = 0; i < Grid.GridBlockList.Count; i++)
            {
                bool result = false;
                foreach (var block in PieceBlocks)
                {
                     result |= (Grid.columnRowDictionary[$"row{block.Row + PieceBlocks[0].ClosestBlock.row - PieceBlocks[0].Row}"][block.Column + PieceBlocks[0].ClosestBlock.column - PieceBlocks[0].Column] == Grid.GridBlockList[i]);
                }
                Grid.GridBlockList[i].IsHover = result;
            }
        }
        else
        {
            for (int i = 0; i < Grid.GridBlockList.Count; i++)
            {
                Grid.GridBlockList[i].IsHover = false;
            }
        }

        //TODO make non viables not hover
    }

    private bool CheckIfAllClosestAreViable()
    {
        bool result = true;
        if (PieceBlocks[0].ClosestBlock == null)
            return false;
        foreach (var block in PieceBlocks)
        {
            if (block.Row + PieceBlocks[0].ClosestBlock.row - PieceBlocks[0].Row > 9 || block.Row + PieceBlocks[0].ClosestBlock.row - PieceBlocks[0].Row < 0 || block.Column + PieceBlocks[0].ClosestBlock.column - PieceBlocks[0].Column > 9 || block.Column + PieceBlocks[0].ClosestBlock.column - PieceBlocks[0].Column < 0)
                return false;
            result &= !Grid.columnRowDictionary[$"row{block.Row + PieceBlocks[0].ClosestBlock.row - PieceBlocks[0].Row}"][block.Column + PieceBlocks[0].ClosestBlock.column - PieceBlocks[0].Column].IsOccupied;
        }
        return result;
    }

    public void SetPiece()
    {
        foreach (var block in PieceBlocks)
        {
            //block.ClosestBlock.IsOccupied = true;
            //Destroy(block.gameObject);
            Grid.columnRowDictionary[$"row{block.Row + PieceBlocks[0].ClosestBlock.row - PieceBlocks[0].Row}"][block.Column + PieceBlocks[0].ClosestBlock.column - PieceBlocks[0].Column].IsOccupied = true;
            Destroy(block.gameObject);
        }
        PieceBlocks.Clear();        
        this.gameObject.GetComponentInParent<ContainerScript>().IsEmpty = true;
        
    }

    public bool CheckIfPieceCanFit()
    {
        if (PieceBlocks.Count == 0)
            return false;

        for (int rowIndex = 0; rowIndex < Grid.rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < Grid.columns; columnIndex++)
            {
                bool result = true;
                foreach (var block in PieceBlocks)
                {
                    
                    if (Grid.rows > (rowIndex + block.Row))
                    {
                        if (Grid.columns > (columnIndex + block.Column))
                            result &= !(Grid.columnRowDictionary[$"row{(rowIndex + block.Row)}"][(columnIndex + block.Column)].IsOccupied);
                        else
                        {
                            result = false;
                            break;
                        }
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                if(result) 
                    return true;
            }
        }
        return false;
    }
}
