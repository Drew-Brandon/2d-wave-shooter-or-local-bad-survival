namespace Logic;
using Godot;

[GlobalClass]
public partial class Area2DButton : Area2D
{
	private bool _isMouseOver = false;
	public bool IsMouseOver { get => _isMouseOver; }

	private bool _inputDown = false;
	public bool InputDown { get => _inputDown; }

	private bool _isPressed = false;
	public bool IsPressed { get => _isPressed; }

	[Export(PropertyHint.InputName)]
	private StringName _triggerInput = "";

	[Signal]
	public delegate void PressedEventHandler();

	[Signal]
	public delegate void HeldEventHandler();

	[Signal]
	public delegate void ReleasedEventHandler();

	private void OnMouseEntered()
	{
		_isMouseOver = true;
	}

	private void OnMouseExited()
	{
		_isMouseOver = false;

		if (_isPressed)
		{
			OnReleased();
		}
	}

	private void OnInputDown()
	{
		_inputDown = true;

		if (_isMouseOver)
		{
			OnPressed();
		}
	}

	private void OnInputUp()
	{
		_inputDown = false;

		if (_isPressed)
		{
			OnReleased();
		}
	}

	private void OnPressed()
	{
		_isPressed = true;
		EmitSignal(SignalName.Pressed);
		EmitSignal(SignalName.Held);
	}

	private void OnReleased()
	{
		_isPressed = false;
		EmitSignal(SignalName.Released);
	}

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(_triggerInput))
		{
			OnInputDown();
		}
		else if (@event.IsActionReleased(_triggerInput))
		{
			OnInputUp();
		}
    }

    public override void _Process(double delta)
    {
        if (_isPressed)
		{
			EmitSignal(SignalName.Held);
		}
    }
}
