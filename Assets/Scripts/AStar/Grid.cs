using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class Grid : MonoBehaviour {
        public bool displaGridGizmos;
        public float nodeRadius;
    
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public TerrainType[] walkableRegions;
        public Node[,] grid;
    
        private LayerMask _walkableMask;
        private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();
    
        private int _gridSizeX;
        private int _gridSizeY;
        private float _nodeDiameter;
    
        public int MaxSize => _gridSizeX * _gridSizeY;

        private void Awake() {
            _nodeDiameter = nodeRadius * 2;
            //_nodeDiameter = nodeRadius;
            
            _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);

            foreach (TerrainType region in walkableRegions) {
                _walkableMask.value += region.terrainMask.value;
                _walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
            }
        
            CreateGrid();
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (grid != null && displaGridGizmos) {
                foreach (var n in grid) {
                    Gizmos.color = n.isWalkable ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (_nodeDiameter - 0.1f));
                }
            }
        }

        private void CreateGrid() {
            grid = new Node[_gridSizeX, _gridSizeY];
            var worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for (var x = 0; x < _gridSizeX; x++) {
                for (var y = 0; y < _gridSizeY; y++) {
                    var worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + nodeRadius) + Vector3.forward * (y * _nodeDiameter + nodeRadius);
                    var walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);

                    int movementPenalty = 0;

                    if (walkable) {
                        Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100, _walkableMask)) {
                            _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        }
                    }
                
                    grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }
        }

        public List<Node> GetNeighbours(Node node) {
            var neighbors = new List<Node>();

            for (var x = -1; x <= 1; x++) {
                for (var y = -1; y <= 1; y++) {
                    if (x == 0 && y == 0)
                        continue;

                    var checkX = node.gridX + x;
                    var checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                        neighbors.Add(grid[checkX, checkY]);
                }
            }

            return neighbors;
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition) {
            var percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            var percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            var x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            var y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

            return grid[x, y];
        }
    }

    [System.Serializable]
    public class TerrainType {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}