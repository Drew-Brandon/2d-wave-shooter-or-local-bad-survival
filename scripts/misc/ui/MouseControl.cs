using Godot;

public partial class MouseControl : Control
{
    public override void _Process(double delta)
    {
        GlobalPosition = GetWindow().GetMousePosition() - Size * PivotOffsetRatio;
    }
}
