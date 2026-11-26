namespace CastInfo2D;
using Godot;

public partial struct PointCastResults2D
{
	public int Shape = -1;

	public Rid ColliderRid = new();

	public GodotObject Collider = null;

	public PointCastResults2D()
	{ }
}
