using Godot;

public partial class NoteSlotUI : Control
{
	private NoteData _note = null;
	public NoteData Note { get => _note; }

	[Signal]
	public delegate void NoteAssignedEventHandler(string title);

	[Signal]
	public delegate void NoteReadEventHandler(NoteData note);

	public void ReadNote()
	{
		EmitSignal(SignalName.NoteRead, _note);
	}

	public void SetNote(NoteData note)
	{
		_note = note;
		EmitSignal(SignalName.NoteAssigned, _note.Title);
	}
}
