using Godot;
using System;

public partial class BackpackArea : Area2D
{
	private void OnItemEntered(Node2D obj)
	{
		GD.Print(obj.Name);
	}

	public override void _Ready()
	{
		BodyEntered += OnItemEntered;
	}
}
