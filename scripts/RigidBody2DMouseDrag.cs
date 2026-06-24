using Godot;
using Godot.Collections;
using Inventory;

public partial class RigidBody2DMouseDrag : AnimatableBody2D
{
	[Export]
	private bool _debug = false;

	private Gizmos2DSource _gizmos = new();

	[Export(PropertyHint.Layers2DPhysics)]
	private uint _layer = 0;

	[Export]
	private float _angularVelScale = 10f;

	[Export]
	private float _forceScale = 10f;

	[Export]
	private float _maxForce = 64f;

	[Export]
	private float _maxLinearVel = 1024f;

	private Vector2 _grabbedPos = Vector2.Zero;

	private RigidBody2D _grabbedBody = null;

	[Export]
	private float _maxSpeed = 64f;

	[Export]
	private float _speedScale = 2048f;

	[Export]
	private PinJoint2D _grabPoint = null;

	private void Grab()
	{
		GlobalPosition = GetGlobalMousePosition();
		PhysicsDirectSpaceState2D state = GetWorld2D().DirectSpaceState;
		PhysicsPointQueryParameters2D query = new()
		{
			Position = GlobalPosition,
			CollisionMask = _layer,
			CanvasInstanceId = GetCanvasLayerNode().GetInstanceId()
		};
		
		Array<Dictionary> results = state.IntersectPoint(query);

		for (int i = 0; i < results.Count; i++)
		{
			RigidBody2D body = results[i]["collider"].AsGodotObject() as RigidBody2D;

			if (body != null)
			{
				_grabbedPos = body.ToLocal(GlobalPosition);
				_grabPoint.GlobalPosition = GlobalPosition;
				_grabPoint.NodeA = GetPath();
				_grabPoint.NodeB = body.GetPath();
				_grabbedBody = body;
			}
		}
	}

	private void Drop()
	{
		_grabPoint.NodeA = null;
		_grabPoint.NodeB = null;
		_grabbedBody = null;
	}

	public override void _Ready()
	{
		Hidden += Drop;
	}

	public override void _Process(double delta)
	{
		_gizmos.Clear();

		if (_grabbedBody != null)
		{
			Vector2 mousePos = GetGlobalMousePosition();
			float dist = GlobalPosition.DistanceTo(mousePos);
			Vector2 vel = ((mousePos - GlobalPosition) * _speedScale).LimitLength(_maxSpeed);
			_gizmos.AddCircle(Vector2.Zero, 16f, new Color(1f, 0f, 0f), GetCanvasItem());
			MoveAndCollide((vel * (float)delta).LimitLength(dist));
		}

		if (Input.IsActionJustPressed("drag") && _grabbedBody == null)
		{
			Grab();
		}
		else if (Input.IsActionJustReleased("drag") && _grabbedBody != null)
		{
			Drop();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		/*if (_grabbedBody != null)
		{
			float rotInput = Input.GetAxis("rotate_left", "rotate_right");
			Vector2 globalGrabPos = _grabbedBody.ToGlobal(_grabbedPos);
			Vector2 disp = (GlobalPosition - globalGrabPos);
			Vector2 force = (disp * _forceScale).LimitLength(_maxForce);
			_gizmos.AddLine(Vector2.Zero, force, new Color(1f, 0f, 0f), GetCanvasItem());
			_grabbedBody.ApplyForce(force, globalGrabPos - _grabbedBody.GlobalPosition);
			_grabbedBody.LinearVelocity = _grabbedBody.LinearVelocity.LimitLength(_maxLinearVel);

			if (!MiscUtils.ApproxEqual(rotInput, 0f, 0.1f))
			{
				_grabbedBody.AngularVelocity = _angularVelScale * rotInput;
			}
		}*/
	}
}
