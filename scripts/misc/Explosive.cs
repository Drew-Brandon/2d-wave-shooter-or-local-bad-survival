using Godot;
using System;
using CastInfo2D;
using HealthSources;

/// <summary>
/// Represents a node that can be exploded.
/// </summary>
public partial class Explosive : Node2D
{
	private bool _hasExploded = false;

	[Export]
	private int _rayCount = 16;

	[Export]
	private float _maxDamage = 100f;

	[Export]
	private float _maxForce = 256f;

	[Export]
	private float _range = 256f;

	[Export]
	private RayCastInfo2D _sightCast = null;

	[Export]
	private CollisionObject2D[] _ignore;

	private Godot.Collections.Array<Rid> _ignoreRid;

	[Signal]
	public delegate void ExplodedEventHandler();

	private float GetStrength(float distance)
	{
		return 1f - distance / _range;
	}

	public override void _Ready()
	{
		_ignoreRid = new Godot.Collections.Array<Rid>();

		for (int i = 0; i < _ignore.Length; i++)
		{
			_ignoreRid.Add(_ignore[i].GetRid());
		}

		Array.Clear(_ignore);
	}

	/// <summary>
	/// Explodes the node.
	/// </summary>
	public void Explode()
	{
		if (_hasExploded)
		{
			return;
		}

		// Scan for all nodes within the explosion's range.
		_hasExploded = true;
		
		/*PhysicsShapeQueryParameters2D rangeQuery = _rangeCast.CreateQuery(GlobalTransform, _ignoreRid);
		ShapeCastResults2D[] rangeResults = ShapeCastInfo2D.IntersectShape(this, rangeQuery);

		for (int i = 0; i < rangeResults.Length; i++)
		{
			PhysicsBody2D body = rangeResults[i].Collider as PhysicsBody2D;
            IHealthSource healthSource = body as IHealthSource;
			RigidBody2D rigidBody = body as RigidBody2D;

			if (healthSource != null || rigidBody != null)
			{
				// Check if the node is in the explosion's line of sight.
				PhysicsRayQueryParameters2D sightQuery = _sightCast.CreateQuery(GlobalPosition, body.GlobalPosition);
				RayCastResults2D sightResults = RayCastInfo2D.IntersectRay(GetWorld2D().DirectSpaceState, sightQuery);

				if (sightResults.Collider == null || sightResults.Collider == body)
				{
					float distanceTo = GlobalPosition.DistanceTo(body.GlobalPosition);
					Vector2 directionTo = GlobalPosition.DirectionTo(body.GlobalPosition);

					if (healthSource != null)
					{
						float damage = GetStrength(distanceTo) * -_maxDamage;
						healthSource.AddHealth(damage, this);
					}
					
					if (rigidBody != null)
					{
						float force = GetStrength(distanceTo) * _maxForce;
						rigidBody.ApplyImpulse(directionTo * force, rigidBody.GlobalPosition - sightResults.Position);
					}
				}
			}
		}*/

		/*for (int i = 0; i < _rayCount; i++)
		{
			float angle = i * Mathf.Pi * 2f / _rayCount;
			PhysicsRayQueryParameters2D sightQuery = _sightCast.CreateQuery(GlobalPosition, body.GlobalPosition);
			RayCastResults2D sightResults = RayCastInfo2D.IntersectRay(GetWorld2D().DirectSpaceState, sightQuery);

			PhysicsBody2D body = rangeResults[i].Collider as PhysicsBody2D;
            IHealthSource healthSource = body as IHealthSource;
			RigidBody2D rigidBody = body as RigidBody2D;

			if (healthSource != null || rigidBody != null)
			{
				// Check if the node is in the explosion's line of sight.
				

				if (sightResults.Collider == null || sightResults.Collider == body)
				{
					float distanceTo = GlobalPosition.DistanceTo(body.GlobalPosition);
					Vector2 directionTo = GlobalPosition.DirectionTo(body.GlobalPosition);

					if (healthSource != null)
					{
						float damage = GetStrength(distanceTo) * -_maxDamage;
						healthSource.AddHealth(damage, this);
					}
					
					if (rigidBody != null)
					{
						float force = GetStrength(distanceTo) * _maxForce;
						rigidBody.ApplyImpulse(directionTo * force, rigidBody.GlobalPosition - sightResults.Position);
					}
				}
			}
		}*/

		EmitSignal(SignalName.Exploded);
		QueueFree();
	}
}
