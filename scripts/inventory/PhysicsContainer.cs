namespace Inventory;
using Godot;

/// <summary>
/// Represents a physics based container that can store physics based items.
/// </summary>
public partial class PhysicsContainer : Node
{
	[Export]
	private ShapeCast2D _areaCast = null;

	/// <summary>
	/// Triggered when the containers contents are supposed to be saved.
	/// </summary>
	/// <param name="items">
	/// The items to save.
	/// </param>
	[Signal]
	public delegate void OnContainerSavedEventHandler(PhysicsItem[] items);

	/// <summary>
	/// Saves the container's contents.
	/// </summary>
	public void SaveContainer()
	{
		_areaCast.ForceShapecastUpdate();
		PhysicsItem[] items = new PhysicsItem[_areaCast.GetCollisionCount()];

		for (int i = 0; i < _areaCast.GetCollisionCount(); i++)
		{
			items[i] = (PhysicsItem)_areaCast.GetCollider(i);
		}

		EmitSignal(SignalName.OnContainerSaved, items);
	}
}
