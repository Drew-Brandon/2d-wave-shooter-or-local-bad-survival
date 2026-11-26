using Godot;

[Tool]
public partial class CallMethod : BTAction
{
    [Export]
    private StringName _nodeName = "";

    [Export]
    private StringName _methodName = "";

    [Export]
    private StringName _outputVarName = "";

    public override string _GenerateName()
    {
        return $@"CallMethod(
                Node: {LimboUtility.DecorateVar(_nodeName)},
                Method: {_methodName}
                ){LimboUtility.DecorateOutputVar(_outputVarName)}";
    }

    public override Status _Tick(double delta)
    {
        Node node = Blackboard.GetVar(_nodeName).As<Node>();
        Variant output = node.Call(_methodName);

        if (Blackboard.HasVar(_outputVarName))
        {
            Blackboard.SetVar(_outputVarName, output);
        }

        return Status.Success;
    }
}
