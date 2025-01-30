//(c) Michael T. Lee

using System;
using UnityEngine;

#region Move
public enum Move {
	Up = 1, Right = 2, Down = -1, Left = -2, Idle = 0
}

#endregion /Move

/// <summary>
/// Manages animations
/// </summary>
public class AnimationManager
{
	private Animator Animator { get; set; }
	private bool IsMoving { get; set; }

	public AnimationManager(Animator animator)
	{
		Animator = animator;
	}


	#region Animation
	/// <summary>
	/// Start 2D movement animation
	/// </summary>
	public void StartMoveAnimation(Transform player, ExtVector target)
	{
		if (IsMoving) return;
		var posX = player.position.x;
		var posY = player.position.y;
		IsMoving = true;
		var direction =
			Math.Abs(target.X - posX) > Math.Abs(target.Y - posY)
				? target.X > posX ? Move.Right : Move.Left
				: target.Y > posY ? Move.Up : Move.Down;
		Animator.SetInteger("Move", (int)direction);
	}

	/// <summary>
	/// Stop 2D Movement animation
	/// </summary>
	public void StopMoveAnimation()
	{
		IsMoving = false;
		Animator.SetInteger("Move", (int)Move.Idle);
	}

	#endregion /Animation

}
