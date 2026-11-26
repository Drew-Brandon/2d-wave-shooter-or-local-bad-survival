using Godot;

[GlobalClass]
public partial class RoundInfo : Resource
{
    private static readonly StringName SPAWN_INFO_PROPERTY_NAME = "spawn_info";

    [Export]
    private int _spawnCooldown = 1000;
    public int SpawnCooldown { get => _spawnCooldown; }

    [Export]
    private float _escalationRate = 1.2f;

    [Export]
    private RoundSpawnInfoBlueprint[] _spawnInfo = null;

    public RoundSpawnInfo[] GetSpawnInfo(int escalation)
    {
        RoundSpawnInfo[] spawnInfo = new RoundSpawnInfo[_spawnInfo.Length];

        for (int i = 0; i < _spawnInfo.Length; i++)
        {
            spawnInfo[i] = _spawnInfo[i].CreateObject();
            spawnInfo[i].SpawnCount = Mathf.FloorToInt(spawnInfo[i].SpawnCount * Mathf.Pow(_escalationRate, escalation));
        }

        return spawnInfo;
    }
}
