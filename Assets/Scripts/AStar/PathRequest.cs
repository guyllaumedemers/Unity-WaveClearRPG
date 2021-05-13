using System;
using UnityEngine;

namespace AStar
{
    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
        {
            this.pathStart = start;
            this.pathEnd = end;
            this.callback = callback;
        }
    }
}