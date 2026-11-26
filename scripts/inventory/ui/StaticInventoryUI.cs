namespace Inventory.UI;
using System;
using Godot;

public partial class StaticInventoryUI : Control
{
    [Export]
    private bool _canDragSlots = true; 

    [Export]
    private int _startIndex = 0;

    [Export]
    private NodePathResource _popupPath = null;

    private ListPopupRelay _popup = null;

    [Export]
    private StaticInventory _inventory = null;
    public StaticInventory Inventory { get => _inventory; }

    [Export]
    private Control _slotsParent = null;

    private void AttachSlot(int index, ItemSlotUI slot)
    {
        slot.Attach(_inventory.GetSlot(index + _startIndex));
    }

    private void DetachSlot(int index, ItemSlotUI slot)
    {
        slot.Detach();
    }

    private void SetSlotPopup(int index, ItemSlotUI slot)
    {
        slot.Popup = _popup;
    }

    private void UnlockSlotDragging(int index, ItemSlotUI slot)
    {
        slot.UnlockDragging();
    }

    private void LockSlotDragging(int index, ItemSlotUI slot)
    {
        slot.LockDragging();
    }

    private void AlterSlots(Action<int, ItemSlotUI> alterAction)
    {
        for (int i = 0; i < _slotsParent.GetChildCount(); i++)
        {
            ItemSlotUI slotUI = _slotsParent.GetChild(i) as ItemSlotUI;

            if (slotUI == null)
            {
                throw new NullReferenceException("Child of slots' parent was not an instance of ItemSlotUI.");
            }
            else
            {
                alterAction.Invoke(i, slotUI);
            }
        }
    }

    private void AttachSlots()
    {
        if (_slotsParent.GetChildCount() > _inventory.Size)
        {
            _inventory = null;
            throw new InvalidOperationException("Number of slot ui's is greater than the number of inventory slots.");
        }

        AlterSlots(AttachSlot);
    }

    public void InitPopup()
    {
        if (_popupPath == null)
        {
            throw new NullReferenceException("A popup path has not been assigned to this inventory UI.");
        }

        _popup = GetCanvasLayerNode().GetNode<ListPopupRelay>(_popupPath.Path);
        AlterSlots(SetSlotPopup);
    }

    public void Attach(StaticInventory inventory)
    {
        if (inventory == null)
        {
            throw new NullReferenceException("Cannot attach a null reference to a static inventory UI.");
        }
        
        if (_inventory != null)
        {
            Detach();
        }

        _inventory = inventory;
        AttachSlots();
    }

    public void Detach()
    {
        if (_inventory == null)
        {
            throw new NullReferenceException("Cannot detach a static inventory UI that isn't attached to anything.");
        }

        AlterSlots(DetachSlot);
    }

    public void UnlockDragging()
    {
        if (_canDragSlots)
        {
            throw new InvalidOperationException("Slots should already be draggable.");
        }

        _canDragSlots = true;
        AlterSlots(UnlockSlotDragging);
    }

    public void LockDragging()
    {
        if (!_canDragSlots)
        {
            throw new InvalidOperationException("Slots should already not be draggable.");
        }

        _canDragSlots = false;
        AlterSlots(LockSlotDragging);
    }

    public void BrushSlot(int index)
    {
        _slotsParent.GetChild<ItemSlotUI>(index).Brush();
    }

    public void HighlightSlot(int index)
    {
        _slotsParent.GetChild<ItemSlotUI>(index).Highlight();
    }

    public override void _Ready()
    {
        if (_inventory != null)
        {
            AttachSlots();
        }

        if (_popupPath != null)
        {
            _popup = Owner.GetNode<ListPopupRelay>(_popupPath.Path);
            AlterSlots(SetSlotPopup);
        }

        if (!_canDragSlots)
        {
            AlterSlots(LockSlotDragging);
        }
    }
}
