using Godot;
using System;

public partial class DayNightCycle : Node
{
    private float _curShadowLength = 0f;

    [Export]
    private float _startShadowLength = 0f;

    [Export]
    private float _maxShadowLength = 128f;

    [Export]
    private float _speed = 10f;

    [Export]
    private StringName _maxDistName = "shader_parameter/max_dist";

    [Export]
    private CanvasItem _cycleDisplay = null;

    public override void _Ready()
    {
        _curShadowLength = _startShadowLength;
    }

    public override void _Process(double delta)
    {
        float deltaF = (float)delta;
        _curShadowLength += _speed * deltaF;
        _cycleDisplay.Material.Set(_maxDistName, _curShadowLength);
    }
}
