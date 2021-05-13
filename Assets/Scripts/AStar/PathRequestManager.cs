using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AStar
{
    public class PathRequestManager : MonoBehaviour {

        public static PathRequestManager Instance { get; protected set; }
        
        private bool _isProcessingPath;
        private Pathfinding _pathfinding;
        private PathRequest _currentPathRequest;
        private Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
        Queue<PathResult> results = new Queue<PathResult>();
        
        private void Awake() {
            if(Instance == null) Instance = this;
            _pathfinding = GetComponent<Pathfinding>();
        }

        void Update() {
            if (results.Count > 0) {
                int itemsInQueue = results.Count;
                lock (results) {
                    for (int i = 0; i < itemsInQueue; i++) {
                        PathResult result = results.Dequeue ();
                        result.callback (result.path, result.success);
                    }
                }
            }
        }
        
        public static void RequestPath(PathRequest request) {
            ThreadStart threadStart = delegate {
                Instance._pathfinding.FindPath(request, Instance.FinishedProcessingPath);
            };
            threadStart.Invoke ();
        }
        
        private void TryProcessNext() {
            if (!_isProcessingPath && _pathRequestQueue.Count > 0) {
                _currentPathRequest = _pathRequestQueue.Dequeue();
                _isProcessingPath = true;
                _pathfinding.StartFindPath(_currentPathRequest.pathStart, _currentPathRequest.pathEnd);
            }
        }
        
        public void FinishedProcessingPath(PathResult result) {
            lock (results) {
                results.Enqueue (result);
            }
        }
    }
}