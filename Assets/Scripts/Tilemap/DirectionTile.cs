using UnityEngine;

public class DirectionTile : MonoBehaviour
{
    public Directions direction;

    // ����Tile�ķ���
    public void SetDirection(Directions newDirection)
    {
        direction = newDirection;
    }
}
