using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSingle : MonoBehaviour
{
    // public GridType gridType;
    // public enum GridType
    // {
    //     grid1,
    //     grid2,
    //     grid3
    // }
    public Directions direction;
    public void Init(Sprite _sprite, Directions initialDirection = 0)
    {
        // gridType = _gridType;
        transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = _sprite;
        direction = initialDirection;
    }
    public void SetDirection(Directions newDirection)
    {
        direction = newDirection;
    }
}
