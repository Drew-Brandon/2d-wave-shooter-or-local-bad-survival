namespace Logic;
using Godot;

[GlobalClass]
public partial class Stopwatch : Node
{
    private bool _isRunning = false;

    private float _curTime = 0f;

    [Signal]
    public delegate void OnStopwatchStartedEventHandler();

    [Signal]
    public delegate void OnStopwatchStoppedEventHandler(float curTime);

    [Signal]
    public delegate void OnStopwatchUpdatedEventHandler(float curTime);

    public void Start()
    {
        _isRunning = true;
        _curTime = 0f;
        EmitSignal(SignalName.OnStopwatchStarted);
    }

    public void Stop()
    {
        _isRunning = false;
        EmitSignal(SignalName.OnStopwatchStopped, _curTime);
    }

    public override void _Process(double delta)
    {
        if (_isRunning)
        {
            _curTime += (float)delta;
            EmitSignal(SignalName.OnStopwatchUpdated, _curTime);
        }
    }
}
