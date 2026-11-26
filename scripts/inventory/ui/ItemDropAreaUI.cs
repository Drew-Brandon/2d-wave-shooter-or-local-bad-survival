namespace Inventory.UI;
using Godot;

public partial class ItemDropAreaUI : Control
{
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		return data.VariantType == Variant.Type.Object && data.AsGodotObject() is ItemSlot;
	}

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        data.As<ItemSlot>().Handler.Drop();
    }
}
