using System;
using Godot;
using HealthSources;
using Inventory.ItemHandlers;
using Inventory.Items;
using Puppets.Bodies;

public partial class HealingTool : UseableTool
{
    public override Type ItemHandlerType { get => typeof(StackableItemHandler); }

    public override Type UserType { get => typeof(IHealthSource); }

    public StackableItemHandler HealthItemHandler { get => (StackableItemHandler)ItemHandler; }

    public IHealthSource HealthSourceUser { get => (IHealthSource)User; }

    protected override bool OnHandlerSet(BaseItemHandler handler)
    {
        if (handler != null && handler.ItemType is not HealthItemType)
        {
            throw new ArgumentException("Handler and instance for healing tool should have a health item type");
        }

        return true;
    }

    protected override void OnSuccessfullyUsed(Vector2 usePosition)
    {
        base.OnSuccessfullyUsed(usePosition);
        HealthItemType healthItemType = (HealthItemType)HealthItemHandler.ItemType;
        HealthSourceUser.AddHealth(healthItemType.HealAmount);
        HealthItemHandler.AddAmount(-1);
    }
}
