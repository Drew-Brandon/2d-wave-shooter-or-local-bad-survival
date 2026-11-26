using Godot;
using System;

public partial class ChildrenRelocator : Node
{
    [Export]
    private Node _oldParent = null;

    [Export]
    private Node _newParent = null;

    public void RelocateChildren()
    {
        for (int i = 0; i < _oldParent.GetChildCount(); i++)
        {
            Node child = _oldParent.GetChild(i);
            _oldParent.RemoveChild(child);
            _newParent.AddChild(child);
        }

        _oldParent.QueueFree();
    }
}
