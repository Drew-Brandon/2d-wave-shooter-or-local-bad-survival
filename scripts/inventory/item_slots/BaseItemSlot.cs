namespace Inventory;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents an item slot that contains an instance.
/// This class can then be used to modify said instance in a variety of ways.
/// </summary>
public partial class BaseItemSlot : RefCounted
{
	/// <summary>
	/// The reference to the base instance being stored.
	/// </summary>
	protected BaseItemInstance InternalInstance = null;

    public BaseItemInstance GenericInstance { get => InternalInstance; }

	/// <summary>
	/// The item being stored in this item slot.
	/// </summary>
	public BaseItem Item { get => InternalInstance.GenericItem; }

    [Signal]
    public delegate void OnSlotClearedEventHandler();

    [Signal]
    public delegate void OnSlotChangedEventHandler();

    /// <summary>
    /// A filled constructor for setting the fields to specified values.
    /// </summary>
    /// <param name="instance">
	/// The instance to store in the slot.
	/// </param>
	/// <param name="inventory">
	/// The inventory that contains this slot.
	/// </param>
    public BaseItemSlot(BaseItemInstance instance)
	{
		InternalInstance = instance;
    }

    protected void NotifyChange()
    {
        EmitSignal(SignalName.OnSlotChanged);
    }

    public void ClearSlot()
    {
        EmitSignal(SignalName.OnSlotCleared);
    }

    /// <summary>
    /// Returns whether or not the slot has the specified instance.
    /// </summary>
    /// <param name="instance">
    /// The instance to check for.
    /// </param>
    /// <returns>
    /// True if the slot contains the instance.
    /// False if otherwise.
    /// </returns>
    public virtual bool HasInstance(BaseItemInstance instance)
	{
		return Item == instance.GenericItem;
	}

    /// <summary>
    /// Gets the display info for a slot.
    /// </summary>
    /// <returns>
    /// The display info for said slot.
    /// </returns>
    public virtual SlotDisplayInfo GetDisplayInfo()
    {
        return new SlotDisplayInfo(Item.Icon);
    }

    public override string ToString()
    {
        return InternalInstance.ToString();
    }

	/// <summary>
	/// Represents a slot that can be merged with other slots.
	/// </summary>
	public partial interface IMergeable
	{
        /// <summary>
        /// Merges the slot with another instance.
        /// </summary>
        /// <param name="other">
        /// The instance to merge with.
        /// </param>
        /// <returns>
        /// True if the merge resulted in the other instance becoming empty.
        /// False if otherwise.
        /// </returns>
        public bool MergeWith(BaseItemInstance other)
		{
			return false;
		}

        /// <summary>
        /// Merges the slot with another slot.
        /// </summary>
        /// <param name="other">
		/// The slot to merge with.
		/// </param>
        /// <returns>
		/// True if the merge resulted in the other slot becoming empty.
		/// False if otherwise.
		/// </returns>
        public bool MergeWith(BaseItemSlot other)
		{
			bool isDone = MergeWith(other.InternalInstance);

			if (isDone)
			{
				other.ClearSlot();
            }
			
			return isDone;
		}

        /// <summary>
        /// Charges the slot with another instance.
        /// </summary>
        /// <param name="other">
        /// The instance to charge with.
        /// </param>
        /// <returns>
        /// Whether the 
        /// </returns>
        public bool ChargeWith(BaseItemInstance other)
		{
			return false;
		}
	}
}
