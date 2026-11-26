namespace CastInfo2D;
using Godot;
using Godot.Collections;

/// <summary>
/// Stores information regarding casting with a ray.
/// </summary>
[GlobalClass, Tool]
public partial class RayCastInfo2D : CastInfo2D
{
    [Export]
    private bool _hitFromInside = false;

    /// <summary>
    /// Whether or not the ray can hit colliders from the inside.
    /// </summary>
    public bool HitFromInside { get => _hitFromInside; }

    /// <summary>
    /// Creates a query that can be used to test for collisions.
    /// </summary>
    /// <param name="from">
    /// The position that the ray will start from.
    /// </param>
    /// <param name="to">
    /// The position that the ray will end at.
    /// </param>
    /// <param name="exclude">
    /// What to exclude in the casting.
    /// </param>
    /// <returns>
    /// A query used to cast with.
    /// </returns>
    public PhysicsRayQueryParameters2D CreateQuery(Vector2 from, Vector2 to, Array<Rid> exclude = null)
    {
        PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(from, to, CollisionMask, exclude);
        query.CollideWithAreas = CollideWithAreas;
        query.CollideWithBodies = CollideWithBodies;
        query.HitFromInside = _hitFromInside;
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
    /// The result of the intersection.
    /// </returns>
    public static RayCastResults2D IntersectRay(PhysicsDirectSpaceState2D spaceState, PhysicsRayQueryParameters2D query)
    {
        Dictionary resultsDict = spaceState.IntersectRay(query);
        RayCastResults2D resultsStruct;

        if (resultsDict.Count > 0)
        {
            resultsStruct = new RayCastResults2D
            {
                Shape = resultsDict["shape"].AsInt32(),
                ColliderRid = resultsDict["rid"].AsRid(),
                Normal = resultsDict["normal"].AsVector2(),
                Position = resultsDict["position"].AsVector2(),
                Collider = resultsDict["collider"].AsGodotObject()
            };
        }
        else
        {
            resultsStruct = new RayCastResults2D()
            {
                Position = query.To
            };
        }

        return resultsStruct;
    }
}