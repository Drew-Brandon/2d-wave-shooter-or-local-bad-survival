namespace Inventory.Items;
using Godot;
using Inventory.ItemInstances;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class StackableItemType : BaseItemType
{
    public override Type ItemInstanceType { get => typeof(StackableItemInstance); }

	[Export]
	private int _maxAmount = 16;

	/// <summary>
	/// The maximum amount of items that can be in a stack.
	/// </summary>
	public int MaxAmount { get => _maxAmount; }

    protected override void AddFullDisplayTextTo(List<string> strings)
    {
        base.AddFullDisplayTextTo(strings);
		strings.Add($"Max Stack Size: {_maxAmount}");
    }
}
