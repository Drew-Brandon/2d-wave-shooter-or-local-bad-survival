using Godot;
using GC = Godot.Collections;

public partial class NodeRefSource : Node
{
    [Export]
    private NodeRefRelay _refRelay = null;

    [Export]
    private GC.Array<NodeRefInfo> _refInfoArr = null;

    private void UpdateRef(NodeRefInfo refInfo)
    {
        Node node = GetNode(refInfo.NodePath.Path);
        _refRelay.UpdateRef(refInfo.RefName, node);
    }

    public override void _Ready()
    {
        for (int i = 0; i < _refInfoArr.Count; i++)
        {
            UpdateRef(_refInfoArr[i]);
        }
    }
}
