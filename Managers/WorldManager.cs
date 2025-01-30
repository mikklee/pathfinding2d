
//(c) Michael T. Lee

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages drawing of objects and tiles, as well as pathfinding.
/// </summary>
public class WorldManager
{
    private Dictionary<string, Sprite> TileSet { get; set; }
    private List<Vector2> EntryPoints { get; set; }
    private TileFactory Factory { get; set; }
    public Dictionary<ExtVector,TileNode> TileDict { get; set; }
    private List<Tile> MarkedTiles { get; set; }
    private LineRenderer LineMaker { get; set; }
    public Queue<ExtVector> WalkPath { get; set; }
    private AstarPathfinder Pathfinder { get; set; }
    private List<TileNode> Path { get; set; }
    private Transform PlayerTransform { get; set; }


    public WorldManager(TileFactory factory, GameObject parentGameObject, Transform playerTransform, LineRenderer lineMaker)
    {

        Factory = factory;

        TileDict = new Dictionary<ExtVector, TileNode>();
        WalkPath = new Queue<ExtVector>();
        MarkedTiles = new List<Tile>();
        Path = new List<TileNode>();

        PlayerTransform = playerTransform;

        MarkedTiles = new List<Tile>();

        Factory.Transform = parentGameObject.transform; //Allocate tile objects with this as parent.
        CreateMaze(60,60);

        LineMaker = lineMaker;

        Pathfinder = new AstarPathfinder(TileDict);
    }

    /// <summary>
    /// Randomly generate a maze and instantiate the tiles.
    /// </summary>
    /// <param name="x">Maze width in tiles</param>
    /// <param name="y">Maze Height in tiles</param>
    private void CreateMaze(int x, int y)
    {
        //Specify maze entry tiles
        EntryPoints = new List<Vector2>
            { new Vector2(0, 1), new Vector2(0, 2), new Vector2(x, y - 1), new Vector2(x, y - 2)};

        //Get Maze structure
        var maze = new Maze(x, y);
        var result = maze.GetMaze();

        //Select tileset
        const string ts = "Hideout";

        try
        {
            //Insert maze into Game
            for (var i = 0; i < result.Count; i++)
            {
                for (var j = 0; j < result[i].Length; j++)
                {
                    AddTileToPathfindingNet(EntryPoints.Contains(new Vector2(j, i))
                        ? Factory.GetTile('$', ts, j, i)
                        : Factory.GetTile(result[j][i], ts, j, i));
                }
            }
        }
        catch (IndexOutOfRangeException)
        {
            Debug.Log("Failed");
        }

        //Insert outer rim
        for (var j = 0; j <= y; j++) {
            Factory.GetTile(' ', ts, -1, j);
            Factory.GetTile(' ', ts, x + 1, j);
            Factory.GetTile(' ', ts, j, -1);
            Factory.GetTile(' ', ts, j, y + 1);
        }

    }

    /// <summary>
    /// Mark the target tile
    /// </summary>
    /// <param name="tile">Target tile</param>
    public void MarkTile(Tile tile)
    {
        if (tile == null) return;
        tile.Mark();
        MarkedTiles.Add(tile);
    }

    /// <summary>
    /// Unmark any targeted tiles
    /// </summary>
    public void UnmarkTiles()
    {
        MarkedTiles.ForEach(t => t.UnMark());
        MarkedTiles.Clear();
    }

    /// <summary>
    /// Pathfind path and set it as new path.
    /// </summary>
    /// <param name="position">position from where to pathfind</param>
    /// <param name="candidateTarget">target</param>
    /// <returns>True if new path was set</returns>
    public bool Pathfind(ExtVector position, ExtVector candidateTarget)
    {
        if (!Pathfinder.TileDict.ContainsKey(candidateTarget) || !Pathfinder.TileDict.ContainsKey(position))
            return false;

        try
        {
            var newPath =
                Pathfinder.GetPath(
                    position,
                    candidateTarget
                );
            Path = newPath;
            return true;
        }
        catch (AstarNoPathException)
        {
            return false;
        }

    }

    /// <summary>
    /// Mark new path in game
    /// </summary>
    public void MarkPath()
    {
        try
        {
            var targetTile = Path[Path.Count - 1].Tile;

            UnmarkTiles();
            MarkTile(targetTile);
            RenderLine(Path, targetTile.Position.Vector3);
        }
        catch (Exception)
        {
            //If target tile does not exist, continue with last highlight
        }
    }

    /// <summary>
    /// Render a line between a list of tileNodes
    /// </summary>
    /// <param name="path">List of tilenodes</param>
    /// <param name="end">The end of the line / the target. The pathfinding list does not have this</param>
    private void RenderLine(IEnumerable<TileNode> path, Vector3 end) {
        var start = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y);
        var color = new Color(.1f, .1f, .1f, .3f);
        LineMaker.startColor = color;
        LineMaker.endColor = color;
        var pathVectors = new List<Vector3>();
        pathVectors.Add(start);
        pathVectors.AddRange(path.Select(node => node.Tile.Position.Vector3));
        pathVectors.Add(end);

        LineMaker.numPositions = pathVectors.Count;
        LineMaker.SetPositions(pathVectors.ToArray());
    }

    private void AddTileToPathfindingNet(Tile tile)
    {
        TileDict.Add(tile.Position,new TileNode(tile));
    }


    /// <summary>
    /// Set the new path as active.
    /// </summary>
    /// <param name="layer">The "Z"-layer at which the player moves</param>
    public void SetNewPath(int layer)
    {
       WalkPath.Clear();
       Path.ForEach(n=>WalkPath.Enqueue(n.Position.CloneToLayer(layer)));
    }
}

