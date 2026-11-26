namespace AttackInfo;
using Godot;
using System;
using ValueLibraries;

/// <summary>
/// Represents the data for a 2D impact effect.
/// This can be used for when something like a projectile hits a surface.
/// </summary>
[GlobalClass]
public partial class ImpactEffectData2D : Resource
{
    [Export]
    private StringNameResource _impactMeta = null;

    /// <summary>
    /// The name of the metadata used to determine the impact effect.
    /// </summary>
    public StringNameResource ImpactMeta { get => _impactMeta; }

    [Export]
    private StringNameResource _impactLayer = null;

    /// <summary>
    /// The name of the layer used to determine the impact effect.
    /// </summary>
    public StringNameResource ImpactLayer { get => _impactLayer; }

    [Export]
    private PackedSceneLibrary _impactLibrary = null;

    /// <summary>
    /// The library used to access the affiliated impact effect.
    /// </summary>
    public PackedSceneLibrary ImpactLibrary { get => _impactLibrary; }

    /// <summary>
    /// Applies the impact effect to a target.
    /// </summary>
    /// <param name="impactParent">
    /// The parent node to add the impact effect to.
    /// </param>
    /// <param name="target">
    /// The target to apply the impact effect to.
    /// </param>
    /// <param name="position">
    /// The position of the impact.
    /// </param>
    /// <param name="normal">
    /// The normal of the impact.
    /// </param>
    /// <exception cref="NullReferenceException">
    /// Thrown when the impact metadata, impact layer, or impact library is null.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the impact parent or target is null.
    /// </exception>
    public void ApplyImpactTo(Node impactParent, Node2D target, Vector2 position, Vector2 normal)
    {
        if (_impactMeta == null)
        {
            throw new NullReferenceException("Impact meta name for impact data cannot be null.");
        }

        if (_impactLayer == null)
        {
            throw new NullReferenceException("Impact layer name for impact data cannot be null.");
        }

        if (_impactLibrary == null)
        {
            throw new NullReferenceException("Impact library for impact data cannot be null.");
        }

        if (impactParent == null)
        {
            throw new ArgumentNullException("impactParent");
        }

        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        // Get the impact effect.
        StringName impactName = "";
        TileMapLayer tileMap = target as TileMapLayer;

        if (tileMap != null)
        {
            Vector2I cellCoords = TileMapUtils.GetCellFromGlobal(tileMap, position);
            TileData tileData = tileMap.GetCellTileData(cellCoords);

            if (tileData.HasCustomData(ImpactLayer.Name))
            {
                impactName = tileData.GetCustomData(ImpactLayer.Name).AsStringName();
            }
        }
        else
        {
            impactName = target.GetMeta(_impactMeta.Name, "").AsStringName();
        }

        // Spawn the impact effect.
        Node2D impact = _impactLibrary.GetValue(impactName).Instantiate<Node2D>();
        impactParent.AddChild(impact);
        impact.GlobalPosition = position;
        impact.Rotation = normal.Angle();
    }
}
