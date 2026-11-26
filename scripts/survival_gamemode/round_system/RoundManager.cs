using CastInfo2D;
using Godot;
using Godot.Collections;
using Puppets.Bodies;
using System;
using System.Collections;

public partial class RoundManager : Node2D
{
    private int _currentRound = 0;

    private int _previousNewRound = 0;

    private int _totalSpawnCount = 0;

    private int _remainingEnemies = 0;

    private int _currentSpawnCooldown = 0;

    [Export]
    private int _failedSpawnCooldown = 100;

    [Export]
    private float _playerSafetyRadius = 256f;

    private RoundInfo _currentRoundInfo = null;

    private RoundSpawnInfo[] _spawnStatus = null;

    private CoroutineSource _spawnRoutine = null;

    [Export]
    private Dictionary<int, RoundInfo> _rounds = null;

    [Export]
    private NodePathResource _spawnPath = null;

    [Export]
    private Node2D _spawnPointsParent = null;

    [Export]
    private ShapeCastInfo2D _spawnCheck = null;

    [Export]
    private StringName _playerBlackboardName = "";

    [Export]
    private PlayerBodyTop2D _player = null;

    [Signal]
    public delegate void NewRoundEventHandler(int round);

    [Signal]
    public delegate void RoundEndedEventHandler();

    [Signal]
    public delegate void EnemyDiedEventHandler(PackedScene enemyScene);

    public void NextRound()
    {
        _currentRound++;

        if (_rounds.ContainsKey(_currentRound))
        {
            _previousNewRound = _currentRound;
            _currentRoundInfo = _rounds[_currentRound];
        }

        int escalation = _currentRound - _previousNewRound;
        _totalSpawnCount = 0;
        _spawnStatus = _currentRoundInfo.GetSpawnInfo(escalation);

        for (int i = 0; i < _spawnStatus.Length; i++)
        {
            _totalSpawnCount += _spawnStatus[i].SpawnCount;
        }

        _remainingEnemies = _totalSpawnCount;
        _currentSpawnCooldown = _currentRoundInfo.SpawnCooldown;

        _spawnRoutine = new();
        _spawnRoutine.StartCoroutine(SpawnEnemies());

        EmitSignal(SignalName.NewRound, _currentRound);
    }

    private RoundSpawnInfo PickEnemy()
    {
        int currentIndex = _spawnStatus[0].SpawnCount;
        int spawnIndex = GD.RandRange(0, _totalSpawnCount - 1);
        RoundSpawnInfo pick = null;

        for (int i = 0; i < _spawnStatus.Length; i++)
        {
            if (spawnIndex < currentIndex)
            {
                pick = _spawnStatus[i];
                break;
            }

            currentIndex += _spawnStatus[i].SpawnCount;
        }
        
        _totalSpawnCount--;
        return pick;
    }

    private void OnEnemyDeath(PackedScene enemyScene)
    {
        _remainingEnemies--;
        EmitSignal(SignalName.EnemyDied, enemyScene);

        if (_remainingEnemies <= 0)
        {
            EmitSignal(SignalName.RoundEnded);
        }
    }

    private Node2D PickSpawn()
    {
        Array<Node> availableSpawns = _spawnPointsParent.GetChildren();
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;

        while (availableSpawns.Count > 0)
        {
            int spawnIndex = GD.RandRange(0, availableSpawns.Count - 1);
            Node2D spawnPoint = availableSpawns[spawnIndex] as Node2D;
            GD.Print($"Spawn: {spawnPoint.Name}");

            if (spawnPoint == null)
            {
                throw new InvalidCastException("Spawn point for round manager must be a node 2D.");
            }            

            if (_player.GlobalPosition.DistanceTo(spawnPoint.GlobalPosition) > _playerSafetyRadius)
            {
                PhysicsShapeQueryParameters2D query = _spawnCheck.CreateQuery(spawnPoint.GlobalTransform);

                if (ShapeCastInfo2D.IntersectShape(spaceState, query).Length == 0)
                {
                    return spawnPoint;
                }
            }

            int lastIndex = availableSpawns.Count - 1;
            availableSpawns[spawnIndex] = availableSpawns[lastIndex];
            availableSpawns[lastIndex] = spawnPoint;
            availableSpawns.RemoveAt(lastIndex);
        }

        return null;
    }

    private bool SpawnEnemy()
    {
        Node2D spawnPoint = PickSpawn();

        if (spawnPoint == null)
        {
            return false;
        }

        RoundSpawnInfo spawnInfo = PickEnemy();
        EnemyBody spawned = spawnInfo.SpawnScene.Instantiate<EnemyBody>();

        Node sceneNode = MiscUtils.GetSceneNodeOf(this);
        sceneNode.GetNode(_spawnPath.Path).AddChild(spawned);

        spawned.GlobalPosition = spawnPoint.GlobalPosition;
        spawned.GlobalRotation = spawnPoint.GlobalRotation;
        
        spawned.HealthSource.Died += (Node instigator) => OnEnemyDeath(spawnInfo.SpawnScene);
        spawned.AIController.SetBlackboardVariable(_playerBlackboardName, _player);
        
        UseableTool tool = spawnInfo.ToolScene.Instantiate<UseableTool>();
        spawned.EquipTool(tool);
        return true;
    }

    private IEnumerable SpawnEnemies()
    {
        int cooldown = _currentSpawnCooldown;

        while (_totalSpawnCount > 0)
        {
            yield return cooldown;
            cooldown = SpawnEnemy() ? _currentSpawnCooldown : _failedSpawnCooldown;
        }
    }

    public override void _ExitTree()
    {
        if (_spawnRoutine != null)
        {
            _spawnRoutine.IsCancelled = true;
        }
    }
}
