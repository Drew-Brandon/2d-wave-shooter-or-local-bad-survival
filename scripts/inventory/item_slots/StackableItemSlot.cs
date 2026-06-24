namespace Inventory;
using Godot;

/// <summary>
/// Represents an item slot that stores stackable instances.
/// </summary>
public partial class StackableItemSlot : ItemSlotGeneric<StackableInstance>, BaseItemSlot.IMergeable
{
    /// <summary>
    /// The current amount of items within the stack.
    /// </summary>
    public int Amount { get => Instance.Amount; }

	/// <summary>
	/// The maximum amount of items that can be stored in the stack.
	/// </summary>
	public int MaxAmount { get => Instance.Item.MaxAmount; }

    /// <summary>
    /// A filled constructor for setting the fields to specified values.
    /// </summary>
    /// <param name="instance">
    /// The instance to store in the slot.
    /// </param>
    public StackableItemSlot(StackableInstance stack) : base(stack)
	{ }

    public override SlotDisplayInfo GetDisplayInfo()
    {
		return new SlotDisplayInfo(Item.Icon, Amount);
    }

    public override bool HasInstance(BaseItemInstance instance)
	{
		StackableInstance stack = instance as StackableInstance;
		return stack != null && stack.Item == Item && Amount >= stack.Amount;
	}

	public bool MergeWith(BaseItemInstance other)
	{
        // Make sure the instance being merged with is stackable, and that it has the same item.
        StackableInstance otherStack = other as StackableInstance;

		if (otherStack != null && otherStack.Item == Item)
		{
            // Calculate the new amount, and confirm that its different than the current value.
            int newAmount = Mathf.Clamp(Instance.Amount + otherStack.Amount, 0, otherStack.Item.MaxAmount);

            if (newAmount == Instance.Amount)
            {
                return false;
            }

            // Update both the stack being charged with and the stack in the slot.
            otherStack.Amount -= newAmount - Instance.Amount;
			Instance.Amount = newAmount;

			// Clear the slot if the instance's stack becomes empty, otherwise just notify the change.
            if (Instance.Amount <= 0)
			{
				ClearSlot();
			}
			else
			{
				NotifyChange();
			}

			return otherStack.Amount <= 0;
		}

		return false;
	}

	public bool ChargeWith(BaseItemInstance other)
	{
		// Make sure the instance being charged with is stackable, and that it has the same item.
		StackableInstance otherStack = other as StackableInstance;

		if (otherStack != null && otherStack.Item == Item)
		{
			// Calculate the new amount, and confirm that its different than the current value.
			int newAmount = Mathf.Clamp(Instance.Amount - otherStack.Amount, 0, otherStack.Item.MaxAmount);

			if (newAmount == Instance.Amount)
			{
				return false;
			}

			// Update both the stack being charged with and the stack in the slot.
			otherStack.Amount -= Instance.Amount - newAmount;
			Instance.Amount = newAmount;

            // Clear the slot if the instance's stack becomes empty, otherwise just notify the change.
            if (Instance.Amount <= 0)
			{
				ClearSlot();
			}
			else
			{
				NotifyChange();
			}

			return (otherStack.Amount <= 0);
		}

		return false;
	}

	/// <summary>
	/// Adds the specified amount to the stored stack.
	/// </summary>
	/// <param name="toAdd">
	/// The amount to add.
	/// </param>
	public void AddAmount(int toAdd)
	{
		// Modify the amount.
		int prevAmount = Instance.Amount;
		Instance.Amount = Mathf.Clamp(Amount + toAdd, 0, MaxAmount);

		// If no change occurred, then don't do anything else.
		if (prevAmount == Instance.Amount)
		{
			return;
		}

		// Clear the slot if the instance's stack becomes empty, otherwise just notify the change.
		if (Instance.Amount <= 0)
		{
			ClearSlot();
		}
		else
		{
			NotifyChange();
		}
	}
}
