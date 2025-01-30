
//(c) Michael T. Lee

using System;
using UnityEngine;

/// <summary>
/// Manages player movement (and properties if any)
/// </summary>
public class PlayerManager
{
    public ExtVector Target { get; set; } //Target that player should move towards.
    public Transform Transform { get; set; }
    public Rigidbody2D PlayerRigidbody { get; set; }
    private float LerpTime { get; set; } //This basically manages how fast movement interpolation becomes over time
    public int Layer { get; private set; } //Layer (Z-component) at which the player is located.

    public bool NotOnTarget //Check if player is closing in/ is on target.
    {
        get
        {
            const double tolerance = .1; //Tolerance of target approaching before repositioning
            var posX = Transform.position.x;
            var posY = Transform.position.y;
            var position = new ExtVector(Transform.position);
            position.Round();
            return Math.Abs(posX - Target.X) > tolerance || Math.Abs(posY - Target.Y) > tolerance;
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="rigidbody2D"></param>
    /// <param name="layer"></param>
    public PlayerManager(Transform transform, Rigidbody2D rigidbody2D, int layer)
    {
        Transform = transform;
        PlayerRigidbody = rigidbody2D;
        Target = new ExtVector(Transform.position);
        Layer = layer;
    }

    #region Movement

    /// <summary>
    /// Moves the "player" object towards a selected target using lerp to interpolate the move
    /// </summary>
    public void MoveToTarget() {

        PlayerRigidbody.MovePosition(Target.LerpTo(Transform.position, LerpTime, lz: false));
        LerpTime += .5f / Time.fixedDeltaTime;
    }

    /// <summary>
    /// Repositions the "player" object to whole number coordinates and stops movement
    /// </summary>
    public void Reposition() {

        var newPos = new ExtVector(Transform.position) { Z = Layer };
        newPos.Round();
        PlayerRigidbody.MovePosition(newPos.Vector3);
        LerpTime = 0f;

    }

    #endregion /Movement


    public void ResetLerpTime()
    {
        LerpTime = 0f; //Reset LerpTime
    }

    public void ResetTarget()
    {
        Target = new ExtVector(Transform.position);
        Target.Round();
    }
}
