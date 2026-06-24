using Godot;
using System;

public partial class AINavigationTester : Node2D
{
	[Export]
	private Vector2 _targPos = Vector2.Zero;

	[Export]
	private MouseSelector _selector;

	private void MoveAI(Node2D node)
	{
		for (int i = 0; i < node.GetChildCount(); i++)
		{
			AIBody2D ai = node as AIBody2D;

			if (ai != null)
			{
				ai.TargetPosition = _targPos;
			}
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("confirm"))
		{
			_selector.GetSelectedNodes(MoveAI);
		}
	}
}
