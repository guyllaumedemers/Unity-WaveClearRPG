using UnityEngine;

namespace AStar
{
    public class Node : IHeapItem<Node> {
        public int gCost;
        public int hCost;
        public int gridX;
        public int gridY;
        public bool isWalkable;
        public int movementPenalty;

        public Node parent;
        public Vector3 worldPosition;

        public int fCost => gCost + hCost;
        public int HeapIndex { get; set; }
        
        public Node(bool walkable, Vector3 worldPos, int gX, int gY, int penalty) {
            isWalkable = walkable;
            worldPosition = worldPos;
            gridX = gX;
            gridY = gY;
            movementPenalty = penalty;
        }
    
        public int CompareTo(Node nodeToCompare) {
            var compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0) compare = hCost.CompareTo(nodeToCompare.hCost);
            return -compare;
        }
    }
}