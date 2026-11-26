using System;
using Godot;
using Logic;

[GlobalClass]
public partial class AimInterfaceButton : Area2DButton
{
    [Export]
    private StringNameResource _targetPart = null;

    public StringName TargetPart { get => _targetPart.Name; }

    public override void _Ready()
    {
        base._Ready();

        if (_targetPart == null)
        {
            throw new NullReferenceException("Target part cannot be null.");
        }
    }
}
