namespace Inventory.ItemInstances;
using Godot;
using System;
using System.Collections.Generic;
using Items;
using Inventory.ItemHandlers;

/// <summary>
/// Represents a stack/collection of items.
/// </summary>
[GlobalClass, Tool]
public partial class StackableItemInstance : BaseItemInstance
{
	public override Type ItemTypeClass { get => typeof(StackableItemType); }

    public override Type HandlerType { get => typeof(StackableItemHandler); }

	/// <summary>
	/// The current amount of items within the stack.
	/// </summary>
	[Export]
	public int Amount = 0;

	public StackableItemType @StackableItemType { get => (StackableItemType)BaseItemType; set => BaseItemType = value; }

	protected override void AddFullDisplayTextTo(List<string> strings)
	{
		base.AddFullDisplayTextTo(strings);
		strings.Add($"Amount: {Amount}/{StackableItemType.MaxAmount}");
	}

    public override string GetShortenedDisplayText()
    {
        return $"{Amount}";
    }

	public override bool EqualsInstance(BaseItemInstance instance)
    {
        bool equalBase = base.EqualsInstance(instance);

        if (!equalBase)
        {
            return false;
        }

        StackableItemInstance stackable = instance as StackableItemInstance;
        return stackable != null && Amount == stackable.Amount;
    }

    public override BaseItemHandler CreateHandlerFrom(bool isRestricted = true)
    {
        return new StackableItemHandler(this, isRestricted);
    }
}
