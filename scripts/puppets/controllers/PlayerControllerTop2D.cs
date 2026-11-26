namespace Puppets.Controllers;
using Godot;
using Bodies;
using System;
using Inventory;

/// <summary>
/// The controller meant to be used for the player character.
/// This controller mainly relies of user input to function.
/// </summary>
[GlobalClass]
public partial class PlayerControllerTop2D : PuppetControllerTop2D
{
    public override Type BodyType { get => typeof(PlayerBodyTop2D); }

	/// <summary>
	/// The player body that this controller is using.
	/// </summary>
	public PlayerBodyTop2D PlayerBody { get => (PlayerBodyTop2D)Body; }

	[Export]
	private PuppetSpeedMultiplierData _sprintMultiplier = null;

	private IndexedInventory _inventory = null;

	[ExportGroup("Melee Swinging")]
	private bool _isSwinging = false;

	private float _swingAmount = 0;

	[Export]
	private float _maxSwingAmount = 64;

	[Export]
	private float _maxSwingSpeed = 64f;

	private ulong _startSwingTime = 0;

	private Vector2 _screenSwingStart = Vector2.Zero;

	private Vector2 _globalSwingStart = Vector2.Zero;
	
	[ExportGroup("Input Actions")]
	[Export(PropertyHint.InputName)]
	private StringName _interactAction = "";

	[Export(PropertyHint.InputName)]
	private StringName _toggleInventoryAction = "";

	[Export(PropertyHint.InputName)]
	private StringName _useToolAction = "";

	[Export(PropertyHint.InputName)]
	private StringName _reloadGunAction = "";

	[ExportSubgroup("Movement Actions")]
	[Export(PropertyHint.InputName)]
	private StringName _sprintAction = "";

	[Export(PropertyHint.InputName)]
	private StringName _moveLeftAction = "";

	[Export(PropertyHint.InputName)]
	private StringName _moveRightAction = "";

	[Export(PropertyHint.InputName)]
	private StringName _moveUpAction = "";

	[Export(PropertyHint.InputName)]
	private StringName _moveDownAction = "";

	[Export(PropertyHint.InputName)]
	private StringName _equipItem1Action = "";

	[Export(PropertyHint.InputName)]
	private StringName _equipItem2Action = "";

	[Export(PropertyHint.InputName)]
	private StringName _equipItem3Action = "";

	[Export(PropertyHint.InputName)]
	private StringName _equipItem4Action = "";

	[Export(PropertyHint.InputName)]
	private StringName _cancelReloadGunAction = "";

	private void MovementInput()
	{
		Vector2 swivelPos = PlayerBody.Swivel.GlobalPosition;

		if (PlayerBody.HasSpeedMultiplierData(_sprintMultiplier))
		{
			if (!Input.IsActionPressed(_sprintAction) || PlayerBody.IsFocusing)
			{
				PlayerBody.ClearSpeedMultiplierData(_sprintMultiplier);
			}
		}
		else
		{
			if (Input.IsActionPressed(_sprintAction) && !PlayerBody.IsFocusing)
			{
				PlayerBody.ApplySpeedMultiplierData(_sprintMultiplier);
			}
		}

		if (!_isSwinging)
		{
			Vector2 mousePosition = PlayerBody.GetGlobalMousePosition();
			PlayerBody.TargetRotation = swivelPos.AngleToPoint(mousePosition);
		}
		
		PlayerBody.CurrentDirection = Input.GetVector(_moveLeftAction, _moveRightAction, _moveUpAction, _moveDownAction);
	}

	/*private void StartSwing(Vector2 globalMousePosition)
	{
		_isSwinging = true;
		_screenSwingStart = GetViewport().GetMousePosition();
		_globalSwingStart = globalMousePosition;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_startSwingTime = Time.GetTicksMsec();
	}

	private void StopSwing()
	{
		_isSwinging = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetViewport().WarpMouse(_screenSwingStart);
	}

	private void FinishSwing()
	{
		StopSwing();
		int swingDuration = (int)(Time.GetTicksMsec() - _startSwingTime);
		float swingSpeed = _maxSwingAmount / swingDuration;
		float swingPower = Mathf.Clamp(swingSpeed / _maxSwingSpeed, 0f, 1f);

		MeleeWeaponTool meleeWeapon = (MeleeWeaponTool)PlayerBody.CurrentTool;
		meleeWeapon.PowerfullyUse(swingPower, _globalSwingStart);
	}

	private void MeleeInput()
	{
		if (_isSwinging)
		{
			if (Input.IsActionJustReleased(_useToolAction))
			{
				StopSwing();
			}
		}
		else
		{
			if (Input.IsActionJustPressed(_useToolAction))
			{
				StartSwing(PlayerBody.GetGlobalMousePosition());
			}
		}
	}*/

