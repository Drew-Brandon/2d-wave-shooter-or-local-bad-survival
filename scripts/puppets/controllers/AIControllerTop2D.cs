namespace Puppets.Controllers;
using Godot;
using Puppets.Bodies;


[GlobalClass]
public partial class AIControllerTop2D : PuppetControllerTop2D
{
    [Export]
    private StringName _safetyMultiplier = "";

    [Export]
    private StringNameResource _toolRange = null;

    [Export]
	private NavigationAgent2D _navigationAgent = null;

    [Export]
    private BTPlayer _btPlayer = null;

	public NavigationAgent2D NavigationAgent { get => _navigationAgent; }

    public AIBodyTop2D AIBody { get => (AIBodyTop2D)Body; }

    private void OnSafeVelocityComputed(Vector2 safeVelocity)
    {
        Body.CurrentDirection = safeVelocity.Normalized();
        Body.ApplySpeedMultiplier(_safetyMultiplier, safeVelocity.Length() / Body.BaseSpeed);
        Body.TargetRotation = Body.CurrentDirection.Angle();
    }

    private void HandleNavigation()
    {
        if (_navigationAgent.IsNavigationFinished())
        {
            Body.CurrentDirection = Vector2.Zero;
        }
        else
        {
            Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
            Vector2 pathDirection = (nextPathPosition - Body.GlobalPosition).Normalized();
            _navigationAgent.Velocity = pathDirection * Body.BaseSpeed;
        }
    }

    protected override void OnControllerProcess(double delta)
    {
        
    }

    protected override void OnControllerPhysicsProcess(double delta)
    {
        HandleNavigation();
    }

    public void SetBlackboardVariable(StringName name, Variant value)
    {
        _btPlayer.Blackboard.SetVar(name, value);
    }

    public override void _Ready()
    {
        base._Ready();
        _navigationAgent.VelocityComputed += OnSafeVelocityComputed;
    }

    public override void _ExitTree()
    {
        _navigationAgent.VelocityComputed -= OnSafeVelocityComputed;
    }
}
