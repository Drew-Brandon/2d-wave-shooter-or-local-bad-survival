using Godot;

public partial class UIDragRerouter : Control
{
	[Export]
    private bool _canDrag = true;
    public bool CanDrag { get => _canDrag; set => _canDrag = value; }

	[Export]
	private Control _target = null;

    public override Variant _GetDragData(Vector2 atPosition)
    {
        return _target._GetDragData(atPosition);
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return _target._CanDropData(atPosition, data);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        _target._DropData(atPosition, data);
    }
}
