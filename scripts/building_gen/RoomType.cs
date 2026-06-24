using Godot;

[GlobalClass]
public partial class RoomType : Resource
{
	[Export]
	private float _minScale = 0.1f;

	[Export]
	private float _maxScale = 0.2f;

	[Export]
	private StringName _displayName = $"";
}
