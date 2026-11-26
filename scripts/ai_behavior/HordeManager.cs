using Godot;
using Logic;

public partial class HordeManager : Node
{
    #region Properties
    private int _totalMembers { get => GetTree().GetNodeCountInGroup(_groupName); }

    private int _arrivedMembers = 0;

    [Export]
    private float _rallyRadius = 1024f;
    public float RallyRadius { get => _rallyRadius; }

    [Export]
    private float _rallyPercentThreshold = 0.8f;

    [Export]
    private int _rallyTimeLimitScale = 100;

    private Vector2 _rallyPoint = Vector2.Zero;
    public Vector2 RallyPoint { get => _rallyPoint; }

    [Export]
    private StringName _groupName = "";

    [Export]
    private Bounds2D _rallyBounds;

    private CoroutineSource _cts = new();
    #endregion

    [Signal]
    public delegate void OnNewRallyPointAssignedEventHandler();

    #region Methods
    public override void _Ready()
    {
        NewRallyPoint();
    }

    public float GetRallyRadius()
    {
        return _rallyRadius;
    }

    public Vector2 GetRallyPoint()
    {
        return _rallyPoint;
    }

    public void NewRallyPoint()
    {
        Vector2 prevRallyPoint = _rallyPoint;
        float travelDist = prevRallyPoint.DistanceTo(_rallyPoint);
        int rallyTimeLimit = Mathf.FloorToInt(travelDist * _rallyTimeLimitScale);

        _arrivedMembers = 0;
        _rallyPoint = _rallyBounds.GetRandomPoint();

        _cts = new();
        _cts.StartReoccurenceLater(rallyTimeLimit, 1, NewRallyPoint);

        EmitSignal(SignalName.OnNewRallyPointAssigned);
    }

    public void NotifyMemberArrival()
    {
        _arrivedMembers++;
        
        GD.Print(_totalMembers);
        if (_arrivedMembers >= _totalMembers * _rallyPercentThreshold)
        {
        //    NewRallyPoint();
        }
    }
    #endregion
}
