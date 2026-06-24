using Godot;

public partial class InteractiveButton : Node, IInteractable
{
	[Signal]
	public delegate void OnButtonPressedEventHandler();

	public bool Interact(PlayerBody2D plyr)
	{
		EmitSignal(SignalName.OnButtonPressed);
		return false;
	}
}
