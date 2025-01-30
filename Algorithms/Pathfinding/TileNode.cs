

using System;
using System.Collections.Generic;

public class TileNode : Node,  IEquatable<TileNode> , IComparable<TileNode>{

	public Tile Tile { get; private set; }
	public new TileNode Parent { get; set; }
	public double MoveCost { get; set; }
	public bool IsWalkable {
		get { return !Tile.IsCollider; }
	}


	public TileNode(Tile tile) : base(tile.Position)
	{
		Tile = tile;
	}

	/// <summary>
	/// Standard comparission method for sorting speed.
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public int CompareTo(TileNode other)
	{
		return TotalDistance.CompareTo(other.TotalDistance);
	}

	public bool Equals(TileNode other)
	{
	    if (other == null)
	        return false;
	    return other == this || other.Position.Equals(Position);
	}

	/// <summary>
	/// Ok, I admit this one is propbably not the best hash code....
	/// </summary>
	/// <returns></returns>
	public override int GetHashCode()
	{
		return DistanceFromStart.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format(
            "Tile:[X:{0} Y:{1}] Distance:[{2}]", 
            Tile.Position.X, Tile.Position.Y, DistanceFromStart
        );
	}

	/// <summary>
	/// Finds all neigbhours in the vincinity of a TileNode (Also Diagonally, so up to 8).
	/// Does not add invalid neigbhours or self.
	/// Sorts them to present lowest cost first.
	/// </summary>
	/// <param name="tileDict"></param>
	/// <returns></returns>
    public List<TileNode> GetNeighbors(Dictionary<ExtVector, TileNode> tileDict)
    {
        var neigbhours = new List<TileNode>();

        for (var x = -1; x < 2; x++)
        {
            for (var y = -1; y < 2; y++)
            {
                var candidate = new ExtVector((int)Position.X+x, (int)Position.Y+y);
                if (candidate.Equals(Position)) continue;
                if (!tileDict.ContainsKey(candidate) || tileDict[candidate].Tile.IsCollider)
                    continue;
                neigbhours.Add(tileDict[candidate]);
            }
        }
		neigbhours.ForEach(n=>n.MoveCost = Tile.GetDistance(n.Tile.Position));
	    neigbhours.Sort(Comparison);
        return neigbhours;
    }

	/// <summary>
	/// Custom comparison between two tilenodes. Compares movement cost.
	/// Used for sorting.
	/// </summary>
	/// <param name="tileNode"></param>
	/// <param name="node"></param>
	/// <returns></returns>
	private static int Comparison(TileNode tileNode, TileNode node)
	{
		return
			tileNode.MoveCost == node.MoveCost ? 0 : tileNode.MoveCost < node.MoveCost ? -1 : 1;
	}
}
