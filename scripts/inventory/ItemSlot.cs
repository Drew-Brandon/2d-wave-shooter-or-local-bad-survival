namespace Inventory;
using Godot;
using System;
using ItemHandlers;

public partial class ItemSlot : RefCounted
{
    [Export]
    private BaseItemHandler _handler = null;
    public BaseItemHandler Handler { get => _handler; }

    [Signal]
    public delegate void HandlerChangedEventHandler();

    [Signal]
    public delegate void HandlerAttachedEventHandler();

    [Signal]
    public delegate void HandlerUpdatedEventHandler();

    [Signal]
    public delegate void HandlerDetachedEventHandler();

    [Signal]
    public delegate void HandlerEmptiedEventHandler();

    [Signal]
    public delegate void HandlerDroppedEventHandler(DroppedItemInstance dropped);

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && _handler != null)
        {
            Detach();
        }
    }

    private void OnHandlerUpdate()
    {
        EmitSignal(SignalName.HandlerUpdated);
    }

    private void OnHandlerEmptied()
    {
        EmitSignal(SignalName.HandlerEmptied);
        Detach();
    }

    private void OnHandlerDropped(DroppedItemInstance dropped)
    {
        EmitSignal(SignalName.HandlerDropped, dropped);
    }

    public bool IsAttached()
    {
        return _handler != null;
    }

    private void InternalAttach(BaseItemHandler handler)
    {
        _handler = handler;
        _handler.Updated += OnHandlerUpdate;
        _handler.Emptied += OnHandlerEmptied;
        _handler.Dropped += OnHandlerDropped;
        
        EmitSignal(SignalName.HandlerAttached);
        EmitSignal(SignalName.HandlerUpdated);
    }

    private void InternalDetach()
    {
        _handler.Updated -= OnHandlerUpdate;
        _handler.Emptied -= OnHandlerEmptied;
        _handler.Dropped -= OnHandlerDropped;
        _handler = null;

        EmitSignal(SignalName.HandlerDetached);
    }

    public void Attach(BaseItemHandler handler)
    {
        if (handler == null)
        {
            throw new NullReferenceException("Cannot attach a null reference to an item slot.");
        }
        
        if (_handler != null)
        {
            Detach();
        }

        InternalAttach(handler);
        EmitSignal(SignalName.HandlerChanged);
    }

    public void Detach()
    {
        if (_handler == null)
        {
            throw new NullReferenceException("Cannot detach an item slot that isn't attached to anything.");
        }

        InternalDetach();
        EmitSignal(SignalName.HandlerChanged);
    }

    public void ExchangeWith(ItemSlot other)
    {
        BaseItemHandler handler1 = _handler;
        BaseItemHandler handler2 = other._handler;
        StackableItemHandler stackHandler1 = handler1 as StackableItemHandler;
        StackableItemHandler stackHandler2 = handler2 as StackableItemHandler;
        
        if (stackHandler1 != null && stackHandler2 != null && stackHandler1.AddTo(stackHandler2))
        {
            return;
        }

        if (handler1 != null)
        {
            InternalDetach();
            other.InternalAttach(handler1);

            if (handler2 != null)
            {
                InternalAttach(handler2);
            }
        }
        else if (handler2 != null)
        {
            other.InternalDetach();
            InternalAttach(handler2);
        }

        EmitSignal(SignalName.HandlerChanged);
        other.EmitSignal(SignalName.HandlerChanged);
    }

    public override string ToString()
    {
        if (_handler == null)
        {
            return "handler null";
        }

        return $"{_handler.ItemType.DisplayName} : {_handler.GetShortenedDisplayText()}";
    }
}
