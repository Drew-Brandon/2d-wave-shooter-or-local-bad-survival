namespace Puppets.Bodies;
using Godot;
using HealthSources;
using Controllers;
using System;
using Puppets.StatusEffects;
using GC = Godot.Collections;
using SCG = System.Collections.Generic;

/// <summary>
/// Represents a "CharacterBody2D" that can be controlled by a "PuppetControllerTop2D."
/// Puppets generally define the actions that a character can perform,
/// with the controllers directing when they should do said actions.
/// </summary>
[GlobalClass]
public partial class PuppetBodyTop2D : CharacterBody2D, IHealthSource, ISkeletalHealthSource
{
	private bool _isFocusing = false;

	/// <summary>
	/// Whether or not the player is currently focusing on some target.
	/// </summary>
	public bool IsFocusing { get => _isFocusing; }

	private Vector2 _focusPosition = Vector2.Zero;
	
	public Vector2 FocusPosition { get => _focusPosition; }

	private PuppetStatusEffectManager _statusEffects = new();
	public PuppetStatusEffectManager StatusEffects { get => _statusEffects; }

	private Node2D _focusTarget = null;

	/// <summary>
	/// The node that the player is currently focusing on.
	/// This means that the player is actively turning themselves towards this node.
	/// </summary>
	public Node2D FocusTarget { get => _focusTarget; }

	/// <summary>
	/// Whether or not this puppet currently has a tool.
	/// </summary>
	public bool HasTool { get => _currentTool != null; }

	[Export]
	private float _mass = 70f;

	[Export]
	private SkeletalHealthSource _healthSource = null;

	/// <summary>
	/// The puppet's health source.
	/// This is used to store the info about and functionality of the puppet's health.
	/// </summary>
	public SkeletalHealthSource HealthSource { get => _healthSource; }

	private UseableTool _currentTool = null;

	/// <summary>
	/// The current tool that this puppet has equipped.
	/// </summary>
	public UseableTool CurrentTool { get => _currentTool; }

	private PuppetControllerTop2D _controller = null;

	/// <summary>
	/// The controller that is currently using this puppet.
	/// </summary>
	public PuppetControllerTop2D Controller => _controller;

	[Export]
	private Node2D _toolsParent = null;
	
	[ExportGroup("Movement")]
	[Export]
	private bool _isFrozen = false;

	/// <summary>
	/// Whether or not the character is frozen and thus cannot move.
	/// </summary>
	public bool IsFrozen { get => _isFrozen; set => _isFrozen = value; }

	[Export]
	private float _rotationWeight = 16f;

	protected bool InternalIsTargetRotationLocked = false;
	public bool IsTargetRotationLocked { get => InternalIsTargetRotationLocked; }

	protected float InternalTargetRotation = 0f;

	/// <summary>
	/// The rotation for the puppet to rotate towards.
	/// </summary>
	public float TargetRotation
	{
		get => InternalTargetRotation;
		set
		{
			if (!InternalIsTargetRotationLocked)
			{
				InternalTargetRotation = value;
			}
		}
	}

	[Export]
	private Node2D _swivel = null;

	/// <summary>
	/// The node that the puppet rotates to show which direction its facing in.
	/// </summary>
	public Node2D Swivel { get => _swivel; }

	[Export]
	private float _baseSpeed = 100f;

	/// <summary>
	/// The base speed that the puppet can move at.
	/// Note that this is not necessarily the speed of the puppet,
	/// but is instead the speed the puppet should be moving at if no extra effects
	/// are being applied.
	/// </summary>
	public float BaseSpeed { get => _baseSpeed; }

	private GC.Dictionary<StringName, float> _speedMultipliers = new();

	[Export]
	private float _velWeight = 100f;

	[Export]
	private float _pushPower = 4f;

	private Vector2 _currentDirection;

	/// <summary>
	/// The current direction that the character moves in.
	/// </summary>
	public Vector2 CurrentDirection { get => _currentDirection; set => _currentDirection = value.Normalized(); }

	[Signal]
	public delegate void ToolChangedEventHandler(UseableTool newTool);

	[Signal]
	public delegate void VelocityUpdatedEventHandler(Vector2 velocity);

	[Signal]
	public delegate void MovedEventHandler(Vector2 motion);

	public void SetFrozenState(bool isFrozen)
	{
		_isFrozen = isFrozen;
	}

