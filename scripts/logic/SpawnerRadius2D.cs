using Godot;

[GlobalClass]
public partial class SpawnerRadius2D : Spawner2D
{
	[Export]
	private float _radius = 128f;

	public float Radius { get => _radius; }

    protected override void OnSpawn(Vector2 position, float rotation, Node2D spawned)
    {
		spawned.Position = position + RandUtils.RandDir() * RandUtils.RandRangef(0f, _radius);
        spawned.Rotation = rotation;
    }
}
