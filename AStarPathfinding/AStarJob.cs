using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace JFramework.AStar
{
    [BurstCompile]
    internal struct AStarJob : IJob
    {
        /// <summary>
        /// 地图宽
        /// </summary>
        public int mapWidth;

        /// <summary>
        /// 地图高
        /// </summary>
        public int mapHeight;

        /// <summary>
        /// 目标节点
        /// </summary>
        public AStarNode target;

        /// <summary>
        /// 当前节点
        /// </summary>
        public AStarNode current;

        /// <summary>
        /// 可行走区域
        /// </summary>
        [ReadOnly] public NativeArray<bool> nodes;

        /// <summary>
        /// 寻路方向
        /// </summary>
        [ReadOnly] public NativeList<Vector2Int> direction;

        /// <summary>
        /// 寻路路径
        /// </summary>
        public NativeList<AStarNode> path;

        /// <summary>
        /// 开启列表
        /// </summary>
        public NativeList<AStarNode> openList;

        /// <summary>
        /// 关闭列表
        /// </summary>
        public NativeHashSet<Vector2Int> closeList;

        /// <summary>
        /// 链接关系
        /// </summary>
        public NativeHashMap<Vector2Int, Vector2Int> linkMap;

        /// <summary>
        /// 比较器
        /// </summary>
        private PointComparer comparer;

        private struct PointComparer : IComparer<AStarNode>
        {
            public int Compare(AStarNode start, AStarNode end)
            {
                return start.fCost.CompareTo(end.fCost);
            }
        }

        public void Execute()
        {
            current.gCost = 0;
            current.hCost = GetDistance(current, target);

            while (current != target)
            {
                GetNextPoint(current);
                if (openList.Length == 0)
                {
                    break;
                }

                current = openList[0];
            }

            if (current == target)
            {
                path.Add(current);
                while (linkMap.ContainsKey(new Vector2Int(current.x, current.y)))
                {
                    Vector2Int lastPoint = new Vector2Int(current.x, current.y);
                    Vector2Int tempPoint = linkMap[lastPoint];
                    AStarNode aStarNode = new AStarNode(tempPoint);
                    current = aStarNode;
                    path.Add(current);
                }
            }
        }

        private bool StopMove(int x, int y)
        {
            if (x < 0) return true;
            if (x >= mapWidth) return true;
            if (y < 0) return true;
            if (y >= mapHeight) return true;
            return nodes[x * mapHeight + y];
        }

        private void GetNextPoint(AStarNode point)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = point.x + direction[i].x;
                int y = point.y + direction[i].y;
                var tempNode = new Vector2Int(x, y);
                if (!StopMove(x, y) && !closeList.Contains(tempNode))
                {
                    AStarNode newPoint = new AStarNode(tempNode);
                    newPoint.gCost = point.gCost + GetDistance(point, newPoint);
                    newPoint.hCost = GetDistance(newPoint, target);

                    int index = IndexOf(newPoint);
                    Vector2Int node = new Vector2Int(point.x, point.y);
                    if (index == -1)
                    {
                        openList.Add(newPoint);
                        linkMap.Add(tempNode, node);
                    }
                    else
                    {
                        AStarNode oldPoint = openList[index];
                        if (newPoint.fCost < oldPoint.fCost)
                        {
                            openList[index] = newPoint;
                            linkMap[tempNode] = node;
                        }
                    }
                }
            }

            for (int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == point)
                {
                    openList.RemoveAt(i);
                    break;
                }
            }


            Vector2Int newNode = new Vector2Int(point.x, point.y);
            closeList.Add(newNode);
            openList.Sort(comparer);
        }

        private static int GetDistance(AStarNode origin, AStarNode target)
        {
            return Mathf.Abs(origin.x - target.x) + Mathf.Abs(origin.y - target.y);
        }

        private int IndexOf(AStarNode point)
        {
            for (int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == point)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}