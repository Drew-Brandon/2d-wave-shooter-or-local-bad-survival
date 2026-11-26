namespace Inventory.Physics;
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
	public delegate void ContainerSavedEventHandler(PhysicsItemInstanceAugmenter[] items);

	/// <summary>
	/// Saves the container's contents.
	/// </summary>
	public void SaveContainer()
	{
		_areaCast.ForceShapecastUpdate();
		PhysicsItemInstanceAugmenter[] items = new PhysicsItemInstanceAugmenter[_areaCast.GetCollisionCount()];

		for (int i = 0; i < _areaCast.GetCollisionCount(); i++)
		{
			items[i] = (PhysicsItemInstanceAugmenter)_areaCast.GetCollider(i);
		}

		EmitSignal(SignalName.ContainerSaved, items);
	}
}
