namespace Inventory.UI;
using System;
using Godot;
using Godot.Collections;

public partial class ItemSlotUI : Control
{
    private bool _popupInvoked = false;
    public bool PopupInvoked { get => _popupInvoked; }

    private bool _canDrag = true;
    public bool CanDrag { get => _canDrag; }

    private ItemSlot _slot = null;
    public ItemSlot Slot { get => _slot; }

    [Export]
    private ListPopupRelay _popup = null;
    public ListPopupRelay Popup { get => _popup; set => _popup = value; }

    [Export]
    private PackedScene _previewScene = null;

    [Signal]
    public delegate void IconUpdatedEventHandler(Texture2D icon);

    [Signal]
    public delegate void LabelUpdatedEventHandler(string label);

    [Signal]
    public delegate void DraggedEventHandler();

    [Signal]
    public delegate void DroppedEventHandler();

    [Signal]
    public delegate void BrushedEventHandler();

    [Signal]
    public delegate void HighlightedEventHandler();

    public void Brush()
    {
        EmitSignal(SignalName.Brushed);
    }

    public void Highlight()
    {
        EmitSignal(SignalName.Highlighted);
    }

    private void Update()
    {
        EmitSignal(SignalName.IconUpdated, _slot.Handler.ItemType.Icon);
        EmitSignal(SignalName.LabelUpdated, _slot.Handler.GetShortenedDisplayText());
    }

    private void Clear()
    {
        EmitSignal(SignalName.IconUpdated, new Variant());
        EmitSignal(SignalName.LabelUpdated, "");
    }

    public void UnlockDragging()
    {
        if (_canDrag)
        {
            throw new InvalidOperationException("Dragging is already unlocked for slot.");
        }

        _canDrag = true;
    }

    public void LockDragging()
    {
        if (!_canDrag)
        {
            throw new InvalidOperationException("Dragging is already locked for slot.");
        }

        _canDrag = false;
        LosePopup();

        if (GetViewport().GuiIsDragging())
        {
            GetViewport().GuiCancelDrag();
        }
    }

    public void Attach(ItemSlot slot)
    {
        if (slot == null)
        {
            throw new NullReferenceException("Cannot attach a null reference to an item slot UI.");
        }
        
        if (_slot != null)
        {
            Detach();
        }

        _slot = slot;
        _slot.HandlerUpdated += Update;
        _slot.HandlerEmptied += Clear;
        _slot.HandlerDetached += Clear;

        if (_slot.Handler != null)
        {
            Update();
        }
    }

    public void Detach()
    {
        if (_slot == null)
        {
            throw new NullReferenceException("Cannot detach an item slot UI that isn't attached to anything.");
        }

        Clear();
        _slot.HandlerUpdated -= Update;
        _slot.HandlerEmptied -= Clear;
        _slot.HandlerDetached -= Clear;
        _slot = null;
    }

    public void InvokePopup()
    {
        if (_popup != null && _slot.Handler != null && !_popupInvoked && !GetViewport().GuiIsDragging() && _canDrag)
        {
            _popupInvoked = true;
            _popup.SendList(_slot.Handler.GetFullDisplayText());
        }
    }

    public void LosePopup()
    {
        if (_popup != null && _popupInvoked)
        {
            _popup.LoseList();
            _popupInvoked = false;
        }
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (_slot.Handler == null)
        {
            return default;
        }

        if (!_canDrag)
        {
            return default;
        }

        ItemSlotUI preview = _previewScene.Instantiate<ItemSlotUI>();
        preview.Attach(_slot);
        preview.MouseBehaviorRecursive = MouseBehaviorRecursiveEnum.Disabled;

        SetDragPreview(preview);
        EmitSignal(SignalName.Dragged);
        return _slot;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return _slot != null && data.VariantType == Variant.Type.Object &&
            data.AsGodotObject() is ItemSlot;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        data.As<ItemSlot>().ExchangeWith(_slot);
        EmitSignal(SignalName.Dropped);
    }

    public override void _ExitTree()
    {
        if (_slot != null)
        {
            Detach();
        }
    }
}
