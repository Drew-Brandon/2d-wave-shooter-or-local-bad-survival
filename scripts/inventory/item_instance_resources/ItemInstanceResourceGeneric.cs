namespace Inventory;
using Godot;

[Tool]
public partial class ItemInstanceResourceGeneric<T> : BaseItemInstanceResource where T : BaseItemInstance, new()
{
	public override BaseItemInstance GetInstance()
	{
		return GetGenericInstance<T>();
	}

	public ItemInstanceResourceGeneric()
	{
		InitGenericProperties<T>($"");
	}
}
