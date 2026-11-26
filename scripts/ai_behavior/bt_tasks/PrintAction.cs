namespace AIBehavior.BTTasks;
using Godot;

[Tool]
public partial class PrintAction : BTAction
{
    [Export]
    private BBVariant _output = new();

    #region Methods
    public override string _GenerateName()
    {
        if (_output.ValueSource == BBParam.ValueSourceEnum.BlackboardVar)
        {
            return $"PrintAction(Output: {LimboUtility.DecorateVar(_output.Variable)})";
        }

        return $"PrintAction(Output: {_output.SavedValue})";
    }

    public override void _Enter()
    {
        if (_output.ValueSource == BBParam.ValueSourceEnum.BlackboardVar)
        {
            GD.Print(Blackboard.GetVar(_output.Variable));
        }
        else
        {
            GD.Print(_output.SavedValue);
        }
    }


    public override Status _Tick(double delta)
    {
        return Status.Success;
    }
    #endregion
}
