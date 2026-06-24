namespace Inventory;
using Godot;

/// <summary>
/// Represents a display that allows players to interface with the inventory system.
/// </summary>
public partial class InventoryView : Node2D
{
	[Export]
	private Vector2 _spawnOffset = new Vector2(256f, 0f);

	[Export]
	private Node2D _itemsParent = null;

	/// <summary>
	/// The node to place items under.
	/// </summary>
	public Node2D ItemParent { get => _itemsParent; }

	[Export]
	private Node2D _containersParent = null;

	/// <summary>
	/// The node to place containers under.
	/// </summary>
	public Node2D ContainersParent { get => _containersParent;}

	/// <summary>
	/// Adds the given array of items to the display.
	/// </summary>
	/// <param name="items">
	/// The items to add into the display.
	/// </param>
	public void AddItems(PhysicsItem[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			_itemsParent.AddChild(items[i]);
			items[i].Position = _itemsParent.GlobalPosition + _spawnOffset * i;
		}
	}

	/// <summary>
	/// Toggles the display on or off.
	/// </summary>
	/// <param name="toggle">
	/// Whether to turn the display on or off.
	/// </param>
	public void Toggle(bool toggle)
	{
		Visible = toggle;

		if (toggle)
		{
			ProcessMode = ProcessModeEnum.Inherit;
		}
		else
		{
			for (int i = 0; i < _itemsParent.GetChildCount(); i++)
			{
				_itemsParent.GetChild(i).QueueFree();
			}

			for (int i = 0; i < _containersParent.GetChildCount(); i++)
			{
				PhysicsContainer container = _containersParent.GetChild<PhysicsContainer>(i);
				container.SaveContainer();
			}

			ProcessMode = ProcessModeEnum.Disabled;
		}
	}
}
