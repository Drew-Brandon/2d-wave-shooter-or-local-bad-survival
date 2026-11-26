namespace Inventory.Physics;
using Godot;
using Inventory.ItemHandlers;

public partial class PhysicsItemInfo : RefCounted
{
    /*/// <summary>
    /// The transform of the item.
    /// </summary>
    public Transform2D Transform = new();
    
    /// <summary>
    /// The scene that the item was created from.
    /// </summary>
    public PackedScene Scene = null;

    public BaseItemHandler Augmenter = null;

    /// <summary>
    /// Creates a new PhysicsItemInfo based off the given item.
    /// </summary>
    /// <param name="item">
    /// The item to store info about.
    /// </param>
    public PhysicsItemInfo(PhysicsItemInstanceAugmenter item)
    {
        Transform = item.Transform;
        Scene = GD.Load<PackedScene>(item.SceneFilePath);
        Augmenter = item.Augmenter;
    }*/
}
