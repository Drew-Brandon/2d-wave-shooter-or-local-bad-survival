using Godot;

public partial class Scatterer2D : Node2D
{
	[Export]
	private float _reach = 8f;

	[Export]
	private PackedScene _scene = null;

	public void Scatter()
	{
		Node2D node = _scene.Instantiate<Node2D>();
		Owner.AddChild(node);

		float angle = RandUtils.RandRangef(0, Mathf.Pi * 2f);
		node.GlobalPosition = GlobalPosition + Vector2.FromAngle(angle) * _reach;
		node.GlobalRotation = RandUtils.RandRangef(0f, Mathf.Pi * 2f);
	}
}
