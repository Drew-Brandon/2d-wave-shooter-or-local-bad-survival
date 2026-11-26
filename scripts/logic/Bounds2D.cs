namespace Logic;
using Godot;

[Tool]
public partial class Bounds2D : Node2D, ISerializationListener
{
	[Export]
	private bool _debugDraw = false;

	private Color _debugColor = CanvasItemUtils.DEFAULT_COLOR;

	[Export]
	public Color DebugColor
	{
		get => _debugColor;
		set
		{
			_debugColor = value;
			UpdateRect();
		}
	}

	private Vector2 _size = Vector2.One * 512f;

	[Export]
	public Vector2 Size
	{
		get => _size;
		set
		{
			_size = value;
			UpdateRect();
		}
	}

	private Rid _rectRid = new();

	private void UpdateRect()
	{
		CanvasItemUtils.TryFreeCanvasItem(_rectRid);

		Rect2 rect = new(Vector2.Zero, _size);
		_rectRid = CanvasItemUtils.CreateRect(rect, _debugColor, GetCanvasItem());
	}

	public Vector2 GetRandomPoint()
	{
		return RandUtils.RandVector2(GlobalPosition, GlobalPosition + _size);
	}

	public void OnBeforeSerialize()
	{ }

	public void OnAfterDeserialize()
	{
		CanvasItemUtils.TryFreeCanvasItem(_rectRid);
	}

	public override void _Ready()
	{
		if (!_debugDraw)
		{
			CanvasItemUtils.TryFreeCanvasItem(_rectRid);
		}
	}
}
