using Puppets.Bodies;

/// <summary>
/// The interface to use for objects that can be interacted with by the player.
/// </summary>
public interface IInteractable
{
	/// <summary>
	/// Starts an interaction with the related object.
	/// </summary>
	/// <param name="player">
	/// The player performing the interaction.
	/// </param>
	/// <returns>
	/// Whether or not the interaction continues.
	/// If the interaction continues,
	/// then it will not stop until either StopInteraction is called,
	/// or InteractionUpdate returns true.
	/// </returns>
	public bool StartInteraction(PlayerBodyTop2D player);

	public void StopInteraction(PlayerBodyTop2D player)
	{ }
}
