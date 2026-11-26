namespace HealthSources;
using Godot;

/// <summary>
/// Used to represent something that has or is attached to something with health.
/// </summary>
public interface IHealthSource
{
	/// <summary>
	/// Adds health to the source.
	/// Note that this can be used to both heal and damage the source.
	/// </summary>
	/// <param name="amount">
	/// The amount of health to add.
	/// </param>
	/// <param name="instigator">
	/// The instigator of the health addition.
	/// </param>
	public void AddHealth(float amount, Node instigator = null);

	/// <summary>
	/// Kills the source.
	/// </summary>
	/// <param name="instigator">
	/// The instigator of the death.
	/// </param>
	public void Kill(Node instigator);
}
