using Godot;
using System;

public partial class PointsCharger : Node
{
    [Export]
    private int _chargeAmount = 1000;

    [Export]
    private PointsCounter _pointsCounter = null;

    [Signal]
    public delegate void ChargeSucceededEventHandler();

    [Signal]
    public delegate void ChargeFailedEventHandler();

    public void ChargePoints()
    {
        if (_pointsCounter.ChargePoints(_chargeAmount))
        {
            EmitSignal(SignalName.ChargeSucceeded);
        }
        else
        {
            EmitSignal(SignalName.ChargeFailed);
        }
    }
}
