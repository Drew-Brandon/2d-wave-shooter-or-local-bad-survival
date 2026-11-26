namespace HealthSources;
using Godot;

/// <summary>
/// A node that has health.
/// </summary>
public partial class HealthSource : Node, IHealthSource
{
	private bool _isTryingToKill = false;

    private bool _isAlive = true;

	/// <summary>
	/// Whether the source is alive or not.
	/// </summary>
	public bool IsAlive { get => _isAlive; }

	[Export]
	private bool _canDie = true;

	/// <summary>
	/// Whether the source can die or not.
	/// </summary>
	public bool CanDie { get => _canDie; set => _canDie = value; }

	[Export]
	private float _maxHealth = 50f;

	/// <summary>
	/// The maximum health of the source.
	/// </summary>
	public float MaxHealth { get => _maxHealth; }

	[Export]
	private float _curHealth = 50f;

	/// <summary>
	/// The current health of the source.
	/// </summary>
	public float CurHealth { get => _curHealth; }

    /// <summary>
	/// Triggered when the health of the source is changed.
	/// </summary>
	/// <param name="health">
	/// The new amount of health.
	/// </param>
	/// <param name="instigator">
	/// The instigator of the health change.
	/// </param>
	[Signal]
	public delegate void HealthChangedEventHandler(float health, Node instigator);

	/// <summary>
	/// Triggered when the source is damaged.
	/// </summary>
	/// <param name="health">
	/// The new health of the source.
	/// </param>
	/// <param name="instigator">
	/// The instigator of the damage.
	/// </param>
	[Signal]
	public delegate void DamagedEventHandler(float health, Node instigator);

	/// <summary>
	/// Triggered when the source is healed.
	/// </summary>
	/// <param name="health">
	/// The new health of the source.
	/// </param>
	/// <param name="instigator">
	/// The instigator of the heal.
	/// </param>
	[Signal]
	public delegate void HealedEventHandler(float health, Node instigator);

	/// <summary>
	/// Triggered when the source dies.
	/// </summary>
	/// <param name="instigator">
	/// The instigator of the death.
	/// </param>
	[Signal]
	public delegate void DiedEventHandler(Node instigator);

	private void InternKill(Node instigator)
	{
		// Plainly kill the source.
		_isAlive = false;
		_curHealth = 0f;
		EmitSignal(SignalName.Died, instigator);
	}

	private void TryToKill(Node instigator)
	{
		// Check if the source should still be killed.
		if (_curHealth <= 0f)
		{
			InternKill(instigator);
		}

		_isTryingToKill = false;
	}

	public void Kill(Node instigator)
	{
		if (_isAlive)
		{
			InternKill(instigator);
		}
	}

    public void AddHealth(float amount, Node instigator = null)
	{
		// Make sure the source is alive as they cannot be resurrected.
		if (_isAlive)
		{
			_curHealth = Mathf.Clamp(_curHealth + amount, 0f, _maxHealth);
			EmitSignal(SignalName.HealthChanged, _curHealth, instigator);

			/* 
			 * Call the approriate signals and functions based on whether the source
			 * was damaged or healed.
			 */
			if (amount < 0f)
			{
				// If the source can't die, then don't bother trying to kill it.
				if (_canDie)
				{
					EmitSignal(SignalName.Damaged, _curHealth, instigator);

					/* 
					 * Check if the source could be killed
					 * and ensure the kill check is only queued once per frame.
					 */
					if (_curHealth <= 0f && !_isTryingToKill)
					{
						/* 
						 * Queue a check for whether the source should be killed.
						 * This is done in the case that health is added back within the same frame.
						 */
						_isTryingToKill = true;
						CallDeferred(MethodName.TryToKill, instigator);
					}
				}
			}
			else if (amount > 0f)
			{
				EmitSignal(SignalName.Healed, _curHealth, instigator);
			}
		}
	}
}
