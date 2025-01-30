
//(c) Michael T. Lee

using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// The game controller.
/// Deals with setup, and controls/runs game logic.
/// </summary>
public class Controller : MonoBehaviour {

    public GameObject PlayerCamera;
    public float CameraSpeed = 20;
    public GameObject TileContainer;

    private AnimationManager Animation { get; set; }
    private PlayerManager Player { get; set; }
    private CameraManager Camera { get; set; }
    private WorldManager World { get; set; }



    /// <summary>
   /// Do heavy work before the "GameLoop"
   /// </summary>
	private void Awake() {

        //Load factory from file
        var jstr = File.ReadAllText(
            Directory.GetCurrentDirectory() +
            "/Assets/Resources/Tiles/Tiledata.json"
        );
        var factory = JsonConvert.DeserializeObject<TileFactory>(jstr);

        //Setup animation
        Animation =
            new AnimationManager(GetComponent<Animator>());

        //Setup player
        Player = //Set layer to -1 (Z.comp)
            new PlayerManager(GetComponent<Transform>(), GetComponent<Rigidbody2D>(), -1);

        //Setup camera
        Camera =
            new CameraManager(
                PlayerCamera.GetComponent<Camera>(),
                PlayerCamera.GetComponent<RectTransform>(), CameraSpeed
        );

        //Setup world
        World =
            new WorldManager(factory, TileContainer, Player.Transform, GetComponent<LineRenderer>());


	}

    /// <summary>
    /// Fixed frame update time allocated for things like transforming positions
    /// </summary>
	private void FixedUpdate()
    {
        if (Player.NotOnTarget)
        {
            //start movement and animation
            Animation.StartMoveAnimation(Player.Transform, Player.Target);
            Player.MoveToTarget();
        }
        else
        {
            //Reset movement and animation
            Animation.StopMoveAnimation();
            Player.Reposition();

            //Try to get next target
            if (World.WalkPath.Count > 0)
                Player.Target = World.WalkPath.Dequeue();
        }

    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
	private void Update () {
        Camera.GetCameraInput();
		GetMouseInput();
    }



    #region Input

    /// <summary>
    /// Get Target from player mouse click
    /// Also mark path to target continuously.
    /// </summary>
    private void GetMouseInput()
    {
        //Live mouse targeting!!!
        GetMouseTarget();

        //Check for click
        if (!Input.GetMouseButtonDown(0)) return;

        //Reset animation and movement
        Animation.StopMoveAnimation();
        Player.ResetLerpTime();

        //Try to set new path and get+set target to walk to
        World.SetNewPath(Player.Layer);
        if(World.WalkPath.Count > 0) Player.Target = World.WalkPath.Dequeue();
    }

    private void GetMouseTarget()
    {
        //No need to pathfind while moving.
        if (World.WalkPath.Count > 0 || Player.NotOnTarget) return;

        //Get new target candidate
        var candidateTarget = Camera.MouseTarget;
        candidateTarget.Round();

        //Z set to 0 because unity does weird stuff with Z coords in 2D projects
        var playerPosition = new ExtVector(Player.Transform.position) {Z = 0};
        playerPosition.Round();

        //Try to get path to candidate target and mark it
        if(World.Pathfind(playerPosition, candidateTarget))
            World.MarkPath();

    }

    #endregion /Input

    #region Collider

    /// <summary>
    /// Automatically called when a RigidBody2D enters a 2Dcollision
    /// In this project we just call OnStay because Unity has a pretty bad collision system.
    /// (Just try to play Recore if you want proof)
    /// Thus we need to tell it a few extra times that it should block the object.
    /// </summary>
    /// <param name="c">Collision info</param>
    private void OnCollisionEnter2D(Collision2D c)
    {
		OnCollisionStay2D (c);
	}

    /// <summary>
    ///  Automatically called when a RigidBody2D stays in a 2Dcollision
    /// </summary>
    /// <param name="c">Collision info</param>
	private void OnCollisionStay2D(Collision2D c)
    {
        //Cease Movement!!!
        World.WalkPath.Clear();
        Player.ResetTarget();
    }

    #endregion /Collider

}
