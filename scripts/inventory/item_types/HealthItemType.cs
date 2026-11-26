namespace Inventory.Items;
using Godot;

[GlobalClass]
public partial class HealthItemType : StackableItemType
{
	[Export]
	private float _healAmount = 40f;
	public float HealAmount { get => _healAmount; }
}
