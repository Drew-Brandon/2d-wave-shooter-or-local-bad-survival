using Godot;

public partial class NodeRefInfo : Resource
{
    [Export]
    private StringName _refName = "";
    public StringName RefName { get => _refName; }

    [Export]
    private NodePathResource _nodePath = null;
    public NodePathResource @NodePath { get => _nodePath; }
}
