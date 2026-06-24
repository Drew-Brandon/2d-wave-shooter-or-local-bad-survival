namespace Inventory;

public partial class ItemNodeGeneric<T> : BaseItemNode where T : BaseItemSlot
{
	public T Slot { get => GenericSlot as T; }
}
