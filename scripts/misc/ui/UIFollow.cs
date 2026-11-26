using Godot;

public partial class UIFollow : Node2D
{
	[Export]
	private float _followRadius = 32f;

	[Export]
	private Node2D _toFollow = null;
	public Node2D ToFollow { get => _toFollow; set => _toFollow = value; }

	[Export]
	private Node2D _toFace = null;
	public Node2D ToFace { get => _toFace; set => _toFace = value; }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_toFollow != null && _toFace != null)
		{
			Vector2 nodeDisp = _toFace.GlobalPosition - _toFollow.GlobalPosition;
			float nodeDist = nodeDisp.Length();
			float nodeAngle = nodeDisp.Angle();
			float followAngle = nodeAngle;
			Vector2 followDisp = Vector2.FromAngle(followAngle) * _followRadius;

			GlobalPosition = _toFollow.GlobalPosition - followDisp;
		}
	}
}
