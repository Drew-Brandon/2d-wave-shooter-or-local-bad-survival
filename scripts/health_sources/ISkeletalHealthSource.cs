namespace HealthSources;
using Godot;
using Puppets.Bodies;

/// <summary>
/// Similar to IHealthSource, this interface is used to represent something that has
/// or is attached to something with health.
/// The main difference is that Skeletal health sources have multiple parts/bones
/// that can be healed or damaged.
/// </summary>
public partial interface ISkeletalHealthSource
{
    /// <summary>
    /// Adds health to the specified part of the skeleton.
    /// </summary>
    /// <param name="amount">
    /// The amount to add.
    /// </param>
    /// <param name="part">
    /// The part to add to.
    /// </param>
    /// <param name="instigator">
    /// The node performing the action.
    /// </param>
    public void AddHealthAt(float amount, StringName part, Node instigator = null);
}
