namespace CustomCasting;
using Godot;

public partial struct PointCastResults
{
	public int Shape = -1;

	public Rid ColliderRid = new();

	public GodotObject Collider = null;

	public PointCastResults()
	{ }
}
