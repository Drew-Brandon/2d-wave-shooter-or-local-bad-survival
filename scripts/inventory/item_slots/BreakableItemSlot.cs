namespace Inventory;
using Godot;

public partial class BreakableItemSlot : ItemSlotGeneric<BreakableInstance>
{
	public float Durability { get => Instance.Durability; }

	public float MaxDurability { get => Instance.Item.MaxDurability; }

	/// <summary>
	/// A filled constructor for setting the fields to specified values.
	/// </summary>
	/// <param name="instance">
	/// The instance to store in the slot.
	/// </param>
	public BreakableItemSlot(BreakableInstance instance) : base(instance)
	{ }

	public override SlotDisplayInfo GetDisplayInfo()
	{
		return new SlotDisplayInfo(Item.Icon, 0, Durability / MaxDurability);
	}

	public void AddDurability(float amount)
	{
		// Modify the durability
		float prevDurability = Durability;
		Instance.Durability = Mathf.Clamp(Durability + amount, 0f, MaxDurability);

		// If no change occurred, then don't do anything else.
		if (Instance.Durability == prevDurability)
		{
			return;
		}

		// Clear the slot if the instance broke, otherwise just notify the change.
		if (Instance.Durability <= 0f)
		{
			ClearSlot();
		}
		else
		{
            NotifyChange();
        }
	}
}
