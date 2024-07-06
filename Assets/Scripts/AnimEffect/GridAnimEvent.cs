using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimEvent : MonoBehaviour
{
    public Sprite level1TargetSprite,level2TargetSprite;
    private TileMapGameObjectController tileMapController;
    private void Awake()
    {
        tileMapController = FindObjectOfType<TileMapGameObjectController>();
    }

    public void AnimEvent_FlipToSprite1()
    {
        Sprite _targetSprite = null;
        switch (TileMapGameObjectController.CurLevel)
        {
            case 3:
                //todo //��Ϸ����
                Debug.Log("Game Over");
                return;
                break;
            case 1:
                _targetSprite = level1TargetSprite;
                break;
            case 2:
                _targetSprite = level2TargetSprite;
                break;
            default:
                return;
        }
        GetComponentInChildren<SpriteRenderer>().sprite = _targetSprite;

        //ɾ���ɵ�С���壬�½��µ�С����
        tileMapController.ReGenerateSmallGameObject(transform);
    }
     
}
