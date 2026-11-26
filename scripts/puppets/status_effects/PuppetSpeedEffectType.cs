namespace Puppets.StatusEffects;
using Godot;
using Puppets.Bodies;

[GlobalClass]
public partial class PuppetSpeedEffectType : PuppetStatusEffectType
{
    public override bool IsPhysicsProcessed { get => true; }

    [Export]
    private float _speedMultiplier = 1f;
    public float SpeedMultiplier { get => _speedMultiplier; }

    public override void ProcessEffect(float delta, PuppetBodyTop2D puppet)
    {
        
    }
}
