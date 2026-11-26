namespace Inventory.ItemInstances;
using Godot;
using System;
using System.Collections.Generic;
using Items;
using Inventory.ItemHandlers;


[GlobalClass, Tool]
public partial class BreakableItemInstance : BaseItemInstance
{
    public override Type ItemTypeClass { get => typeof(BreakableItemType); }

	[Export]
	public float Durability = 0f;

	public BreakableItemType @BreakableItemType { get => (BreakableItemType)BaseItemType; set => BaseItemType = value; }

    protected override void AddFullDisplayTextTo(List<string> strings)
    {
        base.AddFullDisplayTextTo(strings);
		strings.Add($"Durability: {Durability}/{BreakableItemType.MaxDurability}");
    }

    public override string GetShortenedDisplayText()
    {
        return $"{Mathf.CeilToInt(Durability / BreakableItemType.MaxDurability * 100f)}%";
    }

    public override bool EqualsInstance(BaseItemInstance instance)
    {
        bool equalBase = base.EqualsInstance(instance);

        if (!equalBase)
        {
            return false;
        }

        BreakableItemInstance breakable = instance as BreakableItemInstance;
        return breakable != null && Durability == breakable.Durability;
    }

    public override BaseItemHandler CreateHandlerFrom(bool isRestricted = true)
    {
        return new BreakableItemHandler(this, isRestricted);
    }
}
