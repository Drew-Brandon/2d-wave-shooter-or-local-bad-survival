namespace Inventory;
using Godot;

/// <summary>
/// Represents a dropped item that can be picked up by a player.
/// </summary>
public partial class DroppedItem : Node2D, IInteractable
{
	private Callable _onItemAdded = new();

	[Export]
	private BaseItemInstanceResource _itemInstace = null;

	private PlayerBody2D _curPlyr = null;

	public override void _ExitTree()
	{
		if (_curPlyr != null)
		{
			StopInteract(_curPlyr);
		}
	}

	public bool Interact(PlayerBody2D plyr)
	{
		Visible = false;
		BaseItemInstance instance = _itemInstace.GetInstance();
		PhysicsItem newItem = instance.GenericItem.PhysicsScene.Instantiate<PhysicsItem>();
		newItem.Instance = instance;

		plyr.InventoryView.AddItems([newItem]);
		_onItemAdded = Callable.From((PhysicsItem item) =>
		{
			if (item == newItem)
			{
				QueueFree();
			}
		});
		plyr.Inventory.Connect(PhysicsInventory.SignalName.OnItemAdded, _onItemAdded);

		_curPlyr = plyr;
		plyr.ToggleInventory(true);
		return true;
	}

	public void StopInteract(PlayerBody2D plyr)
	{
		_curPlyr = null;
		plyr.ToggleInventory(false);

		if (!IsQueuedForDeletion())
		{
			Visible = true;
			plyr.Inventory.Disconnect(PhysicsInventory.SignalName.OnItemAdded, _onItemAdded);
		}
	}

	public bool InteractUpdate(float delta, PlayerBody2D plyr)
	{
		return !plyr.InventoryOpen;
	}
}
