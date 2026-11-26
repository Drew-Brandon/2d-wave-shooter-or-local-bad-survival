namespace Puppets.StatusEffects;
using Godot;
using Puppets.Bodies;

public abstract partial class PuppetStatusEffectType : Resource
{
    public virtual bool IsPhysicsProcessed { get => false; }

    [Export]
    private float _duration = 5f;
    public float Duration { get => _duration; }

    [Export]
    private StringName _name = "";
    public StringName Name { get => _name; }

    public virtual void ProcessEffect(float delta, PuppetBodyTop2D puppet)
    { }
}
