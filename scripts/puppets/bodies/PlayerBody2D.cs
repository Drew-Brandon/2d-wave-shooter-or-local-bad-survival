using Godot;
using Inventory;
using CustomCasting;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using Godot.Collections;

/// <summary>
/// The body controlled by the player.
/// </summary>
public partial class PlayerBody2D : PuppetBody2D
{
	private bool _canHeal = true;

	private bool _canUse = true;

	private bool _canToggleInven = true;

	private bool _isRecovering = false;

	private int _curWeaponIndex = -1;

	private IInteractable _interactable = null;

	[Export]
	private Node2D _swivel = null;

	[Export]
	private Control _mainUI = null;

	[Export]
	private CanvasLayer _uiLayer = null;

	//[Export]
	//private SaveSource _saveSrc = null;

	[ExportGroup("Health")]
	[Export]
	private int _healTime = 1000;

	[Export]
	private HealthItem _defHealthItem = null;
	
	//private CoroutineSource _healSrc = new();

	[ExportGroup("Mouse Casting")]
	[Export]
	private RayCastInfo _mouseCastSight = null;

	[Export]
	private PointCastInfo _mouseCastPoint = null;

	[ExportGroup("Inventory")]
	private bool _inventoryOpen = false;
	public bool InventoryOpen { get => _inventoryOpen; }

	//[Export]
	//private PlayerInventory _inventory = null;

	//public PlayerInventory Inventory { get => _inventory; }

	//public int HotbarSize { get => _inventory.HotbarSize; }

	//[Export]
	//private InventoryUI _inventoryUI = null;

	//[Export]
	//private HotbarUI _hotbarUI = null;

	[Export]
	private CanvasLayer _inventoryLayer = null;

	[Export]
	private Node _useablesParent = null;

	[Export]
	private InventoryView _inventoryView = null;
	public InventoryView InventoryView { get => _inventoryView; }

	[Export]
	private PhysicsInventory _inventory;
	public PhysicsInventory Inventory { get => _inventory; }

	/// <summary>
	/// Called when the player starts healing themselves.
	/// </summary>
	[Signal]
	public delegate void OnStartHealingEventHandler();

	/// <summary>
	/// Called when the player stops healing themselves.
	/// </summary>
	[Signal]
	public delegate void OnHaltHealingEventHandler();

	/// <summary>
	/// Performs a raycast towards the specified position,
	/// and scans for any nodes around said position if there was no obstruction.
	/// </summary>
	/// <param name="pos">
	/// The position to do the LOS cast to/at.
	/// </param>
	/// <returns>
	/// The results of the cast.
	/// This will be null if the cast was obstructed.
	/// </returns>
	public PointCastResults LOSPointCast(Vector2 pos)
	{
		PhysicsPointQueryParameters2D pointQuery = _mouseCastPoint.CreateQuery(GetGlobalMousePosition(), [GetRid()]);
		PointCastResults[] pointResults = PointCastInfo.IntersectPoint(this, pointQuery);
		
		if (pointResults.Length > 0)
		{
			PhysicsRayQueryParameters2D sightQuery = _mouseCastSight.CreateQuery(GlobalPosition, pos, [GetRid()]);
			RayCastResults sightResults = RayCastInfo.IntersectRay(this, sightQuery);

			for (int i = 0; i < pointResults.Length; i++)
			{
				if (sightResults.Collider == pointResults[i].Collider)
				{
					return pointResults[i];
				}
			}
		}

		return new PointCastResults();
	}
	
	/// <summary>
	/// Stop interacting with the current node being used.
	/// </summary>
	private void StopInteract()
	{
		Node col = (Node)_interactable;
		col.TreeExiting -= StopInteract;
		_interactable.StopInteract(this);
		_interactable = null;
	}

