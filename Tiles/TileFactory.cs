
//(c) Michael T. Lee

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[DataContract]
public class TileFactory {

	private readonly GameObject _tile;
	private readonly SpriteRenderer _fSpriteRenderer;
	private readonly BoxCollider2D _collider;
	private int Counter { get; set; }

    private static readonly Dictionary<string, Dictionary<string, Sprite>> _stock =
        new Dictionary<string, Dictionary<string, Sprite>>();

    [DataMember]
    private Dictionary<string, Dictionary<char, string>> MazeDictionary { get; set; }
    [DataMember]
    private Dictionary<string, string> TileSets { get; set; }
    [DataMember]
    private string MazeColliderList { get; set; }

	public Transform Transform { get; set; }

    [JsonConstructor]
    public TileFactory(
        Dictionary<string, Dictionary<char, string>> mazeDictionary,
        Dictionary<string,string> tileSets,
        string mazeColliderList
        )
    {
        _tile = Resources.Load("Prefabs/Tile") as GameObject;
		_fSpriteRenderer = _tile.GetComponent<SpriteRenderer>();
		_collider = _tile.GetComponent<BoxCollider2D>();
        MazeDictionary = mazeDictionary;
        TileSets = tileSets;
        MazeColliderList = mazeColliderList;
    }

	private Sprite GetSprite(char c, string tileSet){

        if (!TileSets.ContainsKey(tileSet)) return null;

        if (!_stock.ContainsKey(tileSet))
            _stock.Add(
                tileSet, 
                Resources.LoadAll<Sprite>(TileSets[tileSet]).ToDictionary(spr => spr.name, spr => spr)
           );

        return !MazeDictionary[tileSet].ContainsKey(c) ? //Actually, this would be the programmers fault.
            null : _stock[tileSet][MazeDictionary[tileSet][c]];
	}

    /// <summary>
    /// Get a tile GameObject based on tileset name and char "translation".
    /// Configuration for this can be found in Resources/Tiles/TileData.json
    /// A tile consists of a Collider2D and a SpriteRenderer
    /// </summary>
    /// <param name="c">The char to be "translated" into a sprite</param>
    /// <param name="tileSet">Name of the tileset to load the sprite from</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <returns></returns>
	public Tile GetTile(char c, string tileSet, int x, int y)
	{
        _fSpriteRenderer.sprite = GetSprite(c, tileSet);
		_tile.name = Counter.ToString();
		Counter++; //Set tile ID
		//Activate collider if specified in maze collider list
		_collider.enabled = MazeColliderList.Contains (c);
		var newTile =
			(Transform == null)
				? Object.Instantiate(_tile, new Vector2(x, y), Quaternion.identity)
				: Object.Instantiate(_tile, new Vector2(x, y), Quaternion.identity, Transform);
        return new Tile(Counter, newTile, new ExtVector(newTile.transform.position));
	}

}
