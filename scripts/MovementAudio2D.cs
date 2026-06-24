using Godot;

public partial class MovementAudio2D : AudioStreamPlayer2D
{
	[Export]
	private bool _physicsProcess = true;

	[Export]
	private bool _trackSpeed = true;

	private float _curSpeed = 0f;

	public float Speed { get => _curSpeed; set => _curSpeed = value; }

	[Export]
	private float _minSpeed = 0.1f;

	[Export]
	private float _refSpeed = 100f;

	private Vector2 _prevPos = Vector2.Zero;

	[Export]
	private TileMapLayerPlus _tileMap = null;

	[Export]
	private Node2D _movingNode = null;

	[Export]
	private SurfaceAudioLibrary _audioLibrary = new();

	private void UpdateAudio()
	{
		if (_trackSpeed)
		{
			Vector2 disp = _prevPos - _movingNode.GlobalPosition;
			_curSpeed = disp.Length();
			_prevPos = _movingNode.GlobalPosition;
		}

		/* 
		 * Handle the playing of footsteps.
		 * Make sure to not replay the audio, and stop it when not moving.
		 */
		_audioLibrary.UpdatePlayer(this, _tileMap, _curSpeed, _minSpeed, _refSpeed);
	}

	public override void _Ready()
	{
		SetProcess(!_physicsProcess);
		SetPhysicsProcess(_physicsProcess);
	}

	public override void _Process(double delta)
	{
		UpdateAudio();
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateAudio();
	}
}
