namespace Inventory;
using CastInfo2D;
using Godot;
using Puppets.Bodies;
using UI;

public partial class StaticInventoryContainer2D : Node2D, IInteractable
{
	[Export]
	private float _interactRange = 128f;

	[Export]
	private RayCastInfo2D _occlusionCast = null;

	[Export]
	private StaticInventory _inventory = null;

	[Export]
	private PackedScene _inventoryUIScene = null;

	private StaticInventoryUI _inventoryUI = null;

	private PlayerBodyTop2D _player = null;

	private bool CanInteractWith(PlayerBodyTop2D player)
	{
		if (GlobalPosition.DistanceTo(player.GlobalPosition) > _interactRange)
		{
			return false;
		}

		PhysicsRayQueryParameters2D query = _occlusionCast.CreateQuery(player.GlobalPosition, GlobalPosition);
		RayCastResults2D results = RayCastInfo2D.IntersectRay(GetWorld2D().DirectSpaceState, query);

		return results.Collider == this;
	}

	public bool StartInteraction(PlayerBodyTop2D player)
	{
		if (!CanInteractWith(player))
		{
			return false;
		}
		
		player.ToggleInventory(true);

		_inventoryUI = _inventoryUIScene.Instantiate<StaticInventoryUI>();
		_inventoryUI.Attach(_inventory);

		player.AddToViewportUI(_inventoryUI);
		_player = player;
		return true;
	}

	public void StopInteraction(PlayerBodyTop2D player)
	{
		_inventoryUI.QueueFree();

		if (_player.IsInventoryOpen)
		{
			_player.ToggleInventory(false);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player != null && !CanInteractWith(_player))
		{
			_player.StopInteraction();
		}
	}
}
