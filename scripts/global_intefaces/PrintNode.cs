namespace GlobalInterfaces;
using Godot;

/// <summary>
/// A node purely meant for printing messages to the console.
/// Primarily meant to be used by connecting it to signals in the editor.
/// </summary>
[GlobalClass]
public partial class PrintNode : Node
{
	/// <summary>
	/// Prints out the specified message.
	/// </summary>
	/// <param name="msg">
	/// The message to display.
	/// </param>
	public void Print(string msg)
	{
		GD.Print(msg);
	}
}
