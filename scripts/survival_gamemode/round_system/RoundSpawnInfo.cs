using Godot;

public partial class RoundSpawnInfo : RefCounted
{
    [Export]
    private int _spawnCount = 0;
    public int SpawnCount { get => _spawnCount; set => _spawnCount = value; }

    [Export]
    private PackedScene _spawnScene = null;
    public PackedScene SpawnScene { get => _spawnScene; }

    [Export]
    private PackedScene _toolScene = null;
    public PackedScene ToolScene { get => _toolScene; }
}
