namespace AttackInfo;
using CastInfo2D;
using Godot;
using HealthSources;
using System;
using System.Collections.Generic;

/// <summary>
/// Simulates something like a melee weapon being swung via hitscan.
/// </summary>
public partial class HitscanSweeper2D : Node2D
{
    [Export(PropertyHint.Range, "0,360,0.1,radians_as_degrees")]
    private float _angleStep = Mathf.DegToRad(9f);

    [Export(PropertyHint.Range, "0,360,0.1,radians_as_degrees")]
    private float _angleRange = Mathf.DegToRad(45f);

    [Export]
    private float _range = 48f;

    [Export]
    private float _baseDamage = 10f;

    [Export]
    private RayCastInfo2D _rayCast = null;

    [Export]
    private NodePathResource _impactSpawnPath = null;

    [Export]
    private ImpactEffectData2D _impactData = null;

    private void HitNode(float power, Node2D target, Vector2 position, Vector2 normal)
    {
        // Create and apply the impact.
        Node sceneNode = MiscUtils.GetSceneNodeOf(target);
        Node impactParent = sceneNode.GetNode(_impactSpawnPath.Path);
        _impactData.ApplyImpactTo(impactParent, target, position, normal);

        // Apply the damage.
        IHealthSource healthSource = target as IHealthSource;

        if (healthSource != null)
        {
            float damage = _baseDamage * power + 0.5f;
            healthSource.AddHealth(-damage, this);
        }
    }

    /// <summary>
    /// Simulates the sweep/swing of a weapon across an area.
    /// </summary>
    /// <param name="power">
    /// The power of the sweep.
    /// </param>
    /// <exception cref="NullReferenceException">
    /// Thrown if the raycast info or impact data is null.
    /// </exception>
    public void Sweep(float power)
    {
        if (_rayCast == null)
        {
            throw new NullReferenceException("Raycast info for sweeper cannot be null.");
        }

        if (_impactData == null)
        {
            throw new NullReferenceException("Impact data for sweeper cannot be null.");
        }

        HashSet<Node2D> hitNodes = new();
        PhysicsDirectSpaceState2D state = GetWorld2D().DirectSpaceState;

        // Sweep the weapon across the area.
        for (float angle = -_angleRange / 2f; angle <= _angleRange / 2f; angle += _angleStep)
        {
            Vector2 start = GlobalPosition;
            Vector2 end = start + GlobalTransform.X.Rotated(angle) * _range;
            PhysicsRayQueryParameters2D query = _rayCast.CreateQuery(start, end);
            RayCastResults2D results = RayCastInfo2D.IntersectRay(state, query);
            Node2D hitNode = results.Collider as Node2D;

            // If the node has not been hit yet, then hit it.
            if (hitNode != null && !hitNodes.Contains(hitNode))
            {
                HitNode(power, hitNode, results.Position, results.Normal);
                hitNodes.Add(hitNode);
            }
        }
    }
}
