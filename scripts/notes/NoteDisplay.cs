using Godot;
using System;

public partial class NoteDisplay : Control
{
	[Signal]
	public delegate void NoteAssignedEventHandler(string title);

	public void SetNote(NoteData note)
	{
		EmitSignal(SignalName.NoteAssigned, note.Title);
	}
}
