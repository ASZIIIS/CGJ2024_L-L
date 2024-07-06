using UnityEngine;

public class DirectionTile : MonoBehaviour
{
    public Directions direction;

    // 设置Tile的方向
    public void SetDirection(Directions newDirection)
    {
        direction = newDirection;
    }
}
