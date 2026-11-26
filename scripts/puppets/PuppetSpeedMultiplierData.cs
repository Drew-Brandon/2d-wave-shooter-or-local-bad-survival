using Godot;

[GlobalClass]
public partial class PuppetSpeedMultiplierData : Resource
{
    [Export]
    private StringName _name = "";
    public StringName Name { get => _name; }

    [Export]
    private float _multiplier = 1f;
    public float Multiplier { get => _multiplier; }
}
