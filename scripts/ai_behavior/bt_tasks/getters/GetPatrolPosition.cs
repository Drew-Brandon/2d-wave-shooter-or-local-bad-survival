namespace AIBehavior.BTTasks;
using Godot;

[Tool]
public partial class GetPatrolPosition : BTAction
{
    #region Properties
    [Export]
    private StringName _patrolPathName = $"patrol_path";

    [Export]
    private StringName _patrolPointName = $"current_patrol_point";

    [Export]
    private StringName _targPosName = $"target_position";
    #endregion

    #region Methods
    public override string _GenerateName()
    {
        return $@"GetPatrolPosition(
                Patrol Path: {LimboUtility.DecorateVar(_patrolPathName)},
                Patrol Point: {LimboUtility.DecorateVar(_patrolPointName)}
                ) -> {LimboUtility.DecorateOutputVar(_targPosName)}";
    }

    public override void _Enter()
    {
        int patrolPoint = Blackboard.GetVar(_patrolPointName).AsInt32();
        Node2D patrolPath = Blackboard.GetVar(_patrolPathName).As<Node2D>();
        
        Vector2 patrolPointPos = patrolPath.GetChild<Node2D>(patrolPoint).GlobalPosition;
        Blackboard.SetVar(_targPosName, patrolPointPos);
    }

    public override Status _Tick(double delta)
    {
        return Status.Success;
    }
    #endregion
}
