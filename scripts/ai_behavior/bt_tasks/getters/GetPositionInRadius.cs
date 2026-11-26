namespace AIBehavior.BTTasks;
using Godot;

[Tool]
public partial class GetPositionInRadius : BTAction
{
    #region Properties
    [Export]
    private float _minRadius = 256f;

    [Export]
    private float _maxRadius = 512f;

    [Export]
    private StringName _targPosName = $"target_position";
    #endregion

    #region Methods
    public override string _GenerateName()
    {
        return $@"GetPositionInRadius(
                Radius: [{_minRadius}:{_maxRadius}]
                ) -> {LimboUtility.DecorateOutputVar(_targPosName)}";
    }

    public override void _Enter()
    {
        float radius = RandUtils.RandRangef(_minRadius, _maxRadius);
        Vector2 dir = RandUtils.RandDir(0f, Mathf.Pi * 2f);
        Node2D agent2D = (Node2D)Agent;
        Blackboard.SetVar(_targPosName, agent2D.GlobalPosition + dir * radius);
    }

    public override Status _Tick(double delta)
    {
        return Status.Success;
    }
    #endregion
}
