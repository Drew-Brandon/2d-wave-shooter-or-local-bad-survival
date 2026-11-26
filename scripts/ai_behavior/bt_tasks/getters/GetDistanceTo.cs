namespace AIBehavior.BTTasks;
using Godot;

[Tool]
public partial class GetDistanceTo : BTAction
{
    [Export]
    private BBVariant _targPos = new();

    [Export]
    private StringName _targDistName = "target_distance";

    #region Methods
    public override string _GenerateName()
    {
        return $@"GetDistanceTo(
                Target Position: {BBUtils.GetBlackboardValueString(Blackboard, _targPos)}
                ){LimboUtility.DecorateOutputVar(_targDistName)}";
    }

    public override void _Enter()
    {
        Vector2 targPos = BBUtils.GetBlackboardPosition(Blackboard, _targPos);
        Node2D agent2D = (Node2D)Agent;
        float distTo = agent2D.GlobalPosition.DistanceTo(targPos);
        Blackboard.SetVar(_targDistName, distTo);
    }

    public override Status _Tick(double delta)
    {
        return Status.Success;
    }
    #endregion
}
