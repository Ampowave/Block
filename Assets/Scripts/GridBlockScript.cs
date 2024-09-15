using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBlockScript : MonoBehaviour
{
    public int column;
    public int row;
    public bool IsOccupied = false;
    public bool IsHover = false;

    public Color unoccupiedColor;
    public Color hoverColor;
    public Color occupiedColor;

    private void Start()
    {
        
    }
    private void Update()
    {
        switch (IsOccupied, IsHover)
        {
            case (false, false):
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = unoccupiedColor;
                    break;
                }
            case (true, false):
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = occupiedColor;
                    break;
                }
            case (false, true):
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = hoverColor;
                    break;
                }
            case (true, true):
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = occupiedColor;
                    break;
                }
        }


    }
}
