namespace Inventory;
using System;
using Godot;
using Inventory.ItemHandlers;
using ItemInstances;
using Puppets.Bodies;

/// <summary>
/// Represents a dropped item that can be picked up by a player.
/// </summary>
public partial class DroppedItemInstance : Node2D, IInteractable
{
	[Export]
	private ItemInstanceBlueprint _instanceBlueprint = null;

	private BaseItemInstance _instance = null;
	public BaseItemInstance Instance { get => _instance; set => SetInstance(value); }

	[Signal]
	public delegate void IconChangedEventHandler(Texture2D icon);

	public void SetInstance(BaseItemInstance instance)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}

		_instance = instance;
		EmitSignal(SignalName.IconChanged, _instance.BaseItemType.Icon);
	}

	public bool StartInteraction(PlayerBodyTop2D plyr)
	{
		BaseItemHandler handler = (BaseItemHandler)Activator.CreateInstance(_instance.HandlerType, _instance, false);
		plyr.Inventory.TryToAdd(handler);

		if (handler.IsEmpty)
		{
			QueueFree();
		}

		return false;
	}

    public override void _Ready()
    {
		if (_instanceBlueprint != null)
		{
			_instance = _instanceBlueprint.CreateObject();
		}
    }
}
