namespace CustomCasting;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Stores information regarding a piercing raycast.
/// </summary>
[GlobalClass]
public partial class PiercingRayCastInfo : RayCastInfo
{
	[Export]
	private int _maxPierceCount = 0;

	[Export(PropertyHint.Layers2DPhysics)]
	private uint _pierceMask = 0;

	/// <summary>
	/// The layer of collision that can be pierced through.
	/// </summary>
	public uint PierceMask { get => _pierceMask; }

    /// <summary>
    /// Creates a new query with the object's current config.
    /// </summary>
    /// <param name="from">
    /// The position to start the raycast at.
    /// </param>
    /// <param name="to">
    /// The position to end the raycast at.
    /// </param>
    /// <param name="exclude">
    /// The colliders that can be excluded when doing the raycast.
    /// </param>
    /// <returns>
	/// The created query for a piercing raycast.
	/// </returns>
    public new PhysicsPiercingRayQueryParameters2D CreateQuery(Vector2 from, Vector2 to, Godot.Collections.Array<Rid> exclude = null)
	{
		PhysicsPiercingRayQueryParameters2D query = PhysicsPiercingRayQueryParameters2D.Create(from, to, CollisionMask, exclude);
		query.CollideWithAreas = CollideWithAreas;
		query.CollideWithBodies = CollideWithBodies;
		query.HitFromInside = HitFromInside;
		query.MaxPierceCount = _maxPierceCount;
		query.PierceMask = _pierceMask;
		return query;
	}

    /// <summary>
    /// Conducts an intersection with the specified query.
    /// </summary>
    /// <param name="node">
	/// The node that belongs to the world space to do the intersection in.
	/// </param>
    /// <param name="query">
	/// The query to do the intersection with.
	/// </param>
    /// <returns>
	/// The results of the intersection.
	/// </returns>
    public static RayCastResults[] IntersectRay(Node2D node, PhysicsPiercingRayQueryParameters2D query)
	{
		// Start off with a base query.
		List<RayCastResults> resultsList = new();
		PhysicsPiercingRayQueryParameters2D curQuery = PhysicsPiercingRayQueryParameters2D.Create(
			query.From,
			query.To,
			query.CollisionMask,
			query.Exclude
		);

		curQuery.CollideWithAreas = query.CollideWithAreas;
		curQuery.CollideWithBodies = query.CollideWithBodies;
		curQuery.HitFromInside = query.HitFromInside;
		curQuery.MaxPierceCount = query.MaxPierceCount;
		curQuery.PierceMask = query.PierceMask;

		/* 
		 * Perform multiple raycasts
		 * with most of them starting from where the previous ended.
		 */
		for (int i = 0; i != query.MaxPierceCount; i++)
		{
			RayCastResults results = RayCastInfo.IntersectRay(node, curQuery);
			CollisionObject2D col = results.Collider as CollisionObject2D;
			resultsList.Add(results);

			if (col == null || (col.CollisionLayer | query.PierceMask) == 0)
			{
				break;
			}

			curQuery.From = results.Position;
			curQuery.Exclude.Add(results.ColliderRid);
		}

		return resultsList.ToArray();
	}
}
