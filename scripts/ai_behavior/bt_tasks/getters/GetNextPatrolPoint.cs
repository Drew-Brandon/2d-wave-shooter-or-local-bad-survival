namespace AIBehavior.BTTasks;
using Godot;

[Tool]
public partial class GetNextPatrolPoint : BTAction
{
    #region Properties
    private int _step = 1;

    [Export]
    private StringName _isReverseName = "is_reverse";

    [Export]
    private StringName _patrolPathName = "patrol_path";

    [Export]
    private StringName _patrolPointName = "current_patrol_point";
    #endregion

    #region Methods
    public override string _GenerateName()
    {
        return $@"GetNextPatrolPoint(
                Is Reverse: {_isReverseName},
                Patrol Path: {LimboUtility.DecorateVar(_patrolPathName)},
                Patrol Point: {LimboUtility.DecorateVar(_patrolPointName)}
                ) -> {LimboUtility.DecorateOutputVar(_patrolPointName)}";
    }

    public override void _Enter()
    {
        int patrolPoint = Blackboard.GetVar(_patrolPointName).AsInt32();
        Node2D patrolPath = Blackboard.GetVar(_patrolPathName).As<Node2D>();
        
        int nextPatrolPoint = patrolPoint + _step;
        
        if (nextPatrolPoint >= patrolPath.GetChildCount() || nextPatrolPoint < 0)
        {
            if (Blackboard.GetVar(_isReverseName).AsBool())
            {
                nextPatrolPoint = patrolPoint - _step;
                _step = -_step;
            }
            else
            {
                nextPatrolPoint = Mathf.PosMod(nextPatrolPoint, patrolPath.GetChildCount());
            }
        }

        Blackboard.SetVar(_patrolPointName, nextPatrolPoint);
    }

    public override Status _Tick(double delta)
    {
        return Status.Success;
    }
    #endregion
}
