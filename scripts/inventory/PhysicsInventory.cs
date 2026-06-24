namespace Inventory;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents a physics based inventory.
/// </summary>
public partial class PhysicsInventory : Node
{
	/// <summary>
	/// The info regarding a physics item stored in the inventory.
	/// </summary>
	private struct PhysicsItemInfo
	{
		/// <summary>
		/// The transform of the item.
		/// </summary>
		public Transform2D Transform = new();
		
		/// <summary>
		/// The scene that the item was created from.
		/// </summary>
		public PackedScene Scene = null;

		/// <summary>
		/// Creates a new PhysicsItemInfo based off the given item.
		/// </summary>
		/// <param name="item">
		/// The item to store info about.
		/// </param>
		public PhysicsItemInfo(PhysicsItem item)
		{
			Transform = item.Transform;
			Scene = GD.Load<PackedScene>(item.SceneFilePath);
		}
	}

	private bool _hasContainer = false;

	private Dictionary<BaseItemInstance, PhysicsItemInfo> _itemInfos = new();

	[Export]
	private PhysicsContainer _curContainer = null;

	[Export]
	private PackedScene _container = null;

	/// <summary>
	/// Called when a new item is added into the inventory.
	/// </summary>
	/// <param name="item">
	/// The item that was added in.
	/// </param>
	[Signal]
	public delegate void OnItemAddedEventHandler(PhysicsItem item);

	public override void _Ready()
	{
		_hasContainer = _curContainer != null;

		if (_hasContainer)
		{
			_curContainer.OnContainerSaved += SaveInventory;
		}
	}

	public override void _ExitTree()
	{
		_curContainer.OnContainerSaved -= SaveInventory;
	}

	/// <summary>
	/// Saves the given items into the inventory so that they can be loaded later.
	/// </summary>
	/// <param name="items">
	/// The items that to save.
	/// </param>
	public void SaveInventory(PhysicsItem[] items)
	{
		Dictionary<BaseItemInstance, PhysicsItemInfo> newItemInfos = new();

		for (int i = 0; i < items.Length; i++)
		{
			PhysicsItem item = items[i];
			PhysicsItemInfo itemInfo = new(item);

			newItemInfos.Add(item.Instance, itemInfo);

			if (!_itemInfos.ContainsKey(item.Instance))
			{
				EmitSignal(SignalName.OnItemAdded, item);
			}
		}

		if (!_hasContainer)
		{
			_curContainer.QueueFree();
			_curContainer = null;
		}

		_itemInfos = newItemInfos;
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
		if (!_hasContainer)
		{
			_curContainer = _container.Instantiate<PhysicsContainer>();
			containerParent.AddChild(_curContainer);
			_curContainer.OnContainerSaved += SaveInventory;
		}

		foreach (KeyValuePair<BaseItemInstance, PhysicsItemInfo> pair in _itemInfos)
		{
			PhysicsItemInfo itemInfo = pair.Value;
			PhysicsItem item = itemInfo.Scene.Instantiate<PhysicsItem>();

			itemParent.AddChild(item);
			item.Transform = itemInfo.Transform;
			item.Instance = pair.Key;
		}
	}
}
