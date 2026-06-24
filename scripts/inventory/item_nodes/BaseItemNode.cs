namespace Inventory;
using Godot;

/// <summary>
/// Represents a node that is used to execute the behavior of an item.
/// </summary>
public partial class BaseItemNode : Node
{
	private bool _canUse = true;

	/// <summary>
	/// Whether or not the item can currently be used.
	/// </summary>
	public bool CanUse { get => _canUse; set => _canUse = value; }

	private PlayerBody2D _plyr;
	protected PlayerBody2D Plyr { get => _plyr; }

	private PlayerInventory _inventory;
	protected PlayerInventory Inventory { get => _inventory; }

	//private BaseItemSlot _slot;
	//protected BaseItemSlot GenericSlot { get => _slot; }

	/// <summary>
	/// Uses the node for its specific purpose.
	/// </summary>
	/// <param name="plyr">
	/// The player using the node.
	/// </param>
	/// <param name="slot">
	/// The slot that the node is being used with/from.
	/// </param>
	/// <param name="usePos">
	/// The position the node is being used on.
	/// </param>
	/// <returns>
	/// Whether or not the node was successfuly used.
	/// </returns>
	public bool Use(Vector2 usePos)
	{
		if (_canUse)
		{
			return OnUse(usePos);
		}

		return false;
	}

	/// <summary>
	/// Triggered when the item is used.
	/// </summary>
	/// <param name="plyr">
	/// The player using the item.
	/// </param>
	/// <param name="inventory">
	/// The inventory that the item is from.
	/// </param>
	/// <param name="slot">
	/// The slot the item is from.
	/// </param>
	/// <param name="usePos">
	/// The position that the item was used on.
	/// </param>
	/// <returns>
	/// Whether or not the use was successful.
	/// </returns>
	protected virtual bool OnUse(Vector2 usePos)
	{
		return false;
	}

	/// <summary>
	/// Equips the node from the specified item slot.
	/// </summary>
	/// <param name="slot">
	/// The slot that the node is being equipped from.
	/// </param>
	public void Equip(SceneReferences sceneRefs, PlayerBody2D plyr, PlayerInventory inventory, BaseItemSlot slot)
	{
		_plyr = plyr;
		_inventory = inventory;
		_slot = slot;
        OnEquip(sceneRefs);
	}

	protected virtual void OnEquip(SceneReferences sceneRefs)
	{

	}

	/// <summary>
	/// Unequips the node from the specified item slot.
	/// </summary>
	public void Unequip()
	{
		QueueFree();
		OnUnequip();
	}

	protected virtual void OnUnequip()
	{

	}
}
