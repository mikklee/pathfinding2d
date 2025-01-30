
//(C) Michael Thomas Lee

using System;
using System.Collections.Generic;

public class AstarPathfinder {

    public List<TileNode> NodeList { get; private set; }

    private List<TileNode> Visited { get; set; }


    /// <summary>
    /// This is the "pathfinding net". Uses ExtVector hashes for lookup speed.
    /// </summary>
    public Dictionary<ExtVector, TileNode> TileDict { get; private set; }

    public AstarPathfinder(Dictionary<ExtVector, TileNode> tileDict)
    {
        NodeList = new List<TileNode>();
        Visited = new List<TileNode>();
        TileDict = tileDict;
    }

    /// <summary>
    /// Pathfinds a path in 2D-space using the A* algorithm.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="target"></param>
    /// <returns>The path</returns>
    /// <exception cref="AstarNoPathException">Invalid Path</exception>
    public List<TileNode> GetPath(ExtVector startPosition, ExtVector target)
     {
         //Do not waste resources on impossible paths
         //May want zone mapping for more advanced mazes (with pockets) to
         //avoid further waste of resources.
         if(TargetIsInvalid(target))
             throw new AstarNoPathException();

         //Clear visited
         //TileDict.Values.ToList().ForEach(n => n.Visited = false);
         Visited.ForEach(node=>node.Visited = false);
         Visited.Clear();

         var startNode = TileDict[startPosition];
         MarkVisited(startNode);

         var list = new List<TileNode> ();
         list.Add(startNode);

         while (list.Count > 0)
         {
             //Get next
             var current = list[0];
             list.RemoveAt(0);

             //Return if path is complete
             if (current.Position.Equals(target))
                 return RetracePath(startNode, current); //Return path with success

             //Get Neighbors
             foreach (var node in current.GetNeighbors(TileDict))
             {
                 if (node.Visited) continue; //Already visited

                 node.Parent = current;

                 if (CheckCrossColider(node)) continue; //Impossible to move Diagonally

                 var distanceFromParent = node.Parent.Tile.GetDistance(node.Position);

                 MarkVisited(node);
                 list.Add(node);
                 node.DistanceFromStart =
                     node.Parent.DistanceFromStart+distanceFromParent;
                 node.DistanceEstimateToTarget = node.Tile.GetDistance(target);
             }
         }
         throw new AstarNoPathException();
     }

    private void MarkVisited(TileNode node)
    {
        node.Visited = true;
        Visited.Add(node);
    }

    /// <summary>
    /// This is made for diagonal movements to check if there are blocking/invalid
    /// cross-positions preventing successfull movement.
    /// </summary>
    /// <param name="n">The target node with parent set. If parent is not set, this will fail</param>
    /// <returns>Returns whether there were any invalid positions = invalid movement</returns>
    private bool CheckCrossColider(TileNode n)
    {
        //Unlike Vector3/2, ExtVector is mutable, so we clone.
        var start = n.Position.Clone();
        var end = n.Parent.Position.Clone();
        var delta = end - start;

        //Find possible blocking cross-positions
        var crossPos1 = new ExtVector(start.X, start.Y+delta.Y);
        var crossPos2 = new ExtVector(start.X+delta.X, start.Y);

        //Determine if they are blocking or invalid
        var invalids = new List<TileNode>();

        if(TileDict.ContainsKey(crossPos1) && !TileDict[crossPos1].IsWalkable)
            invalids.Add(TileDict[crossPos1]);

        if(TileDict.ContainsKey(crossPos2) && !TileDict[crossPos2].IsWalkable)
            invalids.Add(TileDict[crossPos2]);

        //Set invalid nodes as visited to avoid revisiting
        invalids.ForEach(MarkVisited);

        return invalids.Count > 0;
    }

    /// <summary>
    /// Retraces the path built by the A* alg by using the Node's parents.
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns>Returns path as list</returns>
    private static List<TileNode> RetracePath(TileNode startNode,TileNode endNode)
    {
        var path = new List<TileNode>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = (TileNode)currentNode.Parent;

        }

        path.Reverse();

        return path;
    }

    /// <summary>
    /// Checks if target is in pathfinding net, and if it's possible to walk on it.
    /// (Not a collider basically)
    /// </summary>
    /// <param name="target"></param>
    /// <returns>True or false ;) </returns>
    private bool TargetIsInvalid(ExtVector target)
    {
        return !TileDict.ContainsKey(target) || !TileDict[target].IsWalkable;
    }
}



public class AstarNoPathException : Exception
{
    public AstarNoPathException() : base("Invalid path")
    {
    }
}
