namespace Inventory.Physics;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents a physics based inventory.
/// </summary>
public partial class PhysicsInventory : Node
{
	/*private PhysicsItemInfo[] _storedInfo = [];

	private HashSet<PhysicsItemInstanceAugmenter> _contentsSet = new();

	[Export]
	private PhysicsContainer _curContainer = new();

	[Export]
	private PackedScene _container = null;

	/// <summary>
	/// Called when a new item is added into the inventory.
	/// </summary>
	/// <param name="item">
	/// The item that was added in.
	/// </param>
	[Signal]
	public delegate void ItemAddedEventHandler(PhysicsItemInstanceAugmenter item);

	public override void _Ready()
	{
		if (_curContainer != null)
		{
			_curContainer.ContainerSaved += SaveInventory;
		}
	}

	public override void _ExitTree()
	{
		_curContainer.ContainerSaved -= SaveInventory;
	}

	/// <summary>
	/// Saves the given items into the inventory so that they can be loaded later.
	/// </summary>
	/// <param name="items">
	/// The items that to save.
	/// </param>
	public void SaveInventory(PhysicsItemInstanceAugmenter[] items)
	{
		List<PhysicsItemInfo> newInfo = new();

		for (int i = 0; i < items.Length; i++)
		{
			PhysicsItemInstanceAugmenter physAugmenter = items[i];
			PhysicsItemInfo itemInfo = new(physAugmenter);
			newInfo.Add(itemInfo);

			if (!_contentsSet.Contains(physAugmenter))
			{
				EmitSignal(SignalName.ItemAdded, physAugmenter);
			}
		}

		_contentsSet.Clear();
		_storedInfo = newInfo.ToArray();
	}

	/// <summary>
	/// Loads the inventory's contents into a player's view.
	/// </summary>
	/// <param name="containerParent">
	/// The node to place the inventory's container under.
	/// </param>
	/// <param name="itemParent">
	/// The node to place the inventory's items under.
	/// </param>
	public void LoadInventory(Node2D containerParent, Node2D itemParent)
	{
		if (_curContainer == null)
		{
			_curContainer = _container.Instantiate<PhysicsContainer>();
			containerParent.AddChild(_curContainer);
			_curContainer.ContainerSaved += SaveInventory;
		}

		for (int i = 0; i < _storedInfo.Length; i++)
		{
			PhysicsItemInfo itemInfo = _storedInfo[i];
			PhysicsItemInstanceAugmenter augmenter = itemInfo.Scene.Instantiate<PhysicsItemInstanceAugmenter>();

			itemParent.AddChild(augmenter);
			augmenter.Transform = itemInfo.Transform;
			augmenter.Augmenter = itemInfo.Augmenter;
			_contentsSet.Add(augmenter);
		}
	}*/
}
