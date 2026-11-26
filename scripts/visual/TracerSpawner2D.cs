using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class TracerSpawner2D : Spawner2D
{
	[Export]
	private int _tracerLifetime = 100;

	private List<Line2D> _toFree = new();

	private void FreeTracers()
	{
		if (_toFree.Count == 0)
		{
			return;
		}

		for (int i = 0; i < _toFree.Count; i++)
		{
			_toFree[i].QueueFree();
		}

		_toFree.Clear();
	}

	public async void SpawnTracer(Vector2 targ)
	{
		Line2D tracer = SpawnAt<Line2D>(Vector2.Zero, 0f);

		if (tracer.GetPointCount() != 2)
		{
			throw new InvalidOperationException("Tracer tracer should only have two points.");
		}

		tracer.SetPointPosition(0, GlobalPosition);
		tracer.SetPointPosition(1, targ);
		await Task.Delay(_tracerLifetime);

		if (IsInstanceValid(tracer))
		{
			_toFree.Add(tracer);
		}
	}

    public override void _Ready()
    {
        GetTree().ProcessFrame += FreeTracers;
    }
}
