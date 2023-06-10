# JFramework-AStarPathfinding
基于JobSystem的A星寻路算法的测试脚本：
```csharp
using System.Collections;
using System.Collections.Generic;
using JFramework.AStar;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int mapW;
    public int mapH;

    private void Start()
    {
        var map = new bool[mapW, mapH];
        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                map[i, j] = Random.Range(0, 100) < 80; //设置小于80为可行走区域
            }
        }

        AStarManager.SetMap(map); //初始化可行走的地图
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //按下鼠标
        {
            //起点位置
            var start = new AStarNode(Vector2Int.zero);
            //鼠标点击位置
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //设置为格子的位置
            var nodePos = new Vector2Int((int)position.x, (int)position.y);
            //终点位置
            var end = new AStarNode(nodePos);
            //返回一条路径
            var nodeList = AStarManager.PathFinding(start, end);
            //通过协程遍历路径上的点
            StartCoroutine(MoveToNextNode(nodeList));
        }
    }

    private IEnumerator MoveToNextNode(List<AStarNode> nodeList)
    {
        var nodeIndex = 0;
        while (nodeIndex < nodeList.Count)
        {
            var nextNode = nodeList[nodeIndex];
            var position = new Vector3(nextNode.x, nextNode.y);
            while (transform.position != position)// 移动到下一个节点
            {
                transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime);
                yield return null;
            }

            nodeIndex++;
        }
    }
}
```
