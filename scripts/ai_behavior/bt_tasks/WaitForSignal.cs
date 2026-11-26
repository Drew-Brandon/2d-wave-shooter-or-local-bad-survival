using Godot;

[Tool]
public partial class WaitForSignal : BTAction
{
    #region Properties
    [Export]
    private bool _failOnSignal = false;

    private Status _curStatus = Status.Fresh;

    private Callable _signalCallback = new();

    [Export]
    private StringName _nodeName = new();

    [Export]
    private StringName _signalName = new();
    #endregion

    #region Methods
    public override string _GenerateName()
    {
        return $@"WaitForSignal(
                Node: {LimboUtility.DecorateVar(_nodeName)},
                Signal: {_signalName}
                )";
    }

    public override void _Enter()
    {
        _curStatus = Status.Running;
        _signalCallback = Callable.From(() => _failOnSignal ? Status.Failure : Status.Success);
        Node node = Blackboard.GetVar(_nodeName).As<Node>();
        node.Connect(_signalName, _signalCallback);
    }

    public override void _Exit()
    {
        _curStatus = Status.Fresh;
        Node node = Blackboard.GetVar(_nodeName).As<Node>();
        node.Disconnect(_signalName, _signalCallback);
    }

    public override Status _Tick(double delta)
    {
        return _curStatus;
    }
    #endregion
}
