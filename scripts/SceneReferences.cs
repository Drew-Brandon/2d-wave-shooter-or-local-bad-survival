using Godot;

public partial class SceneReferences : Node2D
{
	[Export]
	private PlayerBody2D _plyr;
	public PlayerBody2D Plyr { get => _plyr; }

	private World2D _world = null;
	public World2D World { get => _world; }

	public override void _Ready()
	{
		_world = GetWorld2D();
	}
}
