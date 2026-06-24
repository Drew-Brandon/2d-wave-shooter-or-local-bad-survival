using Godot;
using System.Collections.Generic;

public partial class Gizmos2DSource
{
	private List<Rid> _gizmoRids = new List<Rid>();
	public int GizmoCount { get => _gizmoRids.Count; }

	~Gizmos2DSource()
	{
		Clear();
	}

	public void Clear()
	{
		for (int i = 0; i < _gizmoRids.Count; i++)
		{
			FreeGizmo(_gizmoRids[i]);
		}

		_gizmoRids.Clear();
	}

	public void AddCircle(Vector2 pos, float radius, Color color, Rid parentRid, bool aa = false)
	{
		_gizmoRids.Add(CreateCircle(pos, radius, color, parentRid, aa));
	}

	public void AddRect(Rect2 rect, Color color, Rid parentRid, bool aa = false)
	{
		_gizmoRids.Add(CreateRect(rect, color, parentRid, aa));
	}

	public void AddLine(Vector2 from, Vector2 to, Color color, Rid parentRid, float width = -1f, bool aa = false)
	{
		_gizmoRids.Add(CreateLine(from, to, color, parentRid, width, aa));
	}

	public static void FreeGizmo(Rid gizmoRid)
	{
		RenderingServer.CanvasItemClear(gizmoRid);
		RenderingServer.FreeRid(gizmoRid);
	}

	public static Rid CreateItem(Rid parentRid)
	{
		Rid itemRid = RenderingServer.CanvasItemCreate();
		RenderingServer.CanvasItemSetParent(itemRid, parentRid);
		return itemRid;
	}

	public static Rid CreateCircle(Vector2 pos, float radius, Color color, Rid parentRid, bool aa = false)
	{
		Rid itemRid = CreateItem(parentRid);
		RenderingServer.CanvasItemAddCircle(itemRid, pos, radius, color, aa);
		return itemRid;
	}

	public static Rid CreateRect(Rect2 rect, Color color, Rid parentRid, bool aa = false)
	{
		Rid itemRid = CreateItem(parentRid);
		RenderingServer.CanvasItemAddRect(itemRid, rect, color, aa);
		return itemRid;
	}

	public static Rid CreateLine(Vector2 from, Vector2 to, Color color, Rid parentRid, float width = -1f, bool aa = false)
	{
		Rid itemRid = CreateItem(parentRid);
		RenderingServer.CanvasItemAddLine(itemRid, from, to, color, width, aa);
		return itemRid;
	}
}
