using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SnakeHead : MonoBehaviour
{
    
    private GridManager manager;
    private Grid grid;
    private TileMapGameObjectController tileController;
    private int direction;
    private Vector3Int currentGrid, targetGrid;
    private bool moveStage=false;
    private bool isGrowing;
    private SnakeBody nextBody=null;
    private bool systemPause=false, pause=true;
    public Transform parent;
    public int forwardTimer, forwardPeriod, waveTimer, wavePeriod;
    public float normalScale;
    public GameObject bodyPrefeb;
    private HeadAnimation animator;
    // Start is called before the first frame update
    void Start()
    {
        manager=new GridManager(6);
        grid=GameObject.Find("GridGameObjectController").GetComponent<Grid>();
        tileController=GameObject.Find("GridGameObjectController").GetComponent<TileMapGameObjectController>();
        animator=GetComponent<HeadAnimation>();
        direction=(int)Directions.Down;
        targetGrid=new Vector3Int(0,0,-9);
        randomCatAnimation();
    }
    // Update is called once per frame
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            pause=!pause;
        }
    }
    void FixedUpdate(){
        if(systemPause){
            return;
        }
        Vector3 currentPosition;
        if(!pause){
            if(forwardTimer==0){
                moveStage=!moveStage;
                isGrowing=false;
                if(moveStage){
                    //reach the center of a grid
                    if(nextBody != null){
                        nextBody.changeTarget(targetGrid, direction);
                    }
                    currentGrid=targetGrid;
                    GridSingle currentGridInfo=tileController.GetTileObject((Vector2Int)currentGrid).GetComponent<GridSingle>();
                    if(currentGridInfo.food != null){
                        isGrowing=true;
                        if(nextBody != null){
                            nextBody.addTail((int)currentGridInfo.food.GetComponent<Food>().foodType, currentGrid, direction);
                            nextBody.activeGrow();
                        }else{
                            nextBody=Instantiate(bodyPrefeb, grid.CellToWorld(currentGrid), Quaternion.identity).GetComponent<SnakeBody>();
                            nextBody.Init(this, grid, currentGrid, direction, 0, /*waveAmplitude*/0.3f, /*wavePhase*/5f, /*normalScale*/normalScale);
                            nextBody.GetComponent<BodyAnimation>().ChangeSprite((int)currentGridInfo.food.GetComponent<Food>().foodType);
                            nextBody.activeGrow();
                        }
                        tileController.RemoveFood((Vector2Int)currentGrid);
                        //TODO: add 1 point
                    }
                    if(currentGridInfo.direction>0){
                        if(((int)currentGridInfo.direction-this.direction+6)%6==3){
                            //TODO: end game
                        }else{
                            this.direction=(int)currentGridInfo.direction;
                            transform.rotation=Quaternion.Euler(0f,0f,60f*(4-this.direction));
                        }
                    }
                    targetGrid=manager.move(currentGrid, direction);
                }
            }
            if(moveStage){
                //first half (center to edge)
                currentPosition=grid.CellToWorld(currentGrid)+GridManager.halfUnitVector[direction]*forwardTimer/forwardPeriod;
                transform.position=currentPosition;
                if(isGrowing){
                    transform.localScale=Vector3.one*normalScale*(1f+0.4f*Mathf.Sin(Mathf.PI*forwardTimer/forwardPeriod));
                }
            }else{
                //second half (edge to center)
                currentPosition=grid.CellToWorld(targetGrid)-GridManager.halfUnitVector[direction]*(forwardPeriod-forwardTimer)/forwardPeriod;
                transform.position=currentPosition;
            }
        }
        if(nextBody != null){
            nextBody.move(moveStage, pause);
        }
        goTime();
    }
    public void goTime(){
        if(!pause){
            forwardGoTime();
        }
        waveGoTime();
    }
    public void forwardGoTime(){
        forwardTimer=(forwardTimer+1)%forwardPeriod;
    }
    public void waveGoTime(){
        waveTimer=(waveTimer+1)%wavePeriod;
    }
    public void initTime(int forwardPeriod, int wavePeriod){
        this.forwardPeriod=forwardPeriod;
        this.wavePeriod=wavePeriod;
        forwardTimer=0;
        waveTimer=0;
    }
    private void randomCatAnimation(){
        int index=Random.Range(0,6);
        animator.ChangeAnimation(index);
        animator.setAnimation(true);
    }

}
