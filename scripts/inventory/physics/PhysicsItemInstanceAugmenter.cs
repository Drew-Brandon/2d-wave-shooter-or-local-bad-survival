namespace Inventory.Physics;
using System;
using Godot;
using Inventory.ItemHandlers;

public partial class PhysicsItemInstanceAugmenter : RigidBody2D
{
	/*private BaseItemHandler _augmenter = null;
	public BaseItemHandler Augmenter
	{
		get => _augmenter;

		set
		{
			if (_augmenter != null)
			{
				throw new InvalidOperationException("Cannot attach an augmentor to a physics augmentor that already has one.");
			}

			if (value == null)
			{
				throw new NullReferenceException("A null augmentor cannot be attached to a physics augmenter cannot be null.");
			}

			_augmenter = value;
			_augmenter.Emptied += QueueFree;
			_augmenter.Updated += UpdateShortDisplay;
			UpdateShortDisplay();
		}
	}
	
	[Signal]
	public delegate void FullDisplayShownEventHandler(string[] displayText);

	[Signal]
	public delegate void FullDisplayHiddenEventHandler();

	[Signal]
	public delegate void ShortDisplayUpdatedEventHandler(string displayText);

	private void UpdateShortDisplay()
	{
		EmitSignal(SignalName.ShortDisplayUpdated, _augmenter.GetShortDisplayText());
	}

    public override void _ExitTree()
    {
        if (_augmenter != null)
		{
			_augmenter.Emptied -= QueueFree;
		}
    }

    public override void _MouseEnter()
    {
        EmitSignal(SignalName.FullDisplayShown, _augmenter.GetFullDisplayText().ToArray());
    }

    public override void _MouseExit()
    {
        EmitSignal(SignalName.FullDisplayHidden);
    }*/
}
