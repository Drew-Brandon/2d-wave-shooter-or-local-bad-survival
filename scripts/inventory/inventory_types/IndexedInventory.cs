namespace Inventory;
using System;
using Godot;
using Inventory.ItemInstances;
using ItemHandlers;

public abstract partial class IndexedInventory : Node2D
{
    public virtual int Size { get; }

    [Export]
    private Vector2 _dropOffset = Vector2.Zero;

    [Export]
    private NodePathResource _dropPath = null;

    [Signal]
    public delegate void InventoryUpdatedEventHandler();

    protected void OnInventoryUpdated()
    {
        EmitSignal(SignalName.InventoryUpdated);
    }

    private void TryToAddAt(int index, BaseItemHandler adder)
    {
        BaseItemHandler target = GetHandler(index);

        if (target == null)
        {
            GetSlot(index).Attach(adder.CloneHandler(true));
            adder.Empty();
        }
    }

    private void TryToAddStackAt(int index, StackableItemHandler adder)
    {
        BaseItemHandler target = GetHandler(index);

        if (target == null)
        {
            GetSlot(index).Attach(adder.TakeOff());
        }
        else
        {
            StackableItemHandler targetStack = target as StackableItemHandler;

            if (targetStack != null)
            {
                targetStack.TakeFrom(adder);
            }
        }
    }

    private void TryToTakeAt(int index, BaseItemHandler taker)
    {
        BaseItemHandler target = GetHandler(index);

        if (target == null && target.ItemType == taker.ItemType)
        {
            taker.Empty();
            target.Empty();
        }
    }

    private void TryToTakeStackAt(int index, StackableItemHandler taker)
    {
        StackableItemHandler target = GetHandler(index) as StackableItemHandler;

        if (target != null)
        {
            target.TakeAmount(taker);
        }
    }

    private void TryToAdd<T>(Action<int, T> addAction, T adder) where T : BaseItemHandler
    {
        for (int i = 0; i < Size; i++)
        {
            addAction.Invoke(i, adder);
            
            if (adder.IsEmpty)
            {
                break;
            }
        }
    }

    private void TryToTake<T>(Action<int, T> takeAction, T taker) where T : BaseItemHandler
    {
        for (int i = 0; i < Size; i++)
        {
            takeAction.Invoke(i, taker);

            if (taker.IsEmpty)
            {
                break;
            }
        }
    }

    public void TryToAdd(BaseItemHandler adder)
    {
        if (adder == null)
        {
            throw new NullReferenceException("Cannot add a null handler to an inventory.");
        }

        StackableItemHandler adderStack = adder as StackableItemHandler;

        if (adderStack == null)
        {
            TryToAdd(TryToAddAt, adder);
        }
        else
        {
            TryToAdd(TryToAddStackAt, adderStack);
        }
    }

    public void TryToAdd(StackableItemHandler adder)
    {
        TryToAdd(TryToAddStackAt, adder);
    }

    public void TryToAdd(BaseItemInstance adder)
    {
        BaseItemHandler adderHandler = adder.CreateHandlerFrom(false);
        TryToAdd(adderHandler);
    }

    public void TryToAdd(StackableItemInstance adder)
    {
        StackableItemHandler adderHandler = new(adder, false);
        TryToAdd(adderHandler);
    }

    public void TryToTake(BaseItemHandler taker)
    {
        if (taker == null)
        {
            throw new NullReferenceException("Cannot take a null handler from an inventory.");
        }

        StackableItemHandler takerStack = taker as StackableItemHandler;

        if (takerStack == null)
        {
            TryToTake(TryToTakeAt, taker);
        }
        else
        {
            TryToTake(TryToTakeStackAt, takerStack);
        }
    }

    public void TryToTake(StackableItemHandler taker)
    {
        TryToTake(TryToTakeStackAt, taker);
    }

    public void TryToTake(BaseItemInstance taker)
    {
        BaseItemHandler takerHandler = new(taker, false);
        TryToTake(takerHandler);
    }

    public void TryToTake(StackableItemInstance taker)
    {
        StackableItemHandler takerHandler = new(taker, false);
        TryToTake(takerHandler);
    }

    public virtual ItemSlot GetSlot(int index)
    {
        throw new NotImplementedException("Method \"GetSlot(int index)\" is not implemented for IndexedInventory.");
    }

    protected virtual void InitSlot(int index)
    {
        throw new NotImplementedException("Method \"InitSlot(int index)\" is not implemented for IndexedInventory.");
    }

    public BaseItemHandler GetHandler(int index)
    {
        return GetSlot(index).Handler;
    }

    private void OnHandlerDropped(DroppedItemInstance dropped)
    {
        Node sceneNode = MiscUtils.GetSceneNodeOf(this);
        sceneNode.GetNode(_dropPath.Path).AddChild(dropped);
        dropped.GlobalPosition = GlobalPosition;
    }

    public override void _Ready()
    {
        for (int i = 0; i < Size; i++)
        {
            InitSlot(i);
            ItemSlot slot = GetSlot(i);

            slot.HandlerAttached += OnInventoryUpdated;
            slot.HandlerUpdated += OnInventoryUpdated;
            slot.HandlerDropped += OnHandlerDropped;
        }
    }

    public override void _ExitTree()
    {
        for (int i = 0; i < Size; i++)
        {
            ItemSlot slot = GetSlot(i);

            slot.HandlerAttached -= OnInventoryUpdated;
            slot.HandlerUpdated -= OnInventoryUpdated;
            slot.HandlerDropped -= OnHandlerDropped;

            if (slot.Handler != null)
            {
                slot.Detach();
            }
        }
    }

    public override string ToString()
    {
        string str = "{ ";
        
        if (Size > 0)
        {
            str += GetSlot(0);

            for (int i = 1; i < Size; i++)
            {
                str += $", {GetSlot(i)}";
            }
        }

        str += " }";
        return str;
    }
}
