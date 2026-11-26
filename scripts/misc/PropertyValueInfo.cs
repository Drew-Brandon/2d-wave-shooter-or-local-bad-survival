using Godot;

public partial class PropertyValueInfo : RefCounted
{
    [Export]
    public Variant DefaultValue;
    
    [Export]
    public Variant CurrentValue;
}
