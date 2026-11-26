using System.Collections.Generic;
using Godot;
using Puppets.Bodies;

public partial class PullableRigidBody2D : RigidBody2D, IInteractable
{
	[Export]
	private float _pullRange = 128f;

	private Node2D _pullPoint = null;

	private List<Vector2> _pullMotions = new();

	public bool StartInteraction(PlayerBodyTop2D player)
	{
		if (GlobalPosition.DistanceTo(player.GlobalPosition) <= _pullRange)
		{
			FreezeMode = FreezeModeEnum.Kinematic;
			Freeze = true;

			_pullPoint = new Node2D();
			player.Swivel.AddChild(_pullPoint);
			_pullPoint.GlobalPosition = GlobalPosition;
			
			PhysicsServer2D.BodyAddCollisionException(player.GetRid(), GetRid());
			return true;
		}

		return false;
	}

	public bool PhysicsProcessInteraction(PlayerBodyTop2D player, double delta)
	{
		GlobalRotation = _pullPoint.GlobalRotation;
		MoveAndCollide(_pullPoint.GlobalPosition - GlobalPosition);
		return GlobalPosition.DistanceTo(player.GlobalPosition) <= _pullRange;
	}

	public void StopInteraction(PlayerBodyTop2D player)
	{
		Freeze = false;
		FreezeMode = FreezeModeEnum.Static;
		PhysicsServer2D.BodyRemoveCollisionException(player.GetRid(), GetRid());
	}
}
