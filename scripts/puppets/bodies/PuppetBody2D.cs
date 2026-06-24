using Godot;

/// <summary>
/// Represents a CharacterBody2D that can be controlled either by a player or AI.
/// </summary>
public partial class PuppetBody2D : CharacterBody2D, IDamageable//, ISaveable
{
	[Export]
	private float _mass = 70f;

	[Export]
	private TileMapLayerPlus _tileMap = null;

	[ExportGroup("Health and Damage")]
	private bool _isAlive = true;

	/// <summary>
	/// Whether the character is alive or not.
	/// </summary>
	public bool IsAlive { get => _isAlive; }

	[Export]
	private bool _canDie = true;

	/// <summary>
	/// Whether the character can die or not.
	/// </summary>
	public bool CanDie { get => _canDie; set => _canDie = value; }

	[Export]
	private float _maxHealth = 50f;

	/// <summary>
	/// The maximum health of the character.
	/// </summary>
	public float MaxHealth { get => _maxHealth; }

	[Export]
	private float _curHealth = 50f;

	/// <summary>
	/// The current health of the character.
	/// </summary>
	public float CurHealth { get => _curHealth; }

	//[Export]
	//private ShapeCastInfo _puppetCast = null;

	[ExportGroup("Movement")]
	private float _baseSpeed = 0f;

	/// <summary>
	/// The base speed that the puppet can move at.
	/// Note that this is not necessarily the speed of the puppet,
	/// but is instead the speed the puppet should be moving at if no extra effects are being applied.
	/// </summary>
	public float BaseSpeed { get => _baseSpeed; set => _baseSpeed = value; }

	private float _curSpeed = 0f;

	/// <summary>
	/// The current speed that the puppet is moving at.
	/// </summary>
	public float CurSpeed { get => _curSpeed; }

	private Vector2 _curDir;

	/// <summary>
	/// The current direction that the character moves in.
	/// </summary>
	public Vector2 CurDir { get => _curDir; set => _curDir = value; }

	[Export]
	private bool _isFrozen = false;

	/// <summary>
	/// Whether or not the character is frozen and thus cannot move.
	/// </summary>
	public bool IsFrozen { get => _isFrozen; set => _isFrozen = value; }

	[Export]
	private float _walkSpeed = 100f;
	
	/// <summary>
	/// The walking speed of this character.
	/// </summary>
	public float WalkSpeed { get => _walkSpeed; }

	[Export]
	private float _runSpeed = 150f;

	/// <summary>
	/// The running speed of this character.
	/// </summary>
	public float RunSpeed { get => _runSpeed; }

	[Export]
	private float _velWeight = 100f;

	[Export]
	private float _pushPower = 4f;

	[Export]
	private AudioStreamPlayer2D _footstepPlayer;

	[Export]
	private SurfaceAudioLibrary _footstepAudio;

	private float _curSpeedFactor = 1f;

	/// <summary>
	/// The scale applied to the current speed.
	/// </summary>
	protected float CurSpeedFactor { get => _curSpeedFactor; set => _curSpeedFactor = value; }

	/// <summary>
	/// Triggered when the health of the character is changed.
	/// </summary>
	/// <param name="health">
	/// The new amount of health.
	/// </param>
	[Signal]
	public delegate void OnHealthUpdatedEventHandler(float health);

	/// <summary>
	/// Triggered when the character is damaged.
	/// </summary>
	/// <param name="health">
	/// The new health of the character.
	/// </param>
	[Signal]
	public delegate void OnDamagedEventHandler(float health);

	/// <summary>
	/// Triggered when the character is healed.
	/// </summary>
	/// <param name="health">
	/// The new health of the character.
	/// </param>
	[Signal]
	public delegate void OnHealedEventHandler(float health);

	/// <summary>
	/// Triggered when the character dies.
	/// </summary>
	[Signal]
	public delegate void OnDeathEventHandler();

	/// <summary>
	/// An internal function that kills the puppet.
	/// This does not do the additional safety check of confirming the puppet is alive,
	/// and thus should be called when that check will knowingly be true.
	/// </summary>
	private void InternKill()
	{
		_isAlive = false;
		_curHealth = 0f;
		OnKill();
		EmitSignal(SignalName.OnDeath);
	}
	
	/// <summary>
	/// Moves the puppet at its specified velocity with footstep sounds being played
	/// and physics objects being pushed.
	/// </summary>
	/// <param name="delta">
	/// The delta time that will be used to make the update framerate independent.
	/// </param>
	protected void FullMoveUpdate(float delta)
	{
		Velocity = VelocityUpdate(delta, Velocity);
		Vector2 disp = MoveUpdate();
		FootstepUpdate(_curSpeed);
		PushUpdate();
	}