	private void StandardToolInput()
	{
		if (PlayerBody.IsFocusing)
		{
			return;
		}

		Vector2 mousePosition = PlayerBody.GetGlobalMousePosition();

		if (Input.IsActionJustPressed(_useToolAction))
		{
			PlayerBody.UseTool(mousePosition);
		}
		else if (PlayerBody.CurrentTool.IsAuto && Input.IsActionPressed(_useToolAction))
		{
			PlayerBody.UseTool(mousePosition);
		}
	}

	private void ToolUseInput()
	{
		StandardToolInput();
	}

	private void ToolEquipInput()
	{
		if (Input.IsActionJustPressed(_equipItem1Action))
		{
			PlayerBody.EquipItemAt(0);
		}
		else if (Input.IsActionJustPressed(_equipItem2Action))
		{
			PlayerBody.EquipItemAt(1);
		}
		else if (Input.IsActionJustPressed(_equipItem3Action))
		{
			PlayerBody.EquipItemAt(2);
		}
		else if (Input.IsActionJustPressed(_equipItem4Action))
		{
			PlayerBody.EquipItemAt(3);
		}
	}

	private void GunReloadInput()
	{
		GunTool gunTool = PlayerBody.CurrentTool as GunTool;

		if (gunTool == null)
		{
			return;
		}

		if (Input.IsActionJustPressed(_reloadGunAction))
		{
			if (gunTool != null)
			{
				if (!gunTool.IsReloading)
				{
					gunTool.Reload(PlayerBody.Inventory);
				}
			}
		}

		if (Input.IsActionJustPressed(_cancelReloadGunAction))
		{
			if (!gunTool.IsCancellingReload)
			{
				gunTool.CancelReload(PlayerBody.Inventory);
			}
		}
	}

	private void ToolInput()
	{
		ToolEquipInput();

		if (PlayerBody.HasTool)
		{
			ToolUseInput();
			GunReloadInput();
		}
	}

	private void InventoryInput()
	{
		if (Input.IsActionJustPressed(_toggleInventoryAction))
		{
			PlayerBody.ToggleInventory(!PlayerBody.IsInventoryOpen);
		}

		if (!PlayerBody.IsInventoryOpen)
		{
			ToolInput();
		}
	}

	private void InteractInput()
	{
		if (Input.IsActionJustPressed(_interactAction))
		{
			if (PlayerBody.IsInteracting)
			{
				PlayerBody.StopInteraction();
			}
			
			PlayerBody.StartInteraction(PlayerBody.GetGlobalMousePosition());
		}
	}

	private void PopupInput()
	{
		PlayerBody.InvokePopupAt(PlayerBody.GetGlobalMousePosition());
	}

	protected override void OnControllerProcess(double delta)
	{
		InventoryInput();

		if (!PlayerBody.IsInventoryOpen)
		{
			InteractInput();
		}
	}

    protected override void OnControllerPhysicsProcess(double delta)
    {
		if (!PlayerBody.IsInventoryOpen)
		{
			MovementInput();
			PopupInput();
		}
		else
		{
			PlayerBody.CurrentDirection = Vector2.Zero;
		}
    }

	/*private void OnToolChanged(UseableTool oldTool, UseableTool newTool)
	{
		if (_isSwinging)
		{
			StopSwing();
		}
	}*/

    /*protected override void OnBodyChanged(PuppetBodyTop2D oldBody, PuppetBodyTop2D newBody)
    {
		if (oldBody != null)
		{
			oldBody.ToolChanged -= OnToolChanged;
		}

        if (newBody != null)
		{
			newBody.ToolChanged += OnToolChanged;
		}
    }*/

    public override void _ExitTree()
    {
		/*if (Body != null)
		{
			Body.ToolChanged -= OnToolChanged;
		}*/
    }
	
	/*public override void _Input(InputEvent @event)
    {
		if (_isSwinging)
		{
			InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;

			if (mouseEvent != null)
			{
				_swingAmount += mouseEvent.ScreenRelative.Length();

				if (Mathf.Abs(_swingAmount) >= _maxSwingAmount)
				{
					FinishSwing();
				}
			}
		}
    }*/
}
