namespace AIBehavior.BTTasks;
using Godot;
using Puppets.Bodies;
using Puppets.Controllers;


[Tool]
public abstract partial class MoveAction : BTAction
{
    private bool _isActive = false;

    private float _prevDesiredDistance = 0f;

    [Export]
    private BBVariant _desiredDistance = new();
    public BBVariant DesiredDistance { get => _desiredDistance; }

    [Export]
    private BBVariant _target = new();
    public BBVariant Target { get => _target; }

    protected Status GetTargetStatus()
    {
        PuppetBodyTop2D puppet = (PuppetBodyTop2D)Agent;
        AIControllerTop2D aiController = puppet.GetController<AIControllerTop2D>();

        if (aiController.NavigationAgent.IsTargetReached() || !_isActive)
        {
            return Status.Success;
        }
        else if (!aiController.NavigationAgent.IsTargetReachable())
        {
            return Status.Failure;
        }
        else
        {
            return Status.Running;
        }
    }

    protected float GetDesiredDist()
    {
        if (!BBUtils.BlackboardValueIsValid(Blackboard, _desiredDistance))
        {
            PuppetBodyTop2D puppet = (PuppetBodyTop2D)Agent;
            AIControllerTop2D aiController = puppet.GetController<AIControllerTop2D>();
            return aiController.NavigationAgent.TargetDesiredDistance;
        }

        return BBUtils.GetBlackboardValue<float>(Blackboard, _desiredDistance);
    }

    protected Vector2 GetTarget()
    {
        return BBUtils.GetBlackboardPosition(Blackboard, _target);
    }

    protected void MoveToTarget()
    {
        MoveToTarget(GetDesiredDist(), GetTarget());
    }

    protected void MoveToTarget(float desiredDist, Vector2 targ)
    {
        PuppetBodyTop2D puppet = (PuppetBodyTop2D)Agent;
        AIControllerTop2D aiController = puppet.GetController<AIControllerTop2D>();

        aiController.NavigationAgent.TargetDesiredDistance = desiredDist;
        aiController.NavigationAgent.TargetPosition = targ;
    }

    protected string GetDesiredDistanceString()
    {
        return BBUtils.GetBlackboardValueString(Blackboard, _desiredDistance);
    }

    protected string GetTargetString()
    {
        return BBUtils.GetBlackboardValueString(Blackboard, _target);
    }

    public override void _Enter()
    {
        _isActive = true;
        
        PuppetBodyTop2D puppet = (PuppetBodyTop2D)Agent;
        AIControllerTop2D aiController = puppet.GetController<AIControllerTop2D>();
        NavigationAgent2D navigationAgent = aiController.NavigationAgent;

        _prevDesiredDistance = navigationAgent.TargetDesiredDistance;
        navigationAgent.TargetDesiredDistance = GetDesiredDist();
    }

    public override void _Exit()
    {
        _isActive = false;

        PuppetBodyTop2D puppet = (PuppetBodyTop2D)Agent;
        AIControllerTop2D aiController = puppet.GetController<AIControllerTop2D>();
        NavigationAgent2D navigationAgent = aiController.NavigationAgent;
        
        navigationAgent.TargetDesiredDistance = _prevDesiredDistance;
    }
}
