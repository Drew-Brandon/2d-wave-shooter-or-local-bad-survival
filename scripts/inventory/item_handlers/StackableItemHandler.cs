namespace Inventory.ItemHandlers;
using Godot;
using System;
using ItemInstances;
using Inventory.Items;


[GlobalClass]
public partial class StackableItemHandler : BaseItemHandler
{
    public int Amount { get => StackableInstance.Amount; }

    public StackableItemType @StackableItemType { get => StackableInstance.StackableItemType; }

    public override Type ItemInstanceType { get => typeof(StackableItemInstance); }

    protected StackableItemInstance StackableInstance { get => (StackableItemInstance)BaseInstance; }

    public StackableItemHandler(BaseItemHandler handler, bool isRestricted) : base(handler, isRestricted)
    { }

    public StackableItemHandler(StackableItemInstance instance, bool isRestricted = true) : base(instance, isRestricted)
    {
        SeekAmount(instance.Amount);
    }

    public void SeekAmount(int targetAmount)
    {
        if (targetAmount <= 0)
        {
            StackableInstance.Amount = 0;
            Empty();
        }
        else if (IsRestricted && targetAmount > StackableInstance.StackableItemType.MaxAmount)
        {
            StackableInstance.Amount = StackableInstance.StackableItemType.MaxAmount;
        }
        else
        {
            StackableInstance.Amount = targetAmount;
        }

        Update();
    }

    public int AddAmount(int amount)
    {
        int prevAmount = StackableInstance.Amount;
        int targetAmount = StackableInstance.Amount + amount;
        SeekAmount(targetAmount);
        return StackableInstance.Amount - prevAmount;
    }

    public bool AddTo(StackableItemHandler other)
    {
        if (other == null)
        {
            throw new NullReferenceException("Cannot add to a null stack.");
        }

        if (BaseInstance.BaseItemType == other.BaseInstance.BaseItemType)
        {
            int added = other.AddAmount(StackableInstance.Amount);
            AddAmount(-added);
            return true;
        }

        return false;
    }

    public bool TakeFrom(StackableItemHandler other)
    {
        if (other == null)
        {
            throw new NullReferenceException("Cannot take from a null stack handler.");
        }

        return other.AddTo(this);
    }

    public bool TakeAmount(StackableItemHandler other)
    {
        if (other != null && BaseInstance.BaseItemType == other.BaseInstance.BaseItemType)
        {
            int taken = -AddAmount(-other.StackableInstance.Amount);
            other.AddAmount(-taken);
            return true;
        }

        return false;
    }

    public StackableItemHandler TakeOff(bool applyRestrictions = true)
    {
        StackableItemInstance stack = (StackableItemInstance)StackableInstance.Clone();
        stack.Amount = 0;

        StackableItemHandler stackHandler = new StackableItemHandler(stack, applyRestrictions);
        stackHandler.TakeFrom(this);
        return stackHandler;
    }

    /*protected override bool OnAddTo(BaseItemHandler other)
    {
        StackableItemHandler otherStack = other as StackableItemHandler; 

        if (otherStack != null && BaseInstance.BaseItemType == otherStack.BaseInstance.BaseItemType)
        {
            int added = otherStack.AddAmount(StackableInstance.Amount);
            AddAmount(-added);
            return true;
        }
        
        return false;
    }

    protected override bool OnTakeFrom(BaseItemHandler other)
    {
        StackableItemHandler otherStack = other as StackableItemHandler; 

        if (otherStack != null && BaseInstance.BaseItemType == otherStack.BaseInstance.BaseItemType)
        {
            int taken = AddAmount(otherStack.StackableInstance.Amount);
            otherStack.AddAmount(-taken);
            return true;
        }
        
        return false;
    }

    protected override bool OnTakeAmount(BaseItemHandler other)
    {
        StackableItemHandler otherStack = other as StackableItemHandler;

        if (otherStack != null && BaseInstance.BaseItemType == otherStack.BaseInstance.BaseItemType)
        {
            int taken = AddAmount(-otherStack.StackableInstance.Amount);
            otherStack.AddAmount(taken);
            return true;
        }

        return false;
    }

    public override BaseItemHandler TakeOff(bool applyRestrictions = true)
    {
        return new StackableItemHandler(this, applyRestrictions);
    }*/
}
