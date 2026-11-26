namespace Puppets.Bodies;

using System;
using System.Collections;
using System.Linq;
using Godot;

[GlobalClass]
public partial class EnemyBody : AIBodyTop2D, IInteractable
{
    [Export]
    private PuppetSpeedMultiplierData _crippleEffect = null;

    [Export]
    private StringNameResource[] _crippleParts = null;

    [Export]
    private int _crippleDuration = 2000;
    
    private CoroutineSource _crippleRoutine = new();

    private IEnumerable CrippleRoutine()
    {
        ApplySpeedMultiplierData(_crippleEffect);
        yield return _crippleDuration;
        ClearSpeedMultiplierData(_crippleEffect);
    }

    private void OnPartDamaged(float amount, StringName part, Node instigator)
    {
        if (Array.IndexOf(_crippleParts, part) != -1)
        {
            if (_crippleRoutine.RoutineCount > 0)
            {
                _crippleRoutine.IsCancelled = true;
                _crippleRoutine = new();
                ClearSpeedMultiplierData(_crippleEffect);
            }

            _crippleRoutine.StartCoroutine(CrippleRoutine());
        }
    }

    public bool StartInteraction(PlayerBodyTop2D player)
    {
        return HealthSource.StartInteraction(player);
    }

    public void StopInteraction(PlayerBodyTop2D player)
    {
        HealthSource.StopInteraction(player);
    }

    public override void _Ready()
    {
        HealthSource.PartDamaged += OnPartDamaged;
    }

    public override void _ExitTree()
    {
        _crippleRoutine.IsCancelled = true;
    }
}
