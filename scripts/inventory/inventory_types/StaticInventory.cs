namespace Inventory;
using Godot;

public partial class StaticInventory : IndexedInventory
{
    [Export]
    private uint _inventorySize = 10;

    private ItemSlot[] _slots = null;

    public override int Size { get => _slots.Length; }

    public override ItemSlot GetSlot(int index)
    {
        return _slots[index];
    }

    protected override void InitSlot(int index)
    {
        _slots[index] = new ItemSlot();
    }

    public override void _Ready()
    {
        _slots = new ItemSlot[_inventorySize];
        base._Ready();
    }
}
