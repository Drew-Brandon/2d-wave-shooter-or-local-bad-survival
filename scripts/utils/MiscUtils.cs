using Godot;
using System.ComponentModel;
using System.Numerics;

public static class MiscUtils
{
	public static bool ApproxEqual<T>(T val, T target, T margin) where T : INumber<T>
	{
		return val >= target - margin && val <= target + margin;
	}
}
