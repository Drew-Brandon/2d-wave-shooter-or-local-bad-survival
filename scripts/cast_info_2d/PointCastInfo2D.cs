namespace CastInfo2D;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PointCastInfo2D : CastInfo2D
{
	public PhysicsPointQueryParameters2D CreateQuery(Vector2 pos, ulong canvasInstanceId = 0, Array<Rid> exclude = null)
	{
		PhysicsPointQueryParameters2D query = new()
		{
			Position = pos,
			CollideWithAreas = CollideWithAreas,
			CollideWithBodies = CollideWithBodies,
			CollisionMask = CollisionMask,
			CanvasInstanceId = canvasInstanceId,
			Exclude = exclude
		};

		return query;
	}

	public static PointCastResults2D[] IntersectPoint(PhysicsDirectSpaceState2D spaceState, PhysicsPointQueryParameters2D query)
	{
		Array<Dictionary> resultsDicts = spaceState.IntersectPoint(query);
		
		if (resultsDicts == null)
		{
			return null;
		}

		PointCastResults2D[] results = new PointCastResults2D[resultsDicts.Count];

		for (int i = 0; i < resultsDicts.Count; i++)
		{
			Dictionary resultsDict = resultsDicts[i];

			results[i] = new PointCastResults2D()
			{
				Shape = resultsDict["shape"].AsInt32(),
				ColliderRid = resultsDict["rid"].AsRid(),
				Collider = resultsDict["collider"].AsGodotObject()
			};
		}

		return results;
	}
}
