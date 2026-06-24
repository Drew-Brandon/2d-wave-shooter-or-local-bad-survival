namespace Inventory;
using Godot;

public partial class BreakableInstance : ItemInstanceGeneric<BreakableItem>
{
	[Export]
	public float Durability = 0f;

	public BreakableInstance()
	{ }

	public BreakableInstance(BreakableItem item, float durability) : base(item)
	{
		Durability = durability;
	}
}
