namespace CustomCasting;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PointCastInfo : CastInfo
{
	[Export]
	private ulong _canvasInstanceId = 0;

	public ulong CanvasInstanceId { get => _canvasInstanceId; }

	public PhysicsPointQueryParameters2D CreateQuery(Vector2 pos, Array<Rid> exclude = null)
	{
		PhysicsPointQueryParameters2D query = new()
		{
			Position = pos,
			CollideWithAreas = CollideWithAreas,
			CollideWithBodies = CollideWithBodies,
			CollisionMask = CollisionMask,
			CanvasInstanceId = _canvasInstanceId,
			Exclude = exclude
		};

		return query;
	}

	public static PointCastResults[] IntersectPoint(Node2D node, PhysicsPointQueryParameters2D query)
	{
		Array<Dictionary> resultsDicts = node.GetWorld2D().DirectSpaceState.IntersectPoint(query);

		if (resultsDicts == null)
		{
			return null;
		}

		PointCastResults[] results = new PointCastResults[resultsDicts.Count];

		for (int i = 0; i < resultsDicts.Count; i++)
		{
			Dictionary resultsDict = resultsDicts[i];

			results[i] = new PointCastResults()
			{
				Shape = resultsDict["shape"].AsInt32(),
				ColliderRid = resultsDict["rid"].AsRid(),
				Collider = resultsDict["collider"].AsGodotObject()
			};
		}

		return results;
	}

	public static PointCastResults[] IntersectPointWithinCanvas(Node2D node, PhysicsPointQueryParameters2D query)
	{
		PhysicsPointQueryParameters2D queryCopy = new()
		{
			Position = query.Position,
			CollideWithAreas = query.CollideWithAreas,
			CollideWithBodies = query.CollideWithBodies,
			CollisionMask = query.CollisionMask,
			CanvasInstanceId = node.GetCanvasLayerNode().GetInstanceId(),
			Exclude = query.Exclude
		};

		return IntersectPoint(node, queryCopy);
	}
}
