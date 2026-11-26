namespace AIBehavior.BTTasks;
using System.Collections;
using Godot;
using Puppets.Bodies;

[Tool]
public partial class LookAround : BTAction
{
    #region Properties
    [Export]
    private int _lookCount = 3;

    [Export]
    private int _lookCooldown = 500;

    private float _minTurn = Mathf.DegToRad(10f);

    [Export]
    private float MinTurn { get => Mathf.RadToDeg(_minTurn); set => _minTurn = Mathf.DegToRad(value); }

    private Status _curStatus = Status.Fresh;

    private CoroutineSource _lookCTS = new();
    #endregion

    #region Methods
    private IEnumerable LookRoutine()
    {
        AIBodyTop2D aiBody2D = (AIBodyTop2D)Agent;

        for (int i = 0; i < _lookCount; i++)
        {
            yield return _lookCooldown;
            aiBody2D.TargetRotation += RandUtils.RandRangef(_minTurn, Mathf.Tau - _minTurn);
        }

        yield return _lookCooldown;
        _curStatus = Status.Success;
    }

    public override string _GenerateName()
    {
        return $@"LookAround(
                Look Count: {_lookCount},
                Look Cooldown: {_lookCooldown},
                Min Turn: {Mathf.RadToDeg(_minTurn)}
                )";
    }

    public override void _Enter()
    {
        _curStatus = Status.Running;
        _lookCTS.StartCoroutine(LookRoutine());
    }

    public override Status _Tick(double delta)
    {
        return _curStatus;
    }
    #endregion
}
