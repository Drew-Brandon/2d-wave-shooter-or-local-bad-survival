namespace Inventory;
using Godot;
using System.Collections.Generic;

public partial class PlayerInventory : Node
{
	private struct ItemInstanceObject
	{
		private Vector2 _pos = Vector2.Zero;
		public Vector2 Pos { get => _pos; }

		private BaseItemInstance _instance = null;
		public BaseItemInstance Instance { get => _instance; }

		public ItemInstanceObject(Vector2 pos, BaseItemInstance instance)
		{
			_pos = pos;
			_instance = instance;
		}
	}

	private List<ItemInstanceObject> _contents = new();

	[Export]
	private ShapeCast2D _inventoryShape = null;

	public void UpdateInventory()
	{
		_inventoryShape.ForceShapecastUpdate();

		for (int i = 0; i < _inventoryShape.GetCollisionCount(); i++)
		{
			 _inventoryShape.GetCollider(i);
		}
	}
}
