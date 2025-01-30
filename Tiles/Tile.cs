
//(C)Michael T. Lee

using UnityEngine;

public class Tile
{
    public int Id { get; private set; }
    public GameObject TileGameObject { get; set; }
    public Collider2D TileCollider { get; private set; }
    public SpriteRenderer TileSpriteRenderer { get; private set; }
    public ExtVector Position { get; private set; }

    public bool IsCollider {
        get { return TileCollider.enabled; }
    }

    public Tile(int id, GameObject tileGameObject, ExtVector position)
    {
        Id = id;
        Position = position;
        TileGameObject = tileGameObject;
        TileCollider = TileGameObject.GetComponent<Collider2D>();
        TileSpriteRenderer = TileGameObject.GetComponent<SpriteRenderer>();
    }

    public void Mark()
    {
        TileSpriteRenderer.color = Color.gray;
    }

    public void UnMark()
    {
        TileSpriteRenderer.color = Color.white;
    }

    /// <summary>
    /// Eucledian distance if I didn't get confused. First tried simple pythagoras code, but it didn't work too well
    /// for this project.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public double GetDistance(ExtVector target)
    {
        //if(IsCollider) return double.PositiveInfinity;
        var distX = Mathf.Abs(target.X - Position.X);
        var distY = Mathf.Abs(target.Y - Position.Y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14*distX +10 * (distY-distX);
        //Math.Sqrt(Math.Pow(target.X - Position.X, 2) + Math.Pow(target.Y - Position.X, 2));
    }



}
