using Godot;
using Puppets.Bodies;
using System;

public partial class GetToolRange : BTAction
{
    [Export]
    private BBVariant _toolRange = null;

    public override void _Enter()
    {
        AIBodyTop2D body = (AIBodyTop2D)Agent;

        if (body.CurrentTool == null)
        {
            throw new NullReferenceException("Attempted to get tool's range from AI body, yet no tool was equipped.");
        }

        Blackboard.SetVar(_toolRange.Variable, body.CurrentTool.Range);
    }

    public override Status _Tick(double delta)
    {
        return Status.Success;
    }

    public override string _GenerateName()
    {
        return $"GetToolRange(Tool Range: {_toolRange})";
    }
}
