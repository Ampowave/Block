using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PieceHandler : MonoBehaviour
{
    private GameObject _tmpBlock;
    private Transform _transform;

    public List<GameObject> PieceBlocks;
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
        FindClosestForEachPieceBlock();
        MakeClosestHover();
    }

    public void PieceOnMouseUp()
    {
        if (CheckIfAllClosestAreViable())
        {
            SetPiece();
            _transform.position = Container.transform.position;
        }
        else
        {
            _transform.position = Container.transform.position;
            _transform.localScale = PieceGeneratorScript.IdlePieceScale;
        }


        for (int i = 0; i < Grid.GridBlockList.Count; i++)
        {
            Grid.GridBlockList[i].GetComponent<GridBlockScript>().IsHover = false;
        }
        Debug.Log("released");


        Grid.AddFilledRowsAndColumnsToPopList();
        Grid.PopFilledRowsAndColumns();

        PieceGeneratorScript.CheckLoseCondition();

        if (PieceGeneratorScript.CheckAllContainersEmpty())
            PieceGeneratorScript.GeneratePieces();
    }

    private void FindClosestForEachPieceBlock()
    {
        foreach (var block in PieceBlocks)
        {
            for (int i = 0; i < Grid.GridBlockList.Count; i++)
            {

                //checking if it's not too far
                if (Vector3.Distance(block.transform.position, Grid.GridBlockList[i].GetComponent<Transform>().position) < 0.5f)
                {
                    if (block.GetComponent<PieceBlockScript>().ClosestBlock == null)
                    {
                        block.GetComponent<PieceBlockScript>().ClosestBlock = Grid.GridBlockList[i];
                    }
                    else
                    {
                        //checking if the new grid block is closer than the one we already have a reference to
                        if (Vector3.Distance(block.transform.position, Grid.GridBlockList[i].GetComponent<Transform>().position) < Vector3.Distance(block.transform.position, block.GetComponent<PieceBlockScript>().ClosestBlock.transform.position))
                        {
                            block.GetComponent<PieceBlockScript>().ClosestBlock = Grid.GridBlockList[i];
                        }
                    }
                }
                
            }
            if (block.GetComponent<PieceBlockScript>().ClosestBlock != null && Vector3.Distance(block.transform.position, block.GetComponent<PieceBlockScript>().ClosestBlock.GetComponent<Transform>().position) >= 0.5f)
                block.GetComponent<PieceBlockScript>().ClosestBlock = null;
        }
    }

    public void MakeClosestHover()
    {
        for (int i = 0; i < Grid.GridBlockList.Count; i++)
        {
            bool shouldBeHover = false;
            foreach (var block in PieceBlocks)
            {
                shouldBeHover |= (block.GetComponent<PieceBlockScript>().ClosestBlock != null && block.GetComponent<PieceBlockScript>().ClosestBlock == Grid.GridBlockList[i] && CheckIfAllClosestAreViable());
            }

            Grid.GridBlockList[i].GetComponent<GridBlockScript>().IsHover = shouldBeHover;
        }
    }

    private bool CheckIfAllClosestAreViable()
    {
        bool result = true;
        foreach (var block in PieceBlocks)
        {
            if (block.GetComponent<PieceBlockScript>().ClosestBlock == null)
            {
                result = false;
                break;
            }
            result &= !block.GetComponent<PieceBlockScript>().ClosestBlock.GetComponent<GridBlockScript>().IsOccupied;
        }
        return result;
    }

    public void SetPiece()
    {
        foreach (var block in PieceBlocks)
        {
            block.GetComponent<PieceBlockScript>().ClosestBlock.GetComponent<GridBlockScript>().IsOccupied = true;
            Destroy(block);
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
                    var tmp = block.GetComponent<PieceBlockScript>();
                    if (Grid.rows > (rowIndex + tmp.Row))
                    {
                        if (Grid.columns > (columnIndex + tmp.Column))
                            result &= !(Grid.columnRowDictionary[$"row{(rowIndex + tmp.Row)}"][(columnIndex + tmp.Column)].GetComponent<GridBlockScript>().IsOccupied);
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
