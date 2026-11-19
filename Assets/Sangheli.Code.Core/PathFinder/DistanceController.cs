using System.Collections.Generic;

public class DistanceController
{
    public static (int, List<IBfsTile>) GetDistanceBetweenRooms(IBfsTile from, IBfsTile to)
    {
        if (from == to) return (0, new List<IBfsTile> { from });

        Queue<IBfsTile> queue = new();
        Dictionary<IBfsTile, IBfsTile> parent = new();
        HashSet<IBfsTile> visited = new();

        queue.Enqueue(from);
        visited.Add(from);
        parent[from] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var next in current.ConnectedTileList)
            {
                if (visited.Contains(next)) continue;

                visited.Add(next);
                parent[next] = current;
                queue.Enqueue(next);

                if (next != to)
                    continue;

                List<IBfsTile> path = new();
                var node = to;
                while (node != null)
                {
                    path.Add(node);
                    node = parent[node];
                }

                path.Reverse();
                return (path.Count - 1, path);
            }
        }

        return (-1, null);
    }
}
