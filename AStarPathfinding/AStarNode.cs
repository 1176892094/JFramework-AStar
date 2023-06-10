using UnityEngine;

namespace JFramework.AStar
{
    public struct AStarNode
    {
        public readonly int x;
        public readonly int y;
        internal int gCost;
        internal int hCost;
        internal int fCost => gCost + hCost;

        public AStarNode(Vector2Int position)
        {
            x = position.x;
            y = position.y;
            gCost = int.MaxValue;
            hCost = int.MaxValue;
        }

        public static bool operator ==(AStarNode a, AStarNode b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(AStarNode a, AStarNode b)
        {
            return !(a.x == b.x && a.y == b.y);
        }

        public override bool Equals(object obj)
        {
            return obj is AStarNode node && node == this;
        }

        public override int GetHashCode()
        {
            return x + y + fCost;
        }

        public override string ToString()
        {
            return $"Point[{x} , {y}] g:{gCost} h:{hCost} f:{fCost}";
        }
    }
}