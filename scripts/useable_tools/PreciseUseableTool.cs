using Godot;
using Inventory.ItemHandlers;

public partial class PreciseUseableTool : UseableTool
{
    [Signal]
    public delegate void PreciselyUsedEventHandler(StringName usePart, Vector2 usePosition, Node2D user);

    [Signal]
    public delegate void PreciselyFailedUseEventHandler(StringName usePart, Vector2 usePosition, Node2D user);

    [Signal]
    public delegate void PreciselySuccessfulUseEventHandler(StringName usePart, Vector2 usePosition, Node2D user);

    protected override void OnUse(Vector2 usePosition)
    {
        base.OnUse(usePosition);
        EmitSignal(SignalName.PreciselyUsed, "", usePosition, User);
    }

    protected override void OnFailedUse(Vector2 usePosition)
    {
        base.OnFailedUse(usePosition);
        EmitSignal(SignalName.PreciselyFailedUse, "", usePosition, User);
    }

    protected override void OnSuccessfullyUsed(Vector2 usePosition)
    {
        base.OnSuccessfullyUsed(usePosition);
        EmitSignal(SignalName.PreciselySuccessfulUse, "", usePosition, User);
    }

    protected virtual void OnPreciselyUsed(Vector2 usePosition, StringName usePart)
    {
        EmitSignal(SignalName.PreciselyUsed, usePart, usePosition, User);
    }

    protected virtual void OnPreciselyFailedUse(Vector2 usePosition, StringName usePart)
    {
        EmitSignal(SignalName.PreciselyFailedUse, usePart, usePosition, User);
    }

    protected virtual void OnPreciselySuccessfulUsed(Vector2 usePosition, StringName usePart)
    {
        EmitSignal(SignalName.PreciselySuccessfulUse, usePart, usePosition, User);
    }

    public bool PreciselyUse(Vector2 usePosition, StringName usePart)
    {
        if (AttemptUse())
        {
            base.OnUse(usePosition);
            base.OnSuccessfullyUsed(usePosition);
            OnPreciselyUsed(usePosition, usePart);
            OnPreciselySuccessfulUsed(usePosition, usePart);
            return true;
        }

        base.OnUse(usePosition);
        base.OnFailedUse(usePosition);
        OnPreciselyUsed(usePosition, usePart);
        OnPreciselyFailedUse(usePosition, usePart);
        return false;
    }
}
