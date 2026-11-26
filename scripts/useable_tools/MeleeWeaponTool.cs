using Godot;
using System;

public partial class MeleeWeaponTool : UseableTool
{
    [Export(PropertyHint.Range, "0,1,0.01")]
    private float _defaultPower = 0.5f;

    [Signal]
    public delegate void PowerfullyUsedEventHandler(float power, Vector2 usePosition);

    [Signal]
    public delegate void PowerfullyFailedUseEventHandler(float power, Vector2 usePosition);

    [Signal]
    public delegate void PowerfullySuccessfulUsedEventHandler(float power, Vector2 usePosition);

    protected override void OnUse(Vector2 usePosition)
    {
        base.OnUse(usePosition);
        EmitSignal(SignalName.PowerfullyUsed, _defaultPower, usePosition);
    }

    protected override void OnFailedUse(Vector2 usePosition)
    {
        base.OnFailedUse(usePosition);
        EmitSignal(SignalName.PowerfullyFailedUse, _defaultPower, usePosition);
    }

    protected override void OnSuccessfullyUsed(Vector2 usePosition)
    {
        base.OnSuccessfullyUsed(usePosition);
        EmitSignal(SignalName.PowerfullySuccessfulUsed, _defaultPower, usePosition);
    }

    protected virtual void OnPowerfullyUse(float power, Vector2 usePosition)
    {
        EmitSignal(SignalName.PowerfullyUsed, power, usePosition);
    }

    protected virtual void OnPowerfullyFailedUse(float power, Vector2 usePosition)
    {
        EmitSignal(SignalName.PowerfullyFailedUse, power, usePosition);
    }

    protected virtual void OnPowerfullySuccessfulUsed(float power, Vector2 usePosition)
    {
        EmitSignal(SignalName.PowerfullySuccessfulUsed, power, usePosition);
    }

    public bool PowerfullyUse(float power, Vector2 usePosition)
    {
        if (AttemptUse())
        {
            base.OnUse(usePosition);
            base.OnSuccessfullyUsed(usePosition);
            OnPowerfullyUse(power, usePosition);
            OnPowerfullySuccessfulUsed(power, usePosition);
            return true;
        }

        base.OnUse(usePosition);
        base.OnFailedUse(usePosition);
        OnPowerfullyUse(power, usePosition);
        OnPowerfullyFailedUse(power, usePosition);
        return false;
    }
}
