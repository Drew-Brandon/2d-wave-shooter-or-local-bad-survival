namespace AttackInfo;
using System;
using Godot;
using HealthSources;

/// <summary>
/// Represents information about a 2D projectile.
/// </summary>
[GlobalClass]
public partial class ProjectileInfo2D : Resource
{
    [Export]
    private uint _splitCount = 1;

    /// <summary>
    /// The number of projectiles that this splits into.
    /// This is mainly useful for shotgun shells that spray multiple pellets at once.
    /// </summary>
    public uint SplitCount { get => _splitCount; }

    [Export]
    private float _idealRange = 4096f;

    /// <summary>
    /// The ideal range of the projectile.
    /// This acts as the distance that the projectile can travel before becoming ineffective.
    /// </summary>
    public float IdealRange { get => _idealRange; }

    [Export]
    private float _baseDamage = 10f;

    /// <summary>
    /// The base damage of the projectile.
    /// This is the amount of damage dealt by the projectile before any multipliers are applied.
    /// </summary>
    public float BaseDamage { get => _baseDamage; }

    [ExportGroup("Piercing")]
    [Export]
    private uint _pierceCount = 0;
    
    /// <summary>
    /// The number of times the projectile can pierce through targets.
    /// </summary>
    public uint PierceCount { get => _pierceCount; }

    [Export(PropertyHint.Layers2DPhysics)]
    private uint _pierceLayer = 0;
    
    /// <summary>
    /// The layer mask used to determine which targets the projectile can pierce.
    /// </summary>
    public uint PierceLayer { get => _pierceLayer; }

    [Export]
    private float _pierceDamageDecay = 0.5f;
    
    /// <summary>
    /// The rate at which the projectile's damage decays with each pierce.
    /// </summary>
    public float PierceDamageDecay { get => _pierceDamageDecay; }

    [Export]
    private NodePathResource _impactSpawnPath = null;

    [Export]
    private ImpactEffectData2D _impactData = null;

    /// <summary>
    /// Applies the projectile to a target.
    /// </summary>
    /// <param name="user">
    /// The user that fired the projectile.
    /// </param>
    /// <param name="target">
    /// The target to apply the projectile to.
    /// </param>
    /// <param name="position">
    /// The position of the impact.
    /// </param>
    /// <param name="normal">
    /// The normal of the impact.
    /// </param>
    /// <param name="part">
    /// The body part to apply damage to.
    /// </param>
    /// <param name="damageMultiplier">
    /// The damage multiplier to apply.
    /// </param>
    /// <param name="pierceNumber">
    /// The number of pierces that have occurred.
    /// </param>
    /// <exception cref="NullReferenceException">
    /// Thrown when the impact spawn path or impact data is null.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the target is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the pierce number is greater than the projectile's pierce count.
    /// </exception>
    public void ApplyProjectileTo(Node2D user, Node2D target, Vector2 position, Vector2 normal, StringName part, float damageMultiplier = 1f, uint pierceNumber = 0)
    {
        if (_impactSpawnPath == null)
        {
            throw new ArgumentNullException("Impact spawn path for projectile cannot be null.");
        }

        if (_impactData == null)
        {
            throw new ArgumentNullException("Impact data for projectile cannot be null.");
        }

        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        if (pierceNumber > _pierceCount)
        {
            throw new ArgumentOutOfRangeException("pierceNumber");
        }

        // Create and apply the impact effect.
        Node sceneNode = MiscUtils.GetSceneNodeOf(target);
        Node impactParent = sceneNode.GetNode(_impactSpawnPath.Path);
        _impactData.ApplyImpactTo(impactParent, target, position, normal);

        // Apply the damage if the target is or has a health source.
        float damage = _baseDamage * damageMultiplier * Mathf.Pow(_pierceDamageDecay, pierceNumber);
        IHealthSource healthSource = target as IHealthSource;

        if (healthSource != null)
        {
            ISkeletalHealthSource skeletalHealthSource = healthSource as ISkeletalHealthSource;

            if (skeletalHealthSource != null)
            {
                skeletalHealthSource.AddHealthAt(-damage, part, user);
            }
            else
            {
                healthSource.AddHealth(-damage, user);
            }
        }
    }
}
