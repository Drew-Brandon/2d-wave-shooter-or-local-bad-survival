using Godot;

public static class CanvasItemUtils
{
    public static readonly Color DEFAULT_COLOR = new(1f, 0f, 0f, 0.5f);

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

    public static void FreeCanvasItem(Rid canvasItem)
	{
		RenderingServer.CanvasItemClear(canvasItem);
		RenderingServer.FreeRid(canvasItem);
	}

    public static void TryFreeCanvasItem(Rid canvasItem)
    {
        if (canvasItem.IsValid)
        {
            FreeCanvasItem(canvasItem);
        }
    }
}
