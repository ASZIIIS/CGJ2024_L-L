using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public List<Sprite> BallSprites;
    public GameObject ballPrefab; // 预制体，用于生成球
    public BoxCollider2D spawnArea; // BoxCollider2D区域

    public float angleRange = 30f;
    public float minSpeed = 1f; // 最小速度
    public float maxSpeed = 5f; // 最大速度
    public float minAngularSpeed = -360f; // 最小角速度
    public float maxAngularSpeed = 360f; // 最大角速度

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            int ballIndex = Random.Range(0, BallSprites.Count);
            SpawnBall(ballIndex);
        }
    }
    
    void SpawnBall(int _index)
    {
        // 获取BoxCollider2D的边界
        Bounds bounds = spawnArea.bounds;

        // 在BoxCollider2D范围内随机生成一个位置
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        Vector2 randomPosition = new Vector2(randomX, randomY);

        // 生成球
        GameObject ball = Instantiate(ballPrefab, randomPosition, Quaternion.identity);
        ball.GetComponentInChildren<SpriteRenderer>().sprite = BallSprites[_index];
        // 获取球的Rigidbody2D组件
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

        // 给球一个朝下的随机速度
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        float randomAngle = Random.Range(-angleRange, angleRange);
        float angleInRadians = randomAngle * Mathf.Deg2Rad;

        // 计算速度的x和y分量
        Vector2 velocity = new Vector2(Mathf.Sin(angleInRadians), -Mathf.Cos(angleInRadians)) * randomSpeed;
        rb.velocity = velocity;
        
        // 给球一个随机旋转角速度
        float randomAngularSpeed = Random.Range(minAngularSpeed, maxAngularSpeed);
        rb.angularVelocity = randomAngularSpeed;
    }
}
