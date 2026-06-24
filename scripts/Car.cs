using Godot;

public partial class Car : RigidBody2D
{
	[Export]
	private float _maxSpeed = 256f;

	[Export]
	private float _breakDamp = 128f;

	[Export]
	private float _turnForce = 1024f;

	[Export]
	private float _driveForce = 1024f;

	[Export]
	private Node2D _turnPoint = null;

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("break"))
		{
			LinearDamp = _breakDamp;
		}
		else
		{
			LinearDamp = 0f;
			float inputForce = Input.GetAxis("decelerate", "accelerate");

			if (inputForce != 0.0f && LinearVelocity.Length() < _maxSpeed)
			{
				ApplyCentralForce(Transform.X * _driveForce * inputForce);
			}
		}

		float inputTurn = Input.GetAxis("turn_left", "turn_right");
		
		if (inputTurn != 0.0f)
		{
			ApplyForce(Transform.Y * inputTurn * _turnForce * (LinearVelocity.Length() / _maxSpeed), _turnPoint.GlobalPosition - GlobalPosition);
		}

		LinearVelocity = LinearVelocity.Project(Transform.X);
	}
}