	/// <summary>
	/// Attempts to interact with a node.
	/// </summary>
	private void Interact()
	{
		PointCastResults castResults = LOSPointCast(GetGlobalMousePosition());
		
		if (castResults.Collider == null)
		{
			return;
		}

		IInteractable interactable = castResults.Collider as IInteractable;

		if (interactable != null)
		{
			bool lockOn = interactable.Interact(this);

			if (lockOn)
			{
				Node node = (Node)castResults.Collider;
				node.TreeExiting += StopInteract;
				_interactable = interactable;
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
		if (_canToggleInven && _inventoryOpen != toggle)
		{
			_inventoryOpen = toggle;
			IsFrozen = toggle;
			_canUse = !toggle;
			_canHeal = !toggle;
			_inventoryView.Toggle(toggle);

			if (toggle)
			{
				_inventory.LoadInventory(_inventoryView.ContainersParent, _inventoryView.ItemParent);
			}
		}
	}

	public void ToggleHeal()
	{
		/*if (!_canHeal || CurHealth >= MaxHealth)
		{
			return;
		}

		if (_healSrc.IsRunning)
		{
			IsFrozen = false;
			_healSrc.IsCancelled = true;
			EmitSignal(SignalName.OnHaltHealing);
		}
		else
		{
			_healSrc.StartCoroutine(HealRoutine());
		}*/
	}

	/// <summary>
	/// The routine in which the player attempts to heal themselves.
	/// </summary>
	/// <returns>
	/// An iterable object that is used as a reference to the wait time in ms of the routine.
	/// </returns>
	private IEnumerable HealRoutine()
	{
		/*StackableInstance neededHealth = new(_defHealthItem, 1);

		if (_inventory.HasInstance(neededHealth))
		{
			IsFrozen = true;
			_canUse = false;
			_canToggleInven = false;
			EmitSignal(SignalName.OnStartHealing);

			yield return _healTime;
			_inventory.ChargeInstance(neededHealth);

			if (neededHealth.Amount != 1)
			{
				AddHealth(this, _defHealthItem.HealAmount);
			}

			IsFrozen = false;
			_canUse = true;
			_canToggleInven = true;
		}*/
		yield return null;
	}

	public void ToggleInteract()
	{
		if (_interactable == null)
		{
			if (_canUse)
			{
				Interact();
			}
		}
		else
		{
			StopInteract();
		}
	}

	public void Equip(int index)
	{
		if (_canUse)
		{
			//_inventory.Equip(index);
		}
	}

	public void UseItem()
	{
		if (_canUse)
		{
			//_inventory.UseSlot(GetGlobalMousePosition());
		}
	}

	public override void _Ready()
	{
		base._Ready();
		//_hotbarUI.BindTo(_inventory);
		//_inventoryUI.BindTo(_inventory, _inventory.HotbarSize, _inventory.Size);
	}

	public override void _Process(double delta)
	{
		if (_interactable != null && _interactable.InteractUpdate((float)delta, this))
		{
			StopInteract();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaF = (float)delta;

		if (!IsFrozen)
		{
			_swivel.LookAt(GetGlobalMousePosition());
		}
		
		FullMoveUpdate(deltaF);
	}

	protected override void OnKill()
	{
		//Callable.From(() => _saveSrc.Load()).CallDeferred();
	}

	/// <summary>
	/// Adds the specified UI to the player's UI.
	/// </summary>
	/// <param name="newUI">
	/// The UI to add.
	/// </param>
	/// <returns>
	/// The UI added to the player.
	/// </returns>
	public Control AddUI(PackedScene newUI)
	{
		Control clone = newUI.Instantiate<Control>();
		_mainUI.AddChild(clone);
		return clone;
	}

	/// <summary>
	/// Adds the specified UI to the player's UI. 
	/// </summary>
	/// <typeparam name="T">
	/// The type of the UI to add. Must be descendant of the Control class.
	/// </typeparam>
	/// <param name="newUI">
	/// The UI to add.
	/// </param>
	/// <returns>
	/// The UI added to the player.
	/// </returns>
	public T AddUI<T>(PackedScene newUI) where T : Control
	{
		return AddUI(newUI) as T;
	}

	/// <summary>
	/// Gives a useable item to the player.
	/// </summary>
	/// <param name="scene">
	/// The scene of the item.
	/// </param>
	/// <returns>
	/// The created item node.
	/// </returns>
	/*public BaseItemNode AddItem(PackedScene scene)
	{
		BaseItemNode node = scene.Instantiate<BaseItemNode>();
		_useablesParent.AddChild(node);
		return node;
	}*/
}
