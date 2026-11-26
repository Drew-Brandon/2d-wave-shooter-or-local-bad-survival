using Godot;

public partial class NotesUI : Control
{
	[Export]
	private Control _noteDisplayArea = null;

	[Export]
	private Control _noteButtonArea = null;

	[Export]
	private PackedScene _noteSlot = null;

	private NoteData _previousNote = null;

	private Control _currentNoteDisplay = null;

	private void DisplayNote(NoteData note)
	{
		if (_previousNote == note)
		{
			return;
		}

		if (_currentNoteDisplay != null)
		{
			_currentNoteDisplay.QueueFree();
		}

		NoteDisplay noteDisplay = note.Scene.Instantiate<NoteDisplay>();
		_noteDisplayArea.AddChild(noteDisplay);
		noteDisplay.SetNote(note);
		_currentNoteDisplay = noteDisplay;
		_previousNote = note;
	}

    public void AddNote(NoteData note)
	{
		NoteSlotUI noteSlot = _noteSlot.Instantiate<NoteSlotUI>();
		_noteButtonArea.AddChild(noteSlot);
		noteSlot.SetNote(note);
		noteSlot.NoteRead += DisplayNote;
	}

	public void AddAndDisplayNote(NoteData note)
	{
		AddNote(note);
		DisplayNote(note);
	}
}
