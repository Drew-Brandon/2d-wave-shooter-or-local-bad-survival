namespace Inventory;
using Godot;

[GlobalClass]
public partial class BreakableItem : BaseItem
{
	[Export]
	private float _maxDurability = 10f;

	public float MaxDurability { get => _maxDurability; }
}
