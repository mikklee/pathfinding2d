
using System;

public class Node
{
    public ExtVector Position { get; private set; }
    public double DistanceEstimateToTarget { get; set; }
    public double DistanceFromStart { get; set; }

    public Node Parent { get; set; }

    public double TotalDistance {
        get
        {
            return DistanceFromStart + DistanceEstimateToTarget;
        }
    }

    public bool Visited { get; set; }

    public Node(ExtVector position)
    {
        Position = position;
        Visited = false;
    }

}
