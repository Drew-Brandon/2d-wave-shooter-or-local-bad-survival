using CastInfo2D;
using Godot;

public partial class FOVRaycasting : Node2D
{
	private float _fovAngleRad = Mathf.Pi / 3f;

	[Export]
	private float _fovAngleDeg { get => Mathf.RadToDeg(_fovAngleRad); set => _fovAngleRad = Mathf.DegToRad(value); }

	private float _angleStepRad = Mathf.Pi / 16f;

	[Export]
	private float _angleStepDeg { get => Mathf.RadToDeg(_angleStepRad); set => _angleStepRad = Mathf.DegToRad(value); }

	[Export]
	private float _outFovRange = 64f;
	
	[Export]
	private float _inFovRange = 512f;

	[Export]
	private RayCastInfo2D _fovRay = null;

	[Signal]
	public delegate void OnFOVVertCountUpdatedEventHandler(int rayCount);

	[Signal]
	public delegate void OnFOVUpdatedLocalEventHandler(Vector2[] verts);

	[Signal]
	public delegate void OnFOVUpdatedGlobalEventHandler(Vector2[] verts);

	private Vector2 CastFOVRay(float angle)
	{
		float range = Mathf.Abs(Mathf.AngleDifference(angle, GlobalRotation)) <= _fovAngleRad ? _inFovRange : _outFovRange;

		Vector2 start = GlobalPosition;
		Vector2 end = start + Vector2.FromAngle(angle) * range;

		PhysicsRayQueryParameters2D query = _fovRay.CreateQuery(start, end);
		RayCastResults2D rayResults = RayCastInfo2D.IntersectRay(GetWorld2D().DirectSpaceState, query);
		return rayResults.Position;
	}

	private void UpdateFOV()
	{
		int vertCount = Mathf.FloorToInt(Mathf.Tau / _angleStepRad);
		Vector2[] localVerts = new Vector2[vertCount];
		Vector2[] globalVerts = new Vector2[vertCount];

		for (int i = 0; i < vertCount; i++)
		{
			float angle = i * _angleStepRad;
			globalVerts[i] = CastFOVRay(angle);
			localVerts[i] = ToLocal(globalVerts[i]);
		}

		EmitSignal(SignalName.OnFOVVertCountUpdated, vertCount);
		EmitSignal(SignalName.OnFOVUpdatedLocal, localVerts);
		EmitSignal(SignalName.OnFOVUpdatedGlobal, globalVerts);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		UpdateFOV();
	}
}
