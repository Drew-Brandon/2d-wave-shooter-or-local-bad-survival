namespace Inventory.Items;
using Godot;
using Inventory.ItemInstances;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BreakableItemType : BaseItemType
{
	public override Type ItemInstanceType { get => typeof(BreakableItemInstance); }

	[Export]
	private float _maxDurability = 10f;

	public float MaxDurability { get => _maxDurability; }

    protected override void AddFullDisplayTextTo(List<string> strings)
	{
		base.AddFullDisplayTextTo(strings);
		strings.Add($"Max Durability: {_maxDurability}");
	}
}
