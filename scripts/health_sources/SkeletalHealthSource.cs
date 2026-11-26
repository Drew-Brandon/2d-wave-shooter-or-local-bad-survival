namespace HealthSources;
using Godot;
using Puppets.Bodies;

/// <summary>
/// A node that has health alongside multiple parts that can be individually healed or damaged.
/// </summary>
[GlobalClass]
public partial class SkeletalHealthSource : HealthSource, ISkeletalHealthSource, IInteractable
{
    private PlayerBodyTop2D _player = null;

    private AimInterface _curInterface = null;

    [Export]
    private SkeletonPartInfo _skeletonInfo = null;

    /// <summary>
    /// Gets the health multipliers for each part of the skeleton.
    /// These are used to scale the amount of health that is added to each part.
    /// </summary>
    public SkeletonPartInfo HealthMultipliers { get => _skeletonInfo; }

    [Export]
    private Node2D _origin = null;

    /// <summary>
    /// Triggered when the health of a specific part changes.
    /// </summary>
    /// <param name="amount">
    /// The amount of health added to the part.
    /// </param>
    /// <param name="part">
    /// The part whose health changed.
    /// </param>
    /// <param name="instigator">
    /// The instigator of the health change.
    /// </param>
    [Signal]
    public delegate void PartHealthChangedEventHandler(float amount, StringName part, Node instigator);

    /// <summary>
    /// Triggered when a specific part is healed.
    /// </summary>
    /// <param name="amount">
    /// The amount of health added to the part.
    /// </param>
    /// <param name="part">
    /// The part that was healed.
    /// </param>
    /// <param name="instigator">
    /// The instigator of the heal.
    /// </param>
    [Signal]
    public delegate void PartHealedEventHandler(float amount, StringName part, Node instigator);
    
    /// <summary>
    /// Triggered when a specific part is damaged.
    /// </summary>
    /// <param name="amount">
    /// The amount of health removed from the part.
    /// </param>
    /// <param name="part">
    /// The part that was damaged.
    /// </param>
    /// <param name="instigator">
    /// The instigator of the damage.
    /// </param>
    [Signal]
    public delegate void PartDamagedEventHandler(float amount, StringName part, Node instigator);

    private void OnAimInterfaceExited()
    {
        _curInterface = null;
        _player.StopInteraction();
    }

    public bool StartInteraction(PlayerBodyTop2D player)
    {
        if (player == null || player.CurrentTool is not PreciseUseableTool)
        {
            return false;
        }

        // Create the aim interface used to target specific parts of the skeleton.
        _curInterface = _skeletonInfo.Interface.Instantiate<AimInterface>();
        _curInterface.ToFollow = _origin;
        _curInterface.ToFace = player;

        _curInterface.Target = this;
        _curInterface.AimFor(player);
        _curInterface.TreeExited += OnAimInterfaceExited;

        _player = player;
        return true;
    }

    public void StopInteraction(PlayerBodyTop2D player)
    {
        if (_curInterface != null)
        {
            _curInterface.TreeExited -= OnAimInterfaceExited;
            _curInterface.QueueFree();
        }

        _player = null;
    }

    public void AddHealthAt(float amount, StringName part, Node instigator = null)
    {
        bool wasAlive = IsAlive;
        float dmgMult = _skeletonInfo.GetMultiplier(part);
        AddHealth(amount * dmgMult, instigator);

        // Don't emit signals if the skeleton was dead before.
        if (wasAlive)
        {
            EmitSignal(SignalName.PartHealthChanged, amount, part, instigator);

            if (amount < 0f)
            {
                EmitSignal(SignalName.PartDamaged, amount, part, instigator);
            }
            else
            {
                EmitSignal(SignalName.PartHealed, amount, part, instigator);
            }
        }
    }
}
