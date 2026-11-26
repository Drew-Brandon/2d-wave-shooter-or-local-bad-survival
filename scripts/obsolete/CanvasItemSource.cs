using Godot;

public partial class CanvasItemSource
{
	private Rid _item = new();
	public Rid Item
	{
		get => _item;
		set
		{
			if (_item.IsValid && _item != value)
			{
				Clear();
			}

			_item = value;
		}
	}

	~CanvasItemSource()
	{
		if (_item.IsValid)
		{
			Clear();
		}
	}

	public void ChangeItem(Rid newItem)
	{
		_item = newItem;
	}

	public void Clear()
	{
		CanvasItemUtils.FreeCanvasItem(_item);
		_item = new();
	}
}
