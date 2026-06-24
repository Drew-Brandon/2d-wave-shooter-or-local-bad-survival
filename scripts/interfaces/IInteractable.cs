public interface IInteractable
{
	public bool Interact(PlayerBody2D plyr);

	public void StopInteract(PlayerBody2D plyr)
	{ }

	public bool InteractUpdate(float delta, PlayerBody2D plyr)
	{
		return false;
	}
}
