using System.Collections.Generic;
using UnityEngine;
using MEC;
using EarlyGameTweaks;

namespace ServerTools
{
    public class PathFinder
    {
        private Vector3 startPos;
        private Vector3 targetPos;
        private List<Vector3> path;
        private HashSet<Vector3> visited;
        private float stepSize;
        private System.Action<List<Vector3>> callback; // Callback to return the path

        public PathFinder(float stepSize)
        {
            this.stepSize = stepSize;
            path = new List<Vector3>();
            visited = new HashSet<Vector3>();
        }

        public void StartPathfinding(Vector3 start, Vector3 target, System.Action<List<Vector3>> callback)
        {
            this.callback = callback;
            Timing.RunCoroutine(FindPathAsync(start, target));
        }

        private IEnumerator<float> FindPathAsync(Vector3 start, Vector3 target)
        {
            startPos = start;
            targetPos = target;
            path.Clear();
            visited.Clear();

            PriorityQueue<Node> openSet = new PriorityQueue<Node>();
            Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
            Dictionary<Vector3, float> gScore = new Dictionary<Vector3, float>();

            Vector3 roundedStart = RoundToGrid(startPos);
            openSet.Enqueue(new Node(roundedStart, 0, Heuristic(roundedStart, targetPos)));
            gScore[roundedStart] = 0;
            visited.Add(roundedStart);

            int iterations = 0;
            while (openSet.Count > 0)
            {
                Node current = openSet.Dequeue();
                Vector3 currentPos = current.position;

                float distanceToTarget = Vector3.Distance(currentPos, targetPos);
                if (distanceToTarget <= stepSize * 1.5f)
                {
                    ReconstructPath(cameFrom, currentPos);
                    callback?.Invoke(path); // Return the path via callback
                    yield break;
                }

                foreach (Vector3 neighbor in GetNeighbors(currentPos))
                {
                    if (visited.Contains(neighbor)) continue;

                    if (!IsPathClear(currentPos, neighbor))
                    {
                        continue;
                    }

                    float tentativeGScore = gScore[currentPos] + Vector3.Distance(currentPos, neighbor);

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = currentPos;
                        gScore[neighbor] = tentativeGScore;
                        float hScore = Heuristic(neighbor, targetPos);
                        openSet.Enqueue(new Node(neighbor, tentativeGScore, hScore));
                        visited.Add(neighbor);
                    }
                }

                // **Yield after every few iterations to avoid freezing the game**
                iterations++;
                if (iterations % 5 == 0)
                    yield return Timing.WaitForOneFrame; // Wait for next frame
            }

            Debug.LogError("Path not found!");
            callback?.Invoke(null); // Return null if no path is found
        }

        private float Heuristic(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        private Vector3 RoundToGrid(Vector3 pos)
        {
            return new Vector3(
                Mathf.Round(pos.x / stepSize) * stepSize,
                Mathf.Round(pos.y / stepSize) * stepSize,
                Mathf.Round(pos.z / stepSize) * stepSize
            );
        }

        private List<Vector3> GetNeighbors(Vector3 pos)
        {
            return new List<Vector3>
            {
                new Vector3(pos.x + stepSize, pos.y, pos.z),
                new Vector3(pos.x - stepSize, pos.y, pos.z),
                new Vector3(pos.x, pos.y, pos.z + stepSize),
                new Vector3(pos.x, pos.y, pos.z - stepSize),
                new Vector3(pos.x, pos.y + stepSize, pos.z),
                new Vector3(pos.x, pos.y - stepSize, pos.z)
            };
        }

        private bool IsPathClear(Vector3 from, Vector3 to)
        {
            Vector3 direction = (to - from).normalized;
            float distance = Vector3.Distance(from, to);
            RaycastHit hit;

            return !Physics.Raycast(from, direction, out hit, distance, layerMask: 537198593);
        }

        private void ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
        {
            path.Add(targetPos);
            while (cameFrom.ContainsKey(current))
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Add(startPos);
            path.Reverse();
        }

        private class Node
        {
            public Vector3 position;
            public float gScore;
            public float fScore;

            public Node(Vector3 pos, float g, float h)
            {
                position = pos;
                gScore = g;
                fScore = g + h;
            }
        }

        private class PriorityQueue<T> where T : Node
        {
            private List<T> items = new List<T>();

            public int Count => items.Count;

            public void Enqueue(T item)
            {
                items.Add(item);
                items.Sort((a, b) => a.fScore.CompareTo(b.fScore));
            }

            public T Dequeue()
            {
                if (items.Count == 0) return null;
                T item = items[0];
                items.RemoveAt(0);
                return item;
            }
        }
    }
}
