using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace JFramework.AStar
{
    /// <summary>
    /// A星寻路管理器
    /// </summary>
    public static class AStarManager
    {
        /// <summary>
        /// 地图节点
        /// </summary>
        private static bool[,] nodeMap;

        /// <summary>
        /// 地图宽度
        /// </summary>
        private static int mapWidth;
        
        /// <summary>
        /// 地图高度
        /// </summary>
        private static int mapHeight;

        /// <summary>
        /// 初始化地图(设置地图大小)
        /// </summary>
        public static void SetMap(bool[,] nodeMap)
        {
            AStarManager.nodeMap = nodeMap;
            mapWidth = nodeMap.GetLength(0);
            mapHeight = nodeMap.GetLength(1);
        }

        /// <summary>
        /// 基于JobSystem的AStar寻路
        /// </summary>
        /// <param name="origin">传入开始位置</param>
        /// <param name="target">传入最终位置</param>
        /// <returns></returns>
        public static List<AStarNode> PathFinding(AStarNode origin, AStarNode target)
        {
            var nodeList = new List<AStarNode>();
            var nodes = new NativeArray<bool>(mapWidth * mapHeight, Allocator.TempJob);
            var path = new NativeList<AStarNode>(Allocator.TempJob);
            var openList = new NativeList<AStarNode>(Allocator.TempJob);
            var closeList = new NativeHashSet<Vector2Int>(mapWidth * mapHeight, Allocator.TempJob);
            var linkRelation = new NativeHashMap<Vector2Int,Vector2Int>(mapWidth * mapHeight - 1, Allocator.TempJob);
            var direction = new NativeList<Vector2Int>(Allocator.TempJob)
            {
                new Vector2Int(-1, 0), new Vector2Int(0, 1),
                new Vector2Int(1, 0), new Vector2Int(0, -1),
            };
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    nodes[i * mapHeight + j] = nodeMap[i, j];
                }
            }
 
            //Job开启
            var job = new AStarJob()
            {
                nodes = nodes,
                path = path,
                target = target,
                mapWidth = mapWidth,
                mapHeight = mapHeight,
                openList = openList,
                closeList = closeList,
                direction = direction,
                linkMap = linkRelation,
                current = origin
            };
            var handle = job.Schedule();
            handle.Complete();
            //Job完成

            for (int i = path.Length - 1; i >= 0; i--)
            {
                nodeList.Add(path[i]);
            }
            
            nodes.Dispose();
            path.Dispose();
            direction.Dispose();
            openList.Dispose();
            closeList.Dispose();
            linkRelation.Dispose();
            return nodeList;
        }
    }
}