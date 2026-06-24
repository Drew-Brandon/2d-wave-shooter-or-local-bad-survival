namespace Inventory;
using Godot;

/// <summary>
/// Represents a stack/collection of items.
/// </summary>
public partial class StackableInstance : ItemInstanceGeneric<StackableItem>
{
	/// <summary>
	/// The current amount of items within the stack.
	/// </summary>
	[Export]
	public int Amount = 0;

    /// <summary>
    /// A blank constructor for leaving all fields blank.
    /// </summary>
    public StackableInstance() : base()
	{
		Amount = 0;
	}

    /// <summary>
    /// A filled constructor for setting the fields to specified values.
    /// </summary>
	/// <param name="item">
    /// The item to be stored within this stack.
    /// </param>
    /// <param name="amount">
    /// The amount of items to be stored within this stack.
    /// </param>
    public StackableInstance(StackableItem item, int amount) : base(item)
	{
		Amount = amount;
	}
}
