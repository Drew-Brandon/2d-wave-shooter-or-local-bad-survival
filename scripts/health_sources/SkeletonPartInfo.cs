namespace HealthSources;
using Godot;
using Godot.Collections;

/// <summary>
/// Information about a skeletal health source.
/// </summary>
[GlobalClass]
public partial class SkeletonPartInfo : Resource
{
    [Export]
    private PackedScene _interface = null;

    /// <summary>
    /// The interface used to target specific parts of the skeleton.
    /// </summary>
    public PackedScene Interface { get => _interface; }

    [Export]
    private Dictionary<StringName, float> _healthMultipliers = null;

    /// <summary>
    /// Gets the health multiplier for a specific part of the skeleton.
    /// </summary>
    /// <param name="part">
    /// The part of the skeleton to get the multiplier for.
    /// </param>
    /// <returns>
    /// The health multiplier for the specified part.
    /// </returns>
    public float GetMultiplier(StringName part)
    {
        if (_healthMultipliers.ContainsKey(part))
        {
            return _healthMultipliers[part];
        }

        return 1f;
    }
}
