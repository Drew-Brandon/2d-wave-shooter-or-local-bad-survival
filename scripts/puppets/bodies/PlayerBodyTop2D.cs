namespace Puppets.Bodies;
using CastInfo2D;
using Godot;
using Inventory;
using Inventory.ItemHandlers;
using Puppets.Controllers;
using System;

/// <summary>
/// The body controlled to be controlled by the player.
/// </summary>
[GlobalClass]
public partial class PlayerBodyTop2D : PuppetBodyTop2D
{
	private bool _popupInvoked = false;

	/// <summary>
	/// Whether or not the player is currently interacting with something.
	/// </summary>
	public bool IsInteracting { get => _currentInteractable != null; }

	private IInteractable _currentInteractable = null;

	/// <summary>
	/// The player controller of this character.
	/// This will return null if there isn't one.
	/// </summary>
	public PlayerControllerTop2D PlayerController { get => GetController<PlayerControllerTop2D>(); }

	[Export]
	private CanvasLayer _viewportUILayer = null;

	[Export]
	private CanvasLayer _worldUILayer = null;

	[Export]
	private ShapeCastInfo2D _pointCast = null;

	[Export]
	private ListPopupRelay _popupRelay = null;

	[ExportGroup("Inventory")]
	private bool _isInventoryOpen = false;

	public bool IsInventoryOpen { get => _isInventoryOpen; set => ToggleInventory(value); }

	private int _equippedIndex = -1;
	public int EquippedIndex { get => _equippedIndex; }

	public BaseItemHandler EquippedItem { get => _inventory.GetHandler(_equippedIndex); }

	[Export]
	private IndexedInventory _inventory = null;

	public IndexedInventory Inventory { get => _inventory; }

	[Signal]
	public delegate void InventoryOpenedEventHandler();

	[Signal]
	public delegate void InventoryClosedEventHandler();

	[Signal]
	public delegate void SlotUnequippedEventHandler(int index);

	[Signal]
	public delegate void SlotEquippedEventHandler(int index);
	
	/// <summary>
	/// Stop the interaction that the player is currently doing.
	/// </summary>
	public void StopInteraction()
	{
		if (!IsInteracting)
		{
			throw new InvalidOperationException("Cannot stop a player's interaction if they're not interacting with anything to begin with.");
		}

		if (IsFrozen)
		{
			return;
		}
		
		_currentInteractable.StopInteraction(this);
		((Node)_currentInteractable).TreeExiting -= StopInteraction;
		_currentInteractable = null;
	}

	private ShapeCastResults2D[] CastAt(Vector2 position)
	{
		PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
		Transform2D targetTransform = new(0f, position);
		PhysicsShapeQueryParameters2D query = _pointCast.CreateQuery(targetTransform);
		return ShapeCastInfo2D.IntersectShape(spaceState, query);
	}
	
	/// <summary>
	/// Attempts to start an interaction with a node.
	/// </summary>
	public void StartInteraction(Vector2 position)
	{
		if (IsFrozen)
		{
			return;
		}
		
		if (IsInteracting)
		{
			StopInteraction();
		}

		ShapeCastResults2D[] results = CastAt(position);
		IInteractable interactable = null;

		for (int i = 0; i < results.Length; i++)
		{
			interactable = results[i].Collider as IInteractable;

			if (interactable != null)
			{
				break;
			}
		}
		
		if (interactable != null)
		{
			if (interactable.StartInteraction(this))
			{
				((Node)interactable).TreeExiting += StopInteraction;
				_currentInteractable = interactable;
			}
		}
	}

	/// <summary>
	/// Toggles the inventory's open status.
	/// </summary>
	/// <param name="toggle">
	/// Whether or not the inventory should be opened or closed.
	/// </param>
	public void ToggleInventory(bool toggle)
	{
		if (_isInventoryOpen != toggle)
		{
			_isInventoryOpen = toggle;

			if (_isInventoryOpen)
			{
				if (_popupInvoked)
				{
					_popupRelay.LoseList();
				}

				EmitSignal(SignalName.InventoryOpened);
			}
			else
			{
				EmitSignal(SignalName.InventoryClosed);
			}
		}
	}

	private void OnEquippedHandlerChanged()
	{
		InternalEquipItemAt(_equippedIndex);
	}

	private void InternalEquipItemAt(int index)
	{
		BaseItemHandler handler = _inventory.GetHandler(index);

		if (handler == null)
		{
			if (CurrentTool != null)
			{
				UnequipTool();
			}
		}
		else
		{
			PackedScene equipScene = handler.ItemType.EquipScene;

			if (equipScene != null)
			{
				Node node = equipScene.Instantiate();
				UseableTool tool = node as UseableTool;

				if (tool == null)
				{
					throw new InvalidCastException("Cannot equip an item that isn't a useable tool.");
				}

				EquipTool(tool);
				tool.SetHandler(handler);
			}
			else if (CurrentTool != null)
			{
				UnequipTool();
			}
		}

		_inventory.GetSlot(index).HandlerChanged += OnEquippedHandlerChanged;
	}

	public void EquipItemAt(int index)
	{
		if (index == _equippedIndex)
		{
			return;
		}

		if (_equippedIndex >= 0)
		{
			_inventory.GetSlot(_equippedIndex).HandlerChanged -= OnEquippedHandlerChanged;
			EmitSignal(SignalName.SlotUnequipped, _equippedIndex);
		}

		_equippedIndex = index;
		InternalEquipItemAt(index);
		EmitSignal(SignalName.SlotEquipped, index);
	}

	/// <summary>
	/// Adds a UI element that will follow the player's viewport.
	/// This is useful for things like inventory windows and such.
	/// </summary>
	/// <param name="node">
	/// The UI element to add.
	/// </param>
	public void AddToViewportUI(Node node)
	{
		_viewportUILayer.AddChild(node);
	}

	/// <summary>
	/// Adds a UI element that will be positioned in the world.
	/// This is useful for when you wan't something to be drawn on-top of a scene,
	/// but don't want it following the player's viewport.
	/// </summary>
	/// <param name="node">
	/// The UI element to add.
	/// </param>
	public void AddToWorldUI(Node node)
	{
		_worldUILayer.AddChild(node);
	}

	public void InvokePopupAt(Vector2 position)
	{
		ShapeCastResults2D[] results = CastAt(position);

		if (results.Length > 0)
		{
			for (int i = 0; i < results.Length; i++)
			{
				IPopup popup = results[i].Collider as IPopup;

				if (popup != null)
				{
					string[] text = popup.GetText(this);

					if (text == null)
					{
						continue;
					}
					
					_popupInvoked = true;
					_popupRelay.SendList(text);
					return;
				}
			}
		}
		
		if (_popupInvoked)
		{
			_popupInvoked = false;
			_popupRelay.LoseList();
		}
	}

    public override void _Ready()
    {
        base._Ready();

		if (_inventory == null)
		{
			throw new NullReferenceException("Player's inventory cannot be null.");
		}

		EquipItemAt(0);
    }

    public override void _ExitTree()
    {
        if (IsInteracting)
		{
			StopInteraction();
		}
    }
}
