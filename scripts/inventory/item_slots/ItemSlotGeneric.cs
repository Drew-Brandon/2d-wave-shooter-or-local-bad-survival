namespace Inventory;

/// <summary>
/// A generic typed version of the item slot class.
/// </summary>
/// <typeparam name="T">
/// The type of instance to store in this slot.
/// </typeparam>
public partial class ItemSlotGeneric<T> : BaseItemSlot where T : BaseItemInstance
{
    /// <summary>
    /// The generic typed version of the stored instance.
    /// </summary>
    protected T Instance { get => (T)InternalInstance; set => InternalInstance = value; }

    /// <summary>
    /// A filled constructor for setting the fields to specified values.
    /// </summary>
    /// <param name="instance">
	/// The instance to store in the slot.
	/// </param>
	public ItemSlotGeneric(T instance) : base(instance)
	{ }
}