	/// <summary>
	/// Updates the given velocity value.
	/// </summary>
	/// <param name="delta">
	/// The delta time to update the velocity with.
	/// This allows for framerate independent updates.
	/// </param>
	/// <param name="velocity">
	/// The velocity value to update.
	/// </param>
	/// <returns>
	/// The new value for the velocity.
	/// </returns>
	protected Vector2 UpdateVelocity(float delta, Vector2 velocity)
	{
		// Get the target velocity, ease into it, and move the character.
		Vector2 targetVelocity;

		if (_isFrozen)
		{
			targetVelocity = Vector2.Zero;
		}
		else
		{
			targetVelocity = _currentDirection * _baseSpeed;

			foreach (SCG.KeyValuePair<StringName, float> multiplierData in _speedMultipliers)
			{
				targetVelocity *= multiplierData.Value;
			}
		}

		Vector2 newVelocity = velocity.Lerp(targetVelocity, _velWeight * delta);
		EmitSignal(SignalName.VelocityUpdated, newVelocity);
		return newVelocity;
	}

	/// <summary>
	/// Pushes any objects in the puppet's way when moving.
	/// </summary>
	protected void PushBodies()
	{
		// Push colliding RigidBody2Ds out of the way.
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D col = GetSlideCollision(i);
			RigidBody2D rb = col.GetCollider() as RigidBody2D;

			if (rb != null)
			{
				// Credit to https://www.youtube.com/watch?v=Uh9PSOORMmA
				Vector2 pushDir = -col.GetNormal();
				Vector2 pushPos = col.GetPosition();
				float velDiff = Mathf.Max(0f, Velocity.Dot(pushDir) - rb.LinearVelocity.Dot(pushDir));
				float massRatio = Mathf.Min(1f, _mass / rb.Mass);
				float adjPushPower = massRatio * _pushPower;
				rb.ApplyImpulse(pushDir * velDiff * adjPushPower, pushPos - rb.GlobalPosition);
			}
		}
	}

	/// <summary>
	/// Updates the rotation of the puppet's swivel so that its closer to
	/// the current target rotation.
	/// </summary>
	/// <param name="delta">
	/// The delta time to update with.
	/// </param>
	protected void UpdateRotation(float delta)
	{
		_swivel.Rotation = Mathf.LerpAngle(_swivel.Rotation, InternalTargetRotation, _rotationWeight * delta);
	}

	/// <summary>
	/// Called when an update to the puppet's movement has been requested.
	/// </summary>
	/// <param name="delta">
	/// The delta time to update with.
	/// </param>
	protected virtual void OnMovementUpdated(float delta)
	{
		UpdateRotation(delta);
		Velocity = UpdateVelocity(delta, Velocity);
		EmitSignal(SignalName.Moved, Velocity * delta);
		MoveAndSlide();
		PushBodies();
	}

	public void AddHealth(float amount, Node instigator = null)
	{
		_healthSource.AddHealth(amount, instigator);
	}

	public void AddHealthAt(float amount, StringName part, Node instigator = null)
	{
		_healthSource.AddHealthAt(amount, part, instigator);
	}

	public void Kill(Node instigator)
	{
		_healthSource.Kill(instigator);
	}

	/// <summary>
	/// Uses the current tool the puppet has.
	/// </summary>
	/// <param name="pos">
	/// The position that the to use the tool on.
	/// </param>
	/// <returns>
	/// Whether or not the tool was successfuly used.
	/// </returns>
	public bool UseTool(Vector2 pos)
	{
		if (_currentTool != null)
		{
			return _currentTool.Use(pos);
		}

		return false;
	}

	private void InternalUnequipTool()
	{
		if (_currentTool == null)
		{
			throw new InvalidOperationException("Cannot unequip tool as there isn't one already equipped.");
		}

		_currentTool.Unequip();
		_currentTool.QueueFree();
		_currentTool = null;
	}

	/// <summary>
	/// Unequips the current tool the puppet has equipped.
	/// </summary>
	public void UnequipTool()
	{
		InternalUnequipTool();
		EmitSignal(SignalName.ToolChanged, new Variant());
	}

	private void InternalEquipTool(UseableTool tool)
	{
		UseableTool oldTool = _currentTool;
		_currentTool = tool;
		_currentTool.Equip(this);
		_currentTool.Position = Vector2.Zero;
		EmitSignal(SignalName.ToolChanged, _currentTool);
	}

	/// <summary>
	/// Equips the puppet with the given tool.
	/// </summary>
	/// <param name="tool">
	/// The tool for the puppet to equip.
	/// </param>
	public void EquipTool(UseableTool tool)
	{
		if (tool == _currentTool)
		{
			return;
		}

		if (_currentTool != null)
		{
			InternalUnequipTool();
		}
		
		InternalEquipTool(tool);
		_toolsParent.AddChild(_currentTool);
	}

	/// <summary>
	/// Sets the controller for this puppet to be used by.
	/// </summary>
	/// <param name="controller">
	/// The controller for the puppet to be used by.
	/// A null value is allowed and, if it has one, the puppet will detach
    /// from its current controller.
	/// </param>
	public void SetController(PuppetControllerTop2D controller)
	{
		if (_controller == controller)
        {
            return;
        }

		if (controller == null)
		{
			if (_controller.Body != null)
            {
                _controller.SetBody(null);
            }

			_controller = null;
			return;
		}

		_controller = controller;

		if (_controller.Body != this)
		{
			try
			{
				_controller.SetBody(this);
			}
			catch (Exception)
			{
				_controller = null;
                throw;
            }
		}
	}

	public void StopFocusing()
	{
		_isFocusing = false;
		_focusPosition = Vector2.Zero;
		_focusTarget = null;
		InternalIsTargetRotationLocked = false;
	}

	public void FocusOnPosition(Vector2 position)
	{
		_isFocusing = true;
		_focusPosition = position;
		InternalIsTargetRotationLocked = true;
	}

	/// <summary>
	/// Focuses the player character on a given target node.
	/// </summary>
	/// <param name="target">
	/// The target to focus on.
	/// </param>
	public void FocusOnTarget(Node2D target)
	{
		if (target == null)
		{
			throw new NullReferenceException("Cannot focus on a null target.");
		}

		_isFocusing = true;
		_focusTarget = target;
		InternalIsTargetRotationLocked = true;
	}

	public T GetController<T>() where T : PuppetControllerTop2D
	{
		T casted = _controller as T;

		if (casted == null)
		{
			throw new NullReferenceException($"Puppet's controller was not of type \"{typeof(T)}.\"");
		}

		return casted;
	}

	public void ApplySpeedMultiplier(StringName name, float multiplier)
	{
		if (_speedMultipliers.ContainsKey(name))
		{
			_speedMultipliers[name] = multiplier;
		}
		else
		{
			_speedMultipliers.Add(name, multiplier);
		}
	}

	public void ApplySpeedMultiplierData(PuppetSpeedMultiplierData multiplier)
	{
		ApplySpeedMultiplier(multiplier.Name, multiplier.Multiplier);
	}

	public bool HasSpeedMultiplier(StringName name)
	{
		return _speedMultipliers.ContainsKey(name);
	}

	public bool HasSpeedMultiplierData(PuppetSpeedMultiplierData multiplier)
	{
		return _speedMultipliers.ContainsKey(multiplier.Name);
	}

	public void ClearSpeedMultiplier(StringName name)
	{
		if (!_speedMultipliers.ContainsKey(name))
		{
			throw new SCG.KeyNotFoundException($"Speed multipliers does not contain {name}.");
		}

		_speedMultipliers.Remove(name);
	}

	public void ClearSpeedMultiplierData(PuppetSpeedMultiplierData multiplier)
	{
		if (multiplier == null)
		{
			throw new ArgumentNullException("multiplier");
		}

		_speedMultipliers.Remove(multiplier.Name);
	}

    public override void _Ready()
    {
		if (_toolsParent.GetChildCount() > 0)
		{
			InternalEquipTool(_toolsParent.GetChild<UseableTool>(0));
		}

		if (_controller != null && _controller.Body != this)
		{
			_controller.SetBody(this);
		}
    }

    public override void _PhysicsProcess(double delta)
    {
        OnMovementUpdated((float)delta);

		if (IsFocusing)
		{
			if (_focusTarget != null)
			{
				_focusPosition = _focusTarget.GlobalPosition;
			}

			InternalTargetRotation = GetAngleTo(_focusPosition);
		}
    }
}
