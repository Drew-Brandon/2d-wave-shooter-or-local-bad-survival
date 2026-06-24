using Godot;
using System;
using System.Collections.Generic;

public partial class MouseSelector : Node2D
{
	private bool _isDragging = false;

	[Export(PropertyHint.Layers2DPhysics)]
	private uint _selectLayer = 0;

	private Vector2 _selectStart = Vector2.Zero;

	[Export]
	private Color _selectRectColor = new(1f, 0f, 0f, 0.5f);

	[Export]
	private Color _selectedNodeColor = new(0f, 1f, 0f, 1f);

	private Gizmos2DSource _gizmos = new();

	private HashSet<Node2D> _selected = new();
	public int SelectedCount { get => _selected.Count; }

	private Godot.Collections.Array<Godot.Collections.Dictionary> SelectionCast()
	{
		RectangleShape2D selectRect = new()
		{
			Size = Scale.Abs()
		};

		PhysicsShapeQueryParameters2D query = new()
		{
			Transform = new Transform2D(0f, GlobalPosition + Scale / 2f),
			CollisionMask = _selectLayer,
			Shape = selectRect
		};

		PhysicsDirectSpaceState2D state = GetWorld2D().DirectSpaceState;
		return state.IntersectShape(query);
	}

	private void SelectNew()
	{
		Godot.Collections.Array<Godot.Collections.Dictionary> results = SelectionCast();

		if (_selected.Count > 0)
		{
			ClearSelection();
		}

		for (int i = 0; i < results.Count; i++)
		{
			Node2D node = results[i]["collider"].As<Node2D>();
			node.Modulate = _selectedNodeColor;
			_selected.Add(node);
		}
	}

	private void SelectAdditional()
	{
		Godot.Collections.Array<Godot.Collections.Dictionary> results = SelectionCast();

		for (int i = 0; i < results.Count; i++)
		{
			Node2D node = results[i]["collider"].As<Node2D>();

			if (!_selected.Contains(node))
			{
				node.Modulate = _selectedNodeColor;
				_selected.Add(node);
			}
		}
	}

	public void ClearSelection()
	{
		foreach (Node2D node in _selected)
		{
			node.Modulate = new Color(1f, 1f, 1f);
		}

		_selected.Clear();
	}

	public void GetSelectedNodes(Action<Node2D> action)
	{
		foreach (Node2D node in _selected)
		{
			action.Invoke(node);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDragging)
		{
			Scale = GetGlobalMousePosition() - _selectStart;

			if (Input.IsActionJustReleased("drag"))
			{
				if (Input.IsActionPressed("multi_select"))
				{
					SelectAdditional();
				}
				else
				{
					SelectNew();
				}

				_isDragging = false;
				_gizmos.Clear();
			}
		}
		else if (Input.IsActionJustPressed("drag"))
		{
			_isDragging = true;
			_selectStart = GetGlobalMousePosition();

			GlobalPosition = _selectStart;
			Scale = GetGlobalMousePosition() - _selectStart;
			_gizmos.AddRect(new Rect2(0f, 0f, Vector2.One), _selectRectColor, GetCanvasItem());
		}
	}
}
