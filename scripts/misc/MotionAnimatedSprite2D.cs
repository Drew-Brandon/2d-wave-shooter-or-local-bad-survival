using Godot;

[GlobalClass]
public partial class MotionAnimatedSprite2D : AnimatedSprite2D
{
	[Export]
	private float _minSpeed = 10f;

	[Export]
	private float _baseSpeed = 100f;

	public void UpdateMotion(Vector2 velocity)
	{
		float speed = velocity.Length();

		if (speed < _minSpeed)
		{
			if (IsPlaying())
			{
				Stop();
			}
		}
		else
		{
			if (!IsPlaying())
			{
				Play();
			}

			GlobalRotation = velocity.Angle();
			SpeedScale = speed / _baseSpeed;
		}
	}
}
