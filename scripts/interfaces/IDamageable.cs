using Godot;
using System;

public interface IDamageable
{
	public void AddHealth(Node instigator, float amount);
}
