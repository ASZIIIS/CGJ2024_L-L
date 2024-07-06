using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public GridManager manager;
    public int mapSize=6;
    void Start()
    {
        manager=new GridManager(mapSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
