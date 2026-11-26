namespace Puppets.StatusEffects;
using System;
using Godot;
using Puppets.Bodies;

public partial class PuppetStatusEffect : RefCounted
{
    private float _remainingTime = 0f;
    public float RemainingTime { get => _remainingTime; }

    private Callable _processEffect = new Callable();

    public PuppetStatusEffect(PuppetStatusEffectType effectType) : base()
    {
        if (effectType == null)
        {
            throw new ArgumentOutOfRangeException("effectType");
        }

        _remainingTime = effectType.Duration;
        _processEffect = Callable.From((float delta, PuppetBodyTop2D puppet) =>
            effectType.ProcessEffect(delta, puppet));
    }

    public void ProcessEffect(float delta, PuppetBodyTop2D puppet)
    {
        if (delta <= 0f)
        {
            throw new ArgumentOutOfRangeException("delta");
        }

        if (puppet == null)
        {
            throw new ArgumentNullException("puppet");
        }

        _remainingTime -= delta;
        _processEffect.Call(delta, puppet);
    }
}
