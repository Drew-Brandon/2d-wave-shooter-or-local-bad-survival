namespace Puppets.StatusEffects;
using Godot;
using Godot.Collections;
using Puppets.Bodies;

[GlobalClass]
public partial class PuppetStatusEffectManager : Node
{
    private Array<PuppetStatusEffect> _effects = new();

    private Array<PuppetStatusEffect> _physicsEffects = new();

    [Export]
    private PuppetBodyTop2D _puppet = null;

    private void InsertEffect(Array<PuppetStatusEffect> effects, PuppetStatusEffect effect)
    {
        int sliceStart = 0;
        int sliceEnd = effects.Count;
        int index = effects.Count / 2;

        while (sliceEnd > sliceStart)
        {
            if (effects[index].RemainingTime > effect.RemainingTime)
            {
                sliceEnd = index;
            }
            else if (effects[index].RemainingTime < effect.RemainingTime)
            {
                sliceStart = index + 1;
            }
            else
            {
                break;
            }

            index = (sliceStart + sliceEnd) / 2;
        }

        effects.Insert(index, effect);
    }

    private void ProcessEffects(float delta, Array<PuppetStatusEffect> effects)
    {
        for (int i = effects.Count - 1; i >= 0 && effects[i].RemainingTime > 0f; i--)
        {
            effects.RemoveAt(effects.Count - 1);
        }

        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].ProcessEffect(delta, _puppet);
        }
    }

    public void AddEffect(PuppetStatusEffectType effectType)
    {
        PuppetStatusEffect effect = new PuppetStatusEffect(effectType);
        InsertEffect(effectType.IsPhysicsProcessed ? _physicsEffects : _effects, effect);
    }

    public void ClearEffects()
    {
        _effects.Clear();
        _physicsEffects.Clear();
    }

    public override void _Process(double delta)
    {
        float deltaF = (float)delta;
        ProcessEffects(deltaF, _effects);
    }

    public override void _PhysicsProcess(double delta)
    {
        float deltaF = (float)delta;
        ProcessEffects(deltaF, _physicsEffects);
    }
}
