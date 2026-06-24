namespace Inventory;
using Godot;

/// <summary>
/// Represents an instance of the base item class.
/// Instances store items alongside info like their stack amount, durability and more.
/// </summary>
public partial class BaseItemInstance : RefCounted
{
	/// <summary>
	/// A reference to the internal item this instance is storing.
	/// </summary>
	[Export]
	public BaseItem GenericItem = null;

	/// <summary>
	/// A blank constructor for leaving all fields blank.
	/// </summary>
	public BaseItemInstance()
	{ }

	/// <summary>
	/// A filled constructor for setting the fields to specified values.
	/// </summary>
	/// <param name="item">
	/// The item to store within this instance.
	/// </param>
	public BaseItemInstance(BaseItem item)
	{
        GenericItem = item;
	}

    public override string ToString()
    {
		return GenericItem.ToString();
    }
}
