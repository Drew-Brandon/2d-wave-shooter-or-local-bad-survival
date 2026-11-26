namespace AIBehavior.BTTasks;
using Godot;
using Puppets.Bodies;
using System;

[Tool]
public partial class ShouldUseToolOn : BTAction
{
    private Status _result = Status.Fresh;

    [Export]
    private BBVariant _target = null;

    public override string _GenerateName()
    {
        return $"ShouldUseToolOn(Target: {BBUtils.GetBlackboardValueString(Blackboard, _target)})";
    }

    public override void _Enter()
    {
        AIBodyTop2D aiBody = (AIBodyTop2D)Agent;
        Vector2 usePosition;
        CollisionObject2D targetCollision = null;
        Variant targetValue = BBUtils.GetBlackboardValue(Blackboard, _target);

        switch (targetValue.VariantType)
        {
            case Variant.Type.Vector2:
                usePosition = targetValue.AsVector2();
                break;

            case Variant.Type.Object:
                targetCollision = targetValue.AsGodotObject() as CollisionObject2D;

                if (targetCollision != null)
                {
                    usePosition = targetCollision.GlobalPosition;
                    break;
                }
                
                goto default;
            
            default:
                throw new InvalidOperationException("Target wasn't a vector or collision object.");
        }

        _result = aiBody.CurrentTool.ShouldUse(usePosition, targetCollision) ? Status.Success : Status.Failure;
    }

    public override Status _Tick(double delta)
    {
        return _result;
    }
}
