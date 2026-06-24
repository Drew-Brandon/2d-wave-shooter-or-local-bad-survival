namespace Inventory;
using Godot;

public partial class PhysicsItem : RigidBody2D
{
	private BaseItemInstance _instance = null;
	public BaseItemInstance Instance { get => _instance; set => _instance = value; }
}
