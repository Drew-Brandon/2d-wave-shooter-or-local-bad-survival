namespace Inventory;
using Godot;

public partial class InventoryAdder : Node
{
    [Export]
    private IndexedInventory _defaultTarget = null;

    [Export]
    private ItemInstanceBlueprint[] _toAdd = null;

    public void AddToDefault()
    {
        AddToInventory(_defaultTarget);
    }

    public void AddToInventory(IndexedInventory inventory)
    {
        for (int i = 0; i < _toAdd.Length; i++)
        {
            inventory.TryToAdd(_toAdd[i].CreateObject());
        }
    }
}
