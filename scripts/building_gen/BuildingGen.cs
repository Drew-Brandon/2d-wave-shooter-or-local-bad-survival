using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class BuildingGen : Node2D, ISerializationListener
{
	private struct StepData
	{
		private int _index;
		public int Index { get => _index; }

		private int _count;
		public int Count { get => _count; }

		private ulong _state;
		public ulong State { get => _state; }

		private Branch _branch;
		public Branch Branch { get => _branch; }

		private Vector2I[,] _cells;

		public StepData(int index, int count, ulong state, Branch branch, Vector2I size)
		{
			_index = index;
			_count = count;
			_state = state;
			_branch = branch;
			_cells = new Vector2I[size.X, size.Y];
		}

		public Vector2I GetSize()
		{
			return new Vector2I(_cells.GetLength(0), _cells.GetLength(1));
		}

		public Vector2I GetCell(Vector2I coords)
		{
			return _cells[coords.X, coords.Y];
		}

		public void SetCell(Vector2I coords, Vector2I cell)
		{
			_cells[coords.X, coords.Y] = cell;
		}
	}

	private struct Branch
	{
		private Vector2I _coords;
		public Vector2I Coords { get => _coords; }

		private Vector2I _dir;
		public Vector2I Dir { get => _dir; }

		public Branch(Vector2I coords, Vector2I dir)
		{
			_coords = coords;
			_dir = dir;
		}
	}

	[Export]
	private int _srcId = 0;

	private Gizmos2DSource _gizmos = new();

	private Stack<StepData> _history = new();

	[Export]
	private uint _seed = 0;

	private RandomNumberGenerator _rng = new();

	[Export]
	private Vector2I _floorCoords = Vector2I.Zero;

	[Export]
	private Vector2I _wallCoords = Vector2I.Zero;

	[Export]
	private Vector2I _plotSize = new Vector2I(20, 20);

	private List<Branch> _branches = new();

	[Export]
	private TileMapLayerPlus _geoLayer = null;

	[ExportToolButton("Randomize Seed")]
	private Callable _randSeed => Callable.From(() => _seed = GD.Randi());

	[ExportToolButton("Restart Generation")]
	private Callable _restartGen => Callable.From(RestartGen);

	[ExportToolButton("Step Forward")]
	private Callable _stepForward => Callable.From(GenNextRoom);

	[ExportToolButton("Step Back")]
	private Callable _stepBack => Callable.From(RemovePrevRoom);

	private void AddBranchGizmos()
	{
		Color originColor = new(1f, 0f, 0f, 0.5f);
		Color arrowColor = new(0f, 1f, 0f, 0.5f);
		Rid canvasItem = GetCanvasItem();

		for (int i = 0; i < _branches.Count; i++)
		{
			Vector2I pos = _branches[i].Coords * _geoLayer.TileSet.TileSize;
			Vector2I dir = _branches[i].Dir * _geoLayer.TileSet.TileSize;
			_gizmos.AddCircle(pos, 4f, originColor, canvasItem);
			_gizmos.AddLine(pos, pos + dir, arrowColor, canvasItem, 4f);
		}
	}

	private void AddBranch(Vector2I coords, Vector2I dir)
	{
		if (_geoLayer.GetCellTileData(coords + dir.Clamp(-1, 0)) != null)
		{
			return;
		}

		Branch branch = new(coords, dir);
		_branches.Add(branch);
	}

	private void GenRoom(Vector2I start, Vector2I stop, Vector2I outlineCoords, Vector2I insideCoords)
	{
		_gizmos.Clear();
		int horzSteps = Mathf.Abs(stop.X - start.X) + 1;
		int vertSteps = Mathf.Abs(stop.Y - start.Y) + 1;
		Vector2I dirX = new(Mathf.Clamp(stop.X - start.X, -1, 1), 0);
		Vector2I dirY = new(0, Mathf.Clamp(stop.X - start.X, -1, 1));
		Vector2I botLeft = new(start.X, stop.Y);
		Vector2I topRight = new(stop.X, start.Y);

		Trace2D.TraceLine(start, dirX, horzSteps, (Vector2I coords) => AddBranch(coords, -dirY));
		Trace2D.TraceLine(start, dirY, vertSteps, (Vector2I coords) => AddBranch(coords, -dirX));
		Trace2D.TraceLine(topRight, dirY, vertSteps, (Vector2I coords) => AddBranch(coords, dirX));
		Trace2D.TraceLine(botLeft, dirX, horzSteps, (Vector2I coords) => AddBranch(coords, dirY));

		Trace2D.TraceHollowRectCells(start, stop, (Vector2I coords) =>
		{
			
		});

		_geoLayer.SetCellRect(start + dirX + dirY, stop - dirX - dirY, _srcId, insideCoords);
		
		AddBranchGizmos();
	}

	private void ClearPlot()
	{
		_gizmos.Clear();
		_branches.Clear();
		_geoLayer.Clear();

		_history.Clear();
		_rng.Randomize();
	}

	private void GenNextRoom()
	{
		List<Branch> availableBranches = new(_branches);

		while (true)
		{
			ulong state = _rng.State;
			int index = _rng.RandiRange(0, availableBranches.Count - 1);
			Branch branch = availableBranches[index];
			availableBranches.RemoveAt(index);

			Vector2I size = new(_rng.RandiRange(4, 10), _rng.RandiRange(4, 10));
			Vector2I start = branch.Coords;
			Vector2I stop = start + size * (branch.Dir + new Vector2I(branch.Dir.Y, branch.Dir.X));
			Vector2I offset = (stop - start).Clamp(-1, 0);

			StepData data = new(index, _branches.Count, state, branch, size);
			Trace2D.TraceRect(start + offset, stop + offset, (Vector2I coords) =>
			{
				Vector2I relCoords = (coords - (start + offset)).Abs();
				Vector2I atlasCoords = _geoLayer.GetCellAtlasCoords(coords);
				data.SetCell(relCoords, atlasCoords);
			});
			_history.Push(data);

			_branches = availableBranches;
			GenRoom(start, stop, _wallCoords, _floorCoords);

			break;
		}
	}

	private void RemovePrevRoom()
	{
		if (_history.Count == 0)
		{
			return;
		}

		StepData data = _history.Pop();
		Branch branch = data.Branch;
		_branches.Insert(data.Index, branch);
		_rng.State = data.State;
		_branches.RemoveRange(data.Count, _branches.Count - data.Count);

		Vector2I size = data.GetSize();
		Vector2I start = branch.Coords;
		Vector2I stop = branch.Coords + size * (branch.Dir + new Vector2I(branch.Dir.Y, branch.Dir.X));
		Vector2I offset = (stop - start).Clamp(-1, 0);

		Trace2D.TraceRect(start + offset, stop + offset, (Vector2I coords) =>
		{
			Vector2I relCoords = (coords - (start + offset)).Abs();
			_geoLayer.SetCell(coords, _srcId, data.GetCell(relCoords));
		});

		_gizmos.Clear();
		AddBranchGizmos();
	}

	private void GenBuilding(Vector2I plotStart, Vector2I plotStop, Vector2I buildingStart)
	{
		
	}

	private void RestartGen()
	{
		ClearPlot();
		_rng.Seed = (ulong)_seed;
		Vector2I size = new(_rng.RandiRange(4, 10), _rng.RandiRange(4, 10));
		GenRoom(Vector2I.Zero, size, _wallCoords, _floorCoords);
	}

	public void OnBeforeSerialize()
	{ }

	public void OnAfterDeserialize()
	{
		ClearPlot();
	}
}