	/// <summary>
	/// Moves the puppet at its specified velocity.
	/// </summary>
	/// <returns>
	/// The amount that the puppet moved by.
	/// </returns>
	protected Vector2 MoveUpdate()
	{
		// Move the puppet.
		Vector2 prevPos = GlobalPosition;
		MoveAndSlide();
		return GlobalPosition - prevPos;
	}

	/// <summary>
	/// Updates the given velocity value.
	/// </summary>
	/// <param name="delta">
	/// The delta time to update the velocity with.
	/// This allows for framerate independent updates.
	/// </param>
	/// <param name="vel">
	/// The velocity value to update.
	/// </param>
	/// <returns>
	/// The new value for the velocity.
	/// </returns>
	protected Vector2 VelocityUpdate(float delta, Vector2 vel)
	{
		// Get the target velocity, ease into it, and move the character.
		Vector2 targVel;

		if (_isFrozen)
		{
			_curSpeed = 0f;
			targVel = Vector2.Zero;
		}
		else
		{
			_curSpeed = CurDir.Length() * _baseSpeed * CurSpeedFactor;
            targVel = CurDir * _curSpeed;
		}
		
		return vel.Lerp(targVel, _velWeight * delta);
	}

	/// <summary>
	/// Updates the footstep sound effects for this puppet.
	/// </summary>
	/// <param name="speed">
	/// The speed that is being moved at to trigger said footsteps.
	/// </param>
	protected void FootstepUpdate(float speed)
	{
		_footstepAudio.UpdatePlayer(_footstepPlayer, _tileMap, speed, 0.1f, _walkSpeed);
    }

	/// <summary>
	/// Pushes any objects in the puppet's way when moving.
	/// </summary>
	protected void PushUpdate()
	{
		// Push colliding RigidBody2Ds out of the way.
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D col = GetSlideCollision(i);
			RigidBody2D rb = col.GetCollider() as RigidBody2D;

			if (rb != null)
			{
				// Credit to https://www.youtube.com/watch?v=Uh9PSOORMmA
				Vector2 pushDir = -col.GetNormal();
				Vector2 pushPos = col.GetPosition();
				float velDiff = Mathf.Max(0f, Velocity.Dot(pushDir) - rb.LinearVelocity.Dot(pushDir));
				float massRatio = Mathf.Min(1f, _mass / rb.Mass);
				float adjPushPower = massRatio * _pushPower;
				rb.ApplyImpulse(pushDir * velDiff * adjPushPower, pushPos - rb.GlobalPosition);
			}
		}
	}

	/// <summary>
	/// Called when the character is killed.
	/// </summary>
	protected virtual void OnKill()
	{
		QueueFree();
	}

	public override void _Ready()
	{
		_curHealth = _maxHealth;
		_baseSpeed = _walkSpeed;
	}

	public void Walk()
	{
		_baseSpeed = _walkSpeed;
	}

	public void Run()
	{
		_baseSpeed = _runSpeed;
	}

	/// <summary>
	/// Kills the puppet.
	/// This method does a safety check to make sure the puppet is not already killed.
	/// </summary>
	public void Kill()
	{
		if (_isAlive)
		{
			InternKill();
		}
	}

	/// <summary>
	/// Called when the health of the puppet is updated.
	/// </summary>
	/// <param name="instigator">
	/// The node that caused the health value to update.
	/// </param>
	protected virtual void HealthUpdated(Node instigator)
	{

	}
	
	public void AddHealth(Node instigator, float amount)
	{
		// Make sure the puppet is alive to avoid possible issues.
		if (_isAlive)
		{
			_curHealth = Mathf.Clamp(_curHealth + amount, 0f, _maxHealth);
			HealthUpdated(instigator);
			EmitSignal(SignalName.OnHealthUpdated, _curHealth);

			/* 
			 * Call the approriate signals and functions based on whether the puppet
			 * was damaged or healed.
			 */
			if (amount < 0f)
			{
				if (_canDie)
				{
					EmitSignal(SignalName.OnDamaged, _curHealth);

					if (_curHealth <= 0f)
					{
						InternKill();
					}
				}
			}
			else if (amount > 0f)
			{
				EmitSignal(SignalName.OnHealed, _curHealth);
			}
		}
	}

	/*public NodeSave GetSave()
	{
		return new Puppet2DSave(Transform, _curHealth);
	}

	public void LoadSave(NodeSave save)
	{
		Puppet2DSave puppetSave = save as Puppet2DSave;

		if (puppetSave == null)
		{
			return;
		}

		Transform = puppetSave.Transform;
		_curHealth = puppetSave.Health;

		if (_curHealth <= 0f)
		{
			Kill();
		}
	}*/
}
