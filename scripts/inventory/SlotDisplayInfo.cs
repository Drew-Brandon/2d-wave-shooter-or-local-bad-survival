namespace Inventory;
using Godot;

public partial class SlotDisplayInfo : RefCounted
{
	public int Int = 0;
	public float Float = 0f;
	public Texture2D Icon = null;

	public SlotDisplayInfo(Texture2D icon = null, int intVal = 0, float floatVal = 0f)
	{
		Icon = icon;
		Int = intVal;
		Float = floatVal;
	}
}
