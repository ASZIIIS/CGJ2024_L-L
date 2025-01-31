using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimEvent : MonoBehaviour
{
    public Sprite level0TargetSprite,level1TargetSprite,level2TargetSprite;
    private TileMapGameObjectController tileMapController;
    private void Awake()
    {
        level0TargetSprite = GetComponentInChildren<SpriteRenderer>().sprite;
        tileMapController = FindObjectOfType<TileMapGameObjectController>();
    }

    public void AnimEvent_FlipToSprite1()
    {
        Sprite _targetSprite = null;
        switch (TileMapGameObjectController.CurLevel)
        {
            case 3:
                //todo //游戏结束
                _targetSprite = level0TargetSprite;
                TileMapGameObjectController.CurLevel = 0;
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

        //删除旧的小物体，新建新的小物体
        tileMapController.ReGenerateSmallGameObject(transform);
        
    }
     
}
