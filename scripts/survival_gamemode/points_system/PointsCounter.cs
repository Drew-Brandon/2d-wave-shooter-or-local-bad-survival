using Godot;
using Godot.Collections;

public partial class PointsCounter : Node
{
    [Export]
    private int _points = 0;

    [Export]
    private Dictionary<PackedScene, int> _enemyPoints = null;

    [Signal]
    public delegate void PointsChangedEventHandler(int points);

    public void AddPoints(int points)
    {
        _points += points;
        EmitSignal(SignalName.PointsChanged, _points);
    }

    public void AddPointsForEnemy(PackedScene enemyScene)
    {
        AddPoints(_enemyPoints[enemyScene]);
    }

    public bool ChargePoints(int points)
    {
        if (_points < points)
        {
            return false;
        }

        _points -= points;
        EmitSignal(SignalName.PointsChanged, _points);
        return true;
    }

    public override void _Ready()
    {
        EmitSignal(SignalName.PointsChanged, _points);
    }
}
