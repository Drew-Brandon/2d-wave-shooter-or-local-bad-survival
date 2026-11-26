namespace Puppets.Bodies;
using Godot;
using Controllers;

/// <summary>
/// The body controlled by an AI.
/// </summary>
[GlobalClass]
public partial class AIBodyTop2D : PuppetBodyTop2D
{
	public AIControllerTop2D AIController { get => GetController<AIControllerTop2D>(); }
}
