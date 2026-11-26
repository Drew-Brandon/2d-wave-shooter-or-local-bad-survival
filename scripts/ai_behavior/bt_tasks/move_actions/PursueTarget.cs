namespace AIBehavior.BTTasks;
using Godot;
using Puppets.Bodies;


[Tool]
public partial class PursueTarget : MoveAction
{
    private bool _justEntered = true;

    [Export]
    private float _targetUpdateThreshold = 8f;

    private Vector2 _prevTargetPos = Vector2.Zero;
    
    public override void _Enter()
    {
        base._Enter();
        _justEntered = true;

        Node2D target = BBUtils.GetBlackboardValue<Node2D>(Blackboard, Target);
        PuppetBodyTop2D puppet = (PuppetBodyTop2D)Agent;
        puppet.FocusOnTarget(target);
    }

    public override string _GenerateName()
    {
        return $@"PursueTarget(
                Target Update Threshold: {_targetUpdateThreshold},
                Desired Distance: {GetDesiredDistanceString()},
                Target: {GetTargetString()}
                )";
    }

    public override Status _Tick(double delta)
    {
        float desiredDist = GetDesiredDist();
        Node2D target = BBUtils.GetBlackboardValue<Node2D>(Blackboard, Target);

        if (_justEntered || _prevTargetPos.DistanceTo(target.GlobalPosition) > _targetUpdateThreshold)
        {
            MoveToTarget(desiredDist, target.GlobalPosition);
            _prevTargetPos = target.GlobalPosition;
            _justEntered = false;
        }

        return GetTargetStatus();
    }
}
