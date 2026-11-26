namespace Inventory.Physics;
using Godot;
using System.Collections.Generic;
using ItemInstances;

/// <summary>
/// Represents a display that allows players to interface with the inventory system.
/// </summary>
public partial class InventoryPhysicsView : Node2D
{
	/*private bool _isOpen = false;
	public bool IsOpen { get => _isOpen; set => Toggle(value); }

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

	private int _displayCount = 0;

	[Export]
	private ListPopupRelay _instancePopup = null;

	/// <summary>
	/// The node to place containers under.
	/// </summary>
	public Node2D ContainersParent { get => _containersParent; }

	[Export]
	private StringName _editAugmentorAction = "edit_augmentor";

	private PhysicsItemInstanceAugmenter _focusedAugmentor = null;

	private HashSet<PhysicsContainer> _permanentContainers = new();

	private void OnAugmenterFocused(PhysicsItemInstanceAugmenter augmenter)
	{
		_focusedAugmentor = augmenter;
	}

	private void OnAugmentorUnfocused(PhysicsItemInstanceAugmenter augmenter)
	{
		if (_focusedAugmentor == augmenter)
		{
			_focusedAugmentor = null;
		}
	}

	private void OnInstanceLost(PhysicsItemInstanceAugmenter instance)
	{
		instance.FullDisplayShown -= _instancePopup.SendList;
		instance.FullDisplayHidden -= _instancePopup.LoseList;
	}

	private PhysicsItemInstanceAugmenter AddInstance(int index, BaseItemInstance instance)
	{
		PhysicsItemInstanceAugmenter instanceNode = instance.BaseItem.PhysicsScene.Instantiate<PhysicsItemInstanceAugmenter>();
		_itemsParent.AddChild(instanceNode);
		
		instanceNode.Position = _itemsParent.GlobalPosition + _spawnOffset * index;
		//instanceNode.Augmenter = instance.GetFilledAugmenter();

		instanceNode.FullDisplayShown += _instancePopup.SendList;
		instanceNode.FullDisplayHidden += _instancePopup.LoseList;

		instanceNode.MouseEntered += () => OnAugmenterFocused(instanceNode);
		instanceNode.MouseExited += () => OnAugmentorUnfocused(instanceNode);

		return instanceNode;
	}

	public PhysicsItemInstanceAugmenter AddInstance(BaseItemInstance instance)
	{
		return AddInstance(0, instance);
	}

	public PhysicsItemInstanceAugmenter[] AddInstances(BaseItemInstance[] instances)
	{
		PhysicsItemInstanceAugmenter[] instanceNodes = new PhysicsItemInstanceAugmenter[instances.Length];

		for (int i = 0; i < instances.Length; i++)
		{
			instanceNodes[i] = AddInstance(i, instances[i]);
		}

		return instanceNodes;
	}

	public void AddContainer(PhysicsContainer container)
	{
		_containersParent.AddChild(container);
	}

	/// <summary>
	/// Toggles the display on or off.
	/// </summary>
	/// <param name="toggle">
	/// Whether to turn the display on or off.
	/// </param>
	public void Toggle(bool toggle)
	{
		if (toggle == _isOpen)
		{
			return;
		}

		_isOpen = toggle;
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

				if (!_permanentContainers.Contains(container))
				{
					container.QueueFree();
				}
			}

			ProcessMode = ProcessModeEnum.Disabled;
		}
	}

	public override void _Ready()
    {
        for (int i = 0; i < _containersParent.GetChildCount(); i++)
		{
			_permanentContainers.Add(_containersParent.GetChild<PhysicsContainer>(i));
		}
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed(_editAugmentorAction))
		{
			
		}
    }*/
}
