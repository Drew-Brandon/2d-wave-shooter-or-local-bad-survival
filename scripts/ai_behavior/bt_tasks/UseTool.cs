namespace AIBehavior.BTTasks;
using Godot;
using Puppets.Bodies;

[Tool]
public partial class UseTool : BTAction
{
    [Export]
    private BBVariant _targPosName = new();
    
    public override string _GenerateName()
    {
        return $"UseTool(Target Position: {BBUtils.GetBlackboardValueString(Blackboard, _targPosName)})";
    }

    public override Status _Tick(double delta)
    {
        Vector2 pos = BBUtils.GetBlackboardPosition(Blackboard, _targPosName);
        PuppetBodyTop2D agentBody = (PuppetBodyTop2D)Agent;

        if (agentBody.UseTool(pos))
        {
            return Status.Success;
        }
        else
        {
            return Status.Failure;
        }
    }
}
