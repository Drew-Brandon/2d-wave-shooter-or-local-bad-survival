namespace AIBehavior.BTTasks;
using Godot;

[Tool]
public partial class MoveTo : MoveAction
{
    public override string _GenerateName()
    {
        return $@"MoveTo(
                Desired Distance: {GetDesiredDistanceString()},
                Target: {GetTargetString()}
                )";
    }

    public override void _Enter()
    {
        MoveToTarget();
    }

    public override Status _Tick(double delta)
    {
        return GetTargetStatus();
    }
}
