namespace Settings;
using Godot;
using System;
using Godot.Collections;

/// <summary>
/// An OptionButton that allows a user to select the window mode.
/// </summary>
[GlobalClass]
public partial class WindowModeOptionButton : OptionButton
{
	[Export]
	private Array<DisplayServer.WindowMode> _itemModes = null;

	private void OnItemSelected(int index)
	{
		DisplayServer.WindowMode mode = _itemModes[index];
		DisplayServer.WindowSetMode(mode);
	}

    public override void _Ready()
    {
		if (_itemModes == null)
		{
			throw new NodeException(
				GetPath(),
				new NullReferenceException("Item modes reference cannot be null for window mode option button.")
			);
		}

		if (_itemModes.Count != ItemCount)
		{
			throw new NodeException(
				GetPath(),
				new InvalidOperationException("Item modes array contain the same number of elements as the amount of options.")
			);
		}

		Select(_itemModes.IndexOf(DisplayServer.WindowGetMode()));
        ItemSelected += (long index) => OnItemSelected((int)index);
    }
}
