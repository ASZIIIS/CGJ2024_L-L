using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager
{
    public static Vector3[] halfUnitVector;
    public static Vector3[] waveUnitVector;
    public static void initHalfUnitVector(){
        halfUnitVector=new Vector3[6];
        waveUnitVector=new Vector3[6];
        Grid grid=GameObject.Find("Grid").GetComponent<Grid>();
        Vector3 centerToWorld=grid.CellToWorld(new Vector3Int(0,0,-10));
        Vector3 nextToWorld;
        nextToWorld=grid.CellToWorld(new Vector3Int(1,0,-10));//Up
        halfUnitVector[0]=(nextToWorld-centerToWorld)/2;
        nextToWorld=grid.CellToWorld(new Vector3Int(0,1,-10));//RightUp
        halfUnitVector[1]=(nextToWorld-centerToWorld)/2;
        nextToWorld=grid.CellToWorld(new Vector3Int(-1,1,-10));//RightDown
        halfUnitVector[2]=(nextToWorld-centerToWorld)/2;
        nextToWorld=grid.CellToWorld(new Vector3Int(-1,0,-10));//Down
        halfUnitVector[3]=(nextToWorld-centerToWorld)/2;
        nextToWorld=grid.CellToWorld(new Vector3Int(-1,-1,-10));//LeftDown
        halfUnitVector[4]=(nextToWorld-centerToWorld)/2;
        nextToWorld=grid.CellToWorld(new Vector3Int(0,-1,-10));//LeftUp
        halfUnitVector[5]=(nextToWorld-centerToWorld)/2;
        for(int i=0;i<6;++i){
            waveUnitVector[i]=(halfUnitVector[(i+1)%6]+halfUnitVector[(i+2)%6])/4;
        }
    }
    public static Vector3Int[] getAroundGrids(Vector3Int gridPosition){
        Vector3Int[] gridList=new Vector3Int[6];
        for(int i=0;i<6;++i){
            gridList[i]=gridPosition;
        }
        //Up
        gridList[0].x+=1;
        //RightUp
        gridList[1].x+=Mathf.Abs(gridPosition.y)%2;
        gridList[1].y+=1;
        //RightDown
        gridList[2].x-=Mathf.Abs(gridPosition.y+1)%2;
        gridList[2].y+=1;
        //Down
        gridList[3].x-=1;
        //LeftDown
        gridList[4].x-=(Mathf.Abs(gridPosition.y+1)%2);
        gridList[4].y-=1;
        //LeftUp
        gridList[5].x-=(Mathf.Abs(gridPosition.y)%2);
        gridList[5].y-=1;
        return gridList;
    }
    public int size;
    public GridManager(int size){
        this.size=size;
        GridManager.initHalfUnitVector();
    }
    public Vector3Int move(Vector3Int origin, int direction){
        int x=origin.x;
        int y=origin.y;
        int targetX=0, targetY=0;
        bool outBound=false;
        switch(direction){
            case 0://Up
                if(x+1+(Mathf.Abs(y)+1)/2>size-1){
                    targetX=-x-(Mathf.Abs(y)%2);
                    targetY=y;
                    outBound=true;
                }else{
                    targetX=x+1;
                    targetY=y;
                }
                break;
            case 1://RightUp
                if(y+1>size-1){
                    targetY=-(size/2)-x;
                    targetX=1-size-(targetY)/2;
                    outBound=true;
                }else if(x+1+(y+1)/2>size-1){
                    targetX=(size-1)/2-y;
                    targetY=1-size;
                    outBound=true;
                }else{
                    targetX=x+(Mathf.Abs(y)%2);
                    targetY=y+1;
                }
                break;
            case 2://RightDown
                if(y+1>size-1){
                    targetY=x-(size-1)/2;
                    targetX=size-1+(targetY-1)/2;
                    outBound=true;
                }else if(-x+1+(y)/2>size-1&&y>=0){
                    targetX=-size/2+y;
                    targetY=1-size;
                    outBound=true;
                }else{
                    targetX=x-(Mathf.Abs(y+1)%2);
                    targetY=y+1;
                }
                break;
            case 3://Down
                if(-x+1+(Mathf.Abs(y))/2>size-1){
                    targetX=-x-(Mathf.Abs(y)%2);
                    targetY=y;
                    outBound=true;
                }else{
                    targetX=x-1;
                    targetY=y;
                }
                break;
            case 4://LeftDown
                if(-y+1>size-1){
                    targetY=(size-1)/2-x;
                    targetX=size-1-(targetY+1)/2;
                    outBound=true;
                }else if(-x+1-(y)/2>size-1&&y<=0){
                    targetX=-(size)/2-y;
                    targetY=size-1;
                    outBound=true;
                }else{
                    targetX=x-(Mathf.Abs(y+1)%2);
                    targetY=y-1;
                }
                break;
            case 5://LeftUp
                if(-y+1>size-1){
                    targetY=x+(size)/2;
                    targetX=(targetY)/2+1-size;
                    outBound=true;
                }else if(x+1+(-y+1)/2>size-1){
                    targetX=(size-1)/2+y;
                    targetY=size-1;
                    outBound=true;
                }else{
                    targetX=x+(Mathf.Abs(y)%2);
                    targetY=y-1;
                }
                break;
        }
        if(outBound){
            return new Vector3Int(targetX, targetY, -8);
        }else{
            return new Vector3Int(targetX, targetY, -9);
        }
    }
    public static Vector2Int[] getAroundGrids(Vector2Int gridPosition){
        Vector2Int[] gridList=new Vector2Int[6];
        for(int i=0;i<6;++i){
            gridList[i]=gridPosition;
        }
        //Up
        gridList[0].x+=1;
        //RightUp
        gridList[1].x+=Mathf.Abs(gridPosition.y)%2;
        gridList[1].y+=1;
        //RightDown
        gridList[2].x-=Mathf.Abs(gridPosition.y+1)%2;
        gridList[2].y+=1;
        //Down
        gridList[3].x-=1;
        //LeftDown
        gridList[4].x-=(Mathf.Abs(gridPosition.y+1)%2);
        gridList[4].y-=1;
        //LeftUp
        gridList[5].x-=(Mathf.Abs(gridPosition.y)%2);
        gridList[5].y-=1;
        return gridList;
    }
}
