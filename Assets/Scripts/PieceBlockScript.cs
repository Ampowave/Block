using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBlockScript : MonoBehaviour
{
    private PieceHandler _parentPieceHandler;

    public int Row;
    public int Column;

    public GridBlockScript ClosestBlock = null;
    //using OnEnable in case we implement object pooling
    private void OnEnable()
    {
        _parentPieceHandler = GetComponentInParent<PieceHandler>();
    }


    //basically this is done because the "Piece" GO doesnt have a collider (but the Piece Blocks do) and i don't know how to make a composite collider from its children's colliders.
    public void OnMouseDown()
    {
        _parentPieceHandler.PieceOnMouseDown();
    }

    public void OnMouseDrag()
    {
        _parentPieceHandler.PieceOnMouseDrag();    
    }

    public void OnMouseUp()
    {
        _parentPieceHandler.PieceOnMouseUp();
    }
}
