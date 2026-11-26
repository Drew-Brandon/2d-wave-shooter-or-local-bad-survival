namespace Inventory.ItemHandlers;
using System;
using Godot;
using Inventory.ItemInstances;
using Inventory.Items;


[GlobalClass]
public partial class BreakableItemHandler : BaseItemHandler
{
    public float Durability { get => BreakableInstance.Durability; }

    public BreakableItemType @BreakableItemType { get => BreakableInstance.BreakableItemType; }

    public override Type ItemInstanceType { get => typeof(BreakableItemInstance); }

    protected BreakableItemInstance BreakableInstance { get => (BreakableItemInstance)BaseInstance; }
    
    public BreakableItemHandler(BaseItemHandler handler, bool isRestricted) : base(handler, isRestricted)
    { }

    public BreakableItemHandler(BreakableItemInstance instance, bool isRestricted = true) : base(instance, isRestricted)
    {
        SeekDurability(instance.Durability);
    }

    public void SeekDurability(float targetDurability)
    {
        if (targetDurability <= 0f)
        {
            BreakableInstance.Durability = 0f;
            Empty();
        }
        else if (IsRestricted && targetDurability > BreakableInstance.BreakableItemType.MaxDurability)
        {
            BreakableInstance.Durability = BreakableInstance.BreakableItemType.MaxDurability;
        }
        else
        {
            BreakableInstance.Durability = targetDurability;
        }

        Update();
    }

    public float AddDurability(float amount)
    {
        float prevDurability = BreakableInstance.Durability;
        float targetDurability = BreakableInstance.Durability + amount;
        SeekDurability(targetDurability);
        return BreakableInstance.Durability - prevDurability;
    }
}
