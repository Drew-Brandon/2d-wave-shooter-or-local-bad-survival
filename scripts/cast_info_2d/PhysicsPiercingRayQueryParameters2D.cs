namespace CastInfo2D;
using Godot;
using Godot.Collections;

/// <summary>
/// A query for a raycast that pierces through multiple colliders.
/// </summary>
public partial class PhysicsPiercingRayQueryParameters2D : PhysicsRayQueryParameters2D
{
	/// <summary>
	/// The maximum amount of times that the raycast can pierce through colliders.
	/// </summary>
	public int MaxPierceCount = 0;

	/// <summary>
	/// The collision layer that can be pierced through.
	/// </summary>
	public uint PierceMask = 0;

	/// <summary>
	/// Creates a new query for a piercing raycast.
	/// </summary>
	/// <param name="from">
	/// The position to start from.
	/// </param>
	/// <param name="to">
	/// The position to stop at.
	/// </param>
	/// <param name="collisionMask">
	/// The layer of collision that the raycast can hit.
	/// </param>
	/// <param name="exclude">
	/// The colliders that can be excluded when doing the raycast.
	/// </param>
	/// <returns>
	/// The created query for a piercing raycast.
	/// </returns>
	public static new PhysicsPiercingRayQueryParameters2D Create(Vector2 from, Vector2 to, uint collisionMask, Array<Rid> exclude = null)
	{
        PhysicsPiercingRayQueryParameters2D query = new()
        {
            From = from,
            To = to,
            CollisionMask = collisionMask,
            Exclude = exclude
        };

        return query;
	}
}
