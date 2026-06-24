using Godot;

public partial class PlayerController : PuppetController
{
	[Export]
	private PlayerBody2D _plyrBody = null;

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("sprint"))
		{
			_plyrBody.Run();
		}
		else
		{
			_plyrBody.Walk();
		}

		if (Input.IsActionJustPressed("heal"))
		{
			_plyrBody.ToggleHeal();
		}

		if (Input.IsActionJustPressed("toggle_inventory"))
		{
			_plyrBody.ToggleInventory(!_plyrBody.InventoryOpen);
		}

		if (Input.IsActionJustPressed("interact"))
		{
			_plyrBody.ToggleInteract();
		}

		/*for (int i = 0; i < _plyrBody.HotbarSize; i++)
		{
			if (Input.IsActionJustPressed("equip_" + i.ToString()))
			{
				_plyrBody.Equip(i);
			}
		}*/

		if (Input.IsActionJustPressed("use"))
		{
			_plyrBody.UseItem();
		}

		_plyrBody.CurDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
	}
}
