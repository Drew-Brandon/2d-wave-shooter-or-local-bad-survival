namespace Inventory;
using Godot;

/// <summary>
/// A generic typed version of the item instance class.
/// </summary>
/// <typeparam name="T">
/// The type of item to store within the instance.
/// </typeparam>
public partial class ItemInstanceGeneric<T> : BaseItemInstance where T : BaseItem
{
	/// <summary>
	/// The generic typed version of the stored item.
	/// </summary>
	public T Item { get => GenericItem as T; set => GenericItem = value; }

    /// <summary>
    /// A blank constructor for leaving all fields blank.
    /// </summary>
    public ItemInstanceGeneric() : base()
	{ }

    /// <summary>
    /// A filled constructor for setting the fields to specified values.
    /// </summary>
    /// <param name="item">
    /// The item to store within this instance.
    /// </param>
    public ItemInstanceGeneric(T item) : base(item)
	{ }
}
