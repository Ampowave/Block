using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceGeneratorScript : MonoBehaviour
{
    [SerializeField] private PieceData[] dataList;
    [SerializeField] private GameObject pieceBlockPrefab;
    [SerializeField] private GameObject[] containers;
    public GameObject[] Pieces;

    public Vector3 IdlePieceScale;



    private void Start()
    {
        if (CheckAllContainersEmpty())
        {
            GeneratePieces();
        }
    }

    public void GeneratePieces()
    {
        foreach (var container in containers)
        {
            var data = GetRandomPieceData();
            BuildPiece(data, container.GetComponentInChildren<PieceHandler>());
            SetPieceInContainer(container, container.GetComponentInChildren<PieceHandler>(), data);
            container.GetComponent<ContainerScript>().IsEmpty = false;
        }
    }

    private PieceData GetRandomPieceData()
    {
       return dataList[UnityEngine.Random.Range(0, dataList.Length)];
    }

    //wanna change this method to pull blocks from an object pool instead of instantiating them
    private void BuildPiece(PieceData data, PieceHandler piece)
    {
        Vector3 spawnPosition = new Vector3(0f, 0f);
        piece.PieceData = data;

        for (int i = 0; i < data.Rows; i++)
        {
            for (int j = 0; j < data.Columns; j++)
            {
                if (data.PieceMap[i].Column[j])
                {
                    GameObject newGO = Instantiate(pieceBlockPrefab, spawnPosition, pieceBlockPrefab.transform.rotation, piece.gameObject.transform); //TODO: ObjectPool.GetObject()
                    newGO.GetComponent<PieceBlockScript>().Row = i;
                    newGO.GetComponent<PieceBlockScript>().Column = j;
                    piece.PieceBlocks.Add(newGO);
                }
                spawnPosition.x += 1f;
            }
            spawnPosition.x = 0f;
            spawnPosition.y -= 1f;
        }
    }

    private void SetPieceInContainer(GameObject container, PieceHandler piece, PieceData data)
    {
        foreach (var block in piece.PieceBlocks)
        {
            // the extra expressions using PieceData properties might look weird but the results are perfect. think of it as a formula.
            block.transform.position = new Vector3(container.transform.position.x + block.transform.position.x - ((data.Columns -1) * 0.5f),
                                                    container.transform.position.y + block.transform.position.y + ((data.Rows - 1) * 0.5f));
        }
        piece.transform.localScale = IdlePieceScale;
    }
    
    public bool CheckAllContainersEmpty()
    {
        bool result = true;

        foreach (var container in containers)
        {
            result &= container.GetComponent<ContainerScript>().IsEmpty;
        }
        return result;
    }

    public void CheckLoseCondition()
    {
        Debug.Log("checking...");
        bool result = false;

        foreach (var piece in Pieces)
        {
            result |= piece.GetComponent<PieceHandler>().CheckIfPieceCanFit();
        }
        Debug.Log(result);
        if (!result) Debug.Log("You Lose!");
    }
}
